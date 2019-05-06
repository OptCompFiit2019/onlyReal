using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using SimpleLang.Visitors;
using SimpleParser;

namespace SimpleLang.Compiler
{
    class ILCodeGenerator
    {
        //http://it.mmcs.sfedu.ru/wiki/%D0%93%D0%B5%D0%BD%D0%B5%D1%80%D0%B0%D1%86%D0%B8%D1%8F_%D0%B8_%D0%B2%D1%8B%D0%BF%D0%BE%D0%BB%D0%BD%D0%B5%D0%BD%D0%B8%D0%B5_IL-%D0%BA%D0%BE%D0%B4%D0%B0
        private Dictionary<string, LocalBuilder> variables;
        private Dictionary<string, Label> labels;
        private GenCodeCreator genc;

        //ToDo for Debug
        public GenCodeCreator GetGenerator()
        {
            return genc;
        }

        // 0 - int
        // 1 - bool
        // 2 - real
        /// <summary>
        /// Создать переменную по типу
        /// </summary>
        /// <returns>The variable.</returns>
        /// <param name="type">тип переменной (0, 1, 2)</param>
        private LocalBuilder CreateVariable(int type) {
            switch (type) {
                case 0:
                    return genc.DeclareLocal(typeof(int));
                case 1:
                    return genc.DeclareLocal(typeof(bool));
                case 2:
                    return genc.DeclareLocal(typeof(double));
                default:
                    throw new Exception("Unknown type");
            }
        }

        // Вывести тип переменной
        /// <summary>
        /// Определить тип переменной
        /// </summary>
        /// <returns>0, 1, 2</returns>
        /// <param name="varTypes">Список существующих переменных с типами</param>
        /// <param name="val">Тип переменной трехадресного кода</param>
        private int DetectType(Dictionary<string, int> varTypes, ThreeAddressValueType val) {
            if (val == null)
                throw new Exception("Variable is null");

            if (val is ThreeAddressStringValue value) {
                if (!variables.ContainsKey(value.Value))
                    throw new Exception("Variable " + value.Value + " is not found");

                int typ = varTypes[value.Value];
                return typ;
            }

            if (val is ThreeAddressIntValue valueInt)
                return 0;

            if (val is ThreeAddressLogicValue valueLogic)
                return 1;

            if (val is ThreeAddressDoubleValue valueDouble)
                return 2;

            throw new Exception("Unknown type");
        }
        /// <summary>
        /// Создать переменную и добавить в список переменных
        /// </summary>
        /// <returns>0, 1, 2</returns>
        /// <param name="varTypes">Список переменных с типами</param>
        /// <param name="name">Имя создаваемой переменной</param>
        /// <param name="val">Тип переменной трехадресного кода</param>
        private int AddVariable(ref Dictionary<string, int> varTypes, string name, ThreeAddressValueType val) {
            if (name == null || name.Length == 0)
                throw new Exception("Variable name is null");

            int type = DetectType(varTypes, val);
            if (varTypes.ContainsKey(name)) {
                if (varTypes[name] != type)
                    throw new Exception("Previous declaration is different");
                return type;
            }
            var v = CreateVariable(type);
            variables.Add(name, v);
            varTypes.Add(name, type);
            return type;
        }

        /// <summary>
        /// Создать переменную и добавить в список переменных
        /// </summary>
        /// <returns>0, 1, 2</returns>
        /// <param name="varTypes">Список переменных с типами</param>
        /// <param name="name">Имя создаваемой переменной</param>
        /// <param name="val1">Тип переменной 1 трехадресного кода</param>
        /// <param name="val2">Тип переменной 2 трехадресного кода</param>
        private int AddVariable(ref Dictionary<string, int> varTypes, string name,
                ThreeAddressValueType val1, ThreeAddressValueType val2, bool isLogic) {
            if (name == null || name.Length == 0)
                throw new Exception("Variable name is null");

            int type1 = DetectType(varTypes, val1);
            int type2 = DetectType(varTypes, val2);

            if (type1 == type2) {
                if (varTypes.ContainsKey(name)) {
                    if (varTypes[name] != type1)
                        throw new Exception("Previous declaration is different");
                    return type1;
                }

                int type = isLogic ? 1 : type1;

                var v = CreateVariable(type);
                variables.Add(name, v);
                varTypes.Add(name, type);
                return type;
            }

            // int + double = double
            if ((type1 == 0 && type2 == 2) || (type1 == 2 && type2 == 0)) {
                if (varTypes.ContainsKey(name)) {
                    if (varTypes[name] != 2)
                        throw new Exception("Previous declaration is different");
                    return varTypes[name];
                }
                var v = CreateVariable(2);
                variables.Add(name, v);
                varTypes.Add(name, 2);
                return 2;
            }

            //ToDo Подумать результаты операций

            throw new Exception("Unknown situation");
        }

        private void Init(LinkedList<ThreeCode> code) {
            genc = new GenCodeCreator();
            labels = new Dictionary<string, Label>();
            variables = new Dictionary<string, LocalBuilder>();

            // 0 - int
            // 1 - bool
            // 2 - real
            Dictionary<string, int> varTypes = new Dictionary<string, int>();

            foreach (var v in SymbolTable.vars)
            {
                int type = (int)v.Value;
                variables.Add(v.Key, CreateVariable(type));
                varTypes.Add(v.Key, type);
            }

            for (var it = code.First; it != null; it = it.Next) {
                //ThreeOperator {  None, Assign, Minus, Plus, Mult, Div, Goto, IfGoto,
                //Logic_or, Logic_and, Logic_less, Logic_equal, Logic_greater, Logic_leq,
                //Logic_not, Logic_neq };
                ThreeCode command = it.Value;

                // Создание метки
                if (command.label != null && command.label.Length > 0) {
                    if (labels.ContainsKey(command.label))
                        throw new Exception("Label already exists");
                    Label label = genc.DefineLabel();
                    labels.Add(command.label, label);
                }

                // Пропускаем операторы без создания переменных
                if (command.operation == ThreeOperator.None 
                        || command.operation == ThreeOperator.Goto
                        || command.operation == ThreeOperator.IfGoto)
                    continue;

                if (command.result == null || command.result.Length == 0)
                    throw new Exception("Variable name is null");

                // Если операция принимает 1 аргумент
                if (command.operation == ThreeOperator.Assign
                    || command.operation == ThreeOperator.Logic_not)
                    AddVariable(ref varTypes, command.result, command.arg1);
                else
                {
                    bool isLogic = command.operation == ThreeOperator.Logic_and
                        || command.operation == ThreeOperator.Logic_or
                        || command.operation == ThreeOperator.Logic_geq
                        || command.operation == ThreeOperator.Logic_leq
                        || command.operation == ThreeOperator.Logic_neq
                        || command.operation == ThreeOperator.Logic_less
                        || command.operation == ThreeOperator.Logic_equal
                        || command.operation == ThreeOperator.Logic_greater;

                    AddVariable(ref varTypes, command.result, command.arg1, command.arg2, isLogic);
                }
            }
        }
        public void Generate(LinkedList<ThreeCode> code)
        {
            Init(code);
        }

        public void Execute()
        {

        }
    }
}