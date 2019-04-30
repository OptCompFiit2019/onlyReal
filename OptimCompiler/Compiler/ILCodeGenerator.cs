using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using SimpleLang.Visitors;

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
                    throw new Exception("Unknow type.");
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

            throw new Exception("Unknow type");
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
                    throw new Exception("Previus declaration is different");
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
        private int AddVariable(ref Dictionary<string, int> varTypes, string name, ThreeAddressValueType val1, ThreeAddressValueType val2 ) {
            if (name == null || name.Length == 0)
                throw new Exception("Variable name is null");
            //if (variables.ContainsKey(name))
             //   throw new Exception("Variable " + name + " allready exists.");

            int type1 = DetectType(varTypes, val1);
            int type2 = DetectType(varTypes, val2);

            if (type1 == type2) {
                if (varTypes.ContainsKey(name)) {
                    if (varTypes[name] != type1)
                        throw new Exception("Previus declaration is different");
                    return type1;
                }

                var v = CreateVariable(type1);
                variables.Add(name, v);
                varTypes.Add(name, type1);
                return type1;
            }

            // int + double = double
            if ((type1 == 0 && type2 == 2) || (type1 == 2 && type2 == 0)) {
                if (varTypes.ContainsKey(name)) {
                    if (varTypes[name] != 2)
                        throw new Exception("Previus declaration is different");
                    return varTypes[name];
                }
                var v = CreateVariable(2);
                variables.Add(name, v);
                varTypes.Add(name, 2);
                return 2;
            }

            //ToDo Подумать результаты операций

            // int + bool = int
            if ((type1 == 0 && type2 == 1) || (type1 == 1 && type2 == 0)) {
                if (varTypes.ContainsKey(name)) {
                    if (varTypes[name] != 0)
                        throw new Exception("Previus declaration is different");
                    return varTypes[name];
                }

                var v = CreateVariable(0);
                variables.Add(name, v);
                varTypes.Add(name, 0);
                return 0;
            }

            // double + bool = double
            if ((type1 == 1 && type2 == 2) || (type1 == 2 && type2 == 1)) {
                if (varTypes.ContainsKey(name)) {
                    if (varTypes[name] != 2)
                        throw new Exception("Previus declaration is different");
                    return varTypes[name]; 
                }
                var v = CreateVariable(2);
                variables.Add(name, v);
                varTypes.Add(name, 2);
                return 2;
            }

            throw new Exception("Unknow situation");
        }

        private void Init(LinkedList<ThreeCode> code) {
            genc = new GenCodeCreator();
            labels = new Dictionary<string, Label>();
            variables = new Dictionary<string, LocalBuilder>();

            // 0 - int
            // 1 - bool
            // 2 - real
            Dictionary<string, int> varTypes = new Dictionary<string, int>();

            for (var it = code.First; it != null; it = it.Next) {
                //ThreeOperator {  None, Assign, Minus, Plus, Mult, Div, Goto, IfGoto,
                //Logic_or, Logic_and, Logic_less, Logic_equal, Logic_greater, Logic_leq,
        //Logic_not, Logic_neq };
                ThreeCode comand = it.Value;


                // Создание метки
                if (comand.label != null && comand.label.Length > 0) {
                    if (labels.ContainsKey(comand.label))
                        throw new Exception("Duble label.");
                    Label label = genc.DefineLabel();
                    labels.Add(comand.label, label);
                }

                // Пропускаем операторы без создания переменных
                if (comand.operation == ThreeOperator.None 
                        || comand.operation == ThreeOperator.Goto
                        || comand.operation == ThreeOperator.IfGoto)
                    continue;

                if (comand.result == null || comand.result.Length == 0)
                    throw new Exception("Variable name is null");

                // Если операция принимает 1 аргумент
                if (comand.operation == ThreeOperator.Assign) {
                    ThreeAddressValueType value = comand.arg1;

                    int type = AddVariable(ref varTypes, comand.result, value);
                    continue;
                }
                if (comand.operation == ThreeOperator.Logic_not) {
                    var v = CreateVariable(1);
                    variables.Add(comand.result, v);
                    varTypes.Add(comand.result, 1);
                    continue;
                }

                // Если операция принимает 2 аргумента

                // Если логическая операция
                if (comand.operation == ThreeOperator.Logic_and
                        || comand.operation == ThreeOperator.Logic_or
                        || comand.operation == ThreeOperator.Logic_geq
                        || comand.operation == ThreeOperator.Logic_leq
                        || comand.operation == ThreeOperator.Logic_neq
                        || comand.operation == ThreeOperator.Logic_less
                        || comand.operation == ThreeOperator.Logic_equal
                        || comand.operation == ThreeOperator.Logic_greater)
                {
                    var v = CreateVariable(1);
                    variables.Add(comand.result, v);
                    varTypes.Add(comand.result, 1);
                    continue;
                }

                if (comand.operation == ThreeOperator.Minus
                        || comand.operation == ThreeOperator.Plus
                        || comand.operation == ThreeOperator.Div
                        || comand.operation == ThreeOperator.Mult)
                {
                    int type = AddVariable(ref varTypes, comand.result, comand.arg1, comand.arg2);
                    continue;
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