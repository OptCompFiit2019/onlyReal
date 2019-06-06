using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using SimpleLang.Visitors;
using SimpleParser;

namespace SimpleLang.Compiler
{
    public class ILCodeGenerator
    {
        //http://it.mmcs.sfedu.ru/wiki/%D0%93%D0%B5%D0%BD%D0%B5%D1%80%D0%B0%D1%86%D0%B8%D1%8F_%D0%B8_%D0%B2%D1%8B%D0%BF%D0%BE%D0%BB%D0%BD%D0%B5%D0%BD%D0%B8%D0%B5_IL-%D0%BA%D0%BE%D0%B4%D0%B0
        private Dictionary<string, LocalBuilder> variables;
        private Dictionary<string, Label> labels;
        private Dictionary<string, type> varTypes;
        private GenCodeCreator genc;

        //ToDo for Debug
        public GenCodeCreator GetGenerator()
        {
            return genc;
        }

		public void PrintCommands()
		{
			var lst = genc.commands;
			foreach (string cmd in lst)
			{
				Console.WriteLine(cmd);
			}
		}

        // 0 - int
        // 1 - bool
        // 2 - real
        /// <summary>
        /// Создать переменную по типу
        /// </summary>
        /// <returns>The variable.</returns>
        /// <param name="type">тип переменной (0, 1, 2)</param>
        private LocalBuilder CreateVariable(type t) {
            switch (t) {
                case type.tint:
                    return genc.DeclareLocal(typeof(int));
                case type.tbool:
                    return genc.DeclareLocal(typeof(bool));
                case type.treal:
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
        private type DetectType(ThreeAddressValueType val) {
            if (val == null)
                throw new Exception("Variable is null");

            if (val is ThreeAddressStringValue value) {
                if (!variables.ContainsKey(value.Value))
                    throw new Exception("Variable " + value.Value + " is not found");

                return varTypes[value.Value];
            }

            if (val is ThreeAddressIntValue valueInt)
                return type.tint;

            if (val is ThreeAddressLogicValue valueLogic)
                return type.tbool;

            if (val is ThreeAddressDoubleValue valueDouble)
                return type.treal;

            throw new Exception("Unknown type");
        }
        /// <summary>
        /// Создать переменную и добавить в список переменных
        /// </summary>
        /// <returns>0, 1, 2</returns>
        /// <param name="varTypes">Список переменных с типами</param>
        /// <param name="name">Имя создаваемой переменной</param>
        /// <param name="val">Тип переменной трехадресного кода</param>
        private void AddVariable(string name, ThreeAddressValueType val) {
            if (name == null || name.Length == 0)
                throw new Exception("Variable name is null");

            type t = DetectType(val);
            if (varTypes.ContainsKey(name)) {
                if (varTypes[name] != t)
                    throw new Exception("Previous declaration is different");
                return;
            }
            var v = CreateVariable(t);
            variables.Add(name, v);
            varTypes.Add(name, t);
        }

        /// <summary>
        /// Создать переменную и добавить в список переменных
        /// </summary>
        /// <returns>0, 1, 2</returns>
        /// <param name="varTypes">Список переменных с типами</param>
        /// <param name="name">Имя создаваемой переменной</param>
        /// <param name="val1">Тип переменной 1 трехадресного кода</param>
        /// <param name="val2">Тип переменной 2 трехадресного кода</param>
        private void AddVariable(string name, ThreeAddressValueType val1,
                ThreeAddressValueType val2, bool isLogic) {
            if (name == null || name.Length == 0)
                throw new Exception("Variable name is null");

            type type1 = DetectType(val1);
            type type2 = DetectType(val2);

            if (type1 == type2) {
                type t = isLogic ? type.tbool : type1;

                if (varTypes.ContainsKey(name)) {
                    if (varTypes[name] != t)
                        throw new Exception("Previous declaration is different");
                    return;
                }

                var v = CreateVariable(t);
                variables.Add(name, v);
                varTypes.Add(name, t);
                return;
            }

            // int + double = double
            if ((type1 == type.tint && type2 == type.treal)
                    || (type1 == type.treal && type2 == type.tint)) {
                if (varTypes.ContainsKey(name)) {
                    if (varTypes[name] != type.treal)
                        throw new Exception("Previous declaration is different");
                    return;
                }

                type t = isLogic ? type.tbool : type.treal;

                var v = CreateVariable(t);
                variables.Add(name, v);
                varTypes.Add(name, t);
                return;
            }

            //ToDo Подумать результаты операций

            throw new Exception("Unknown situation");
        }

        private void Init(LinkedList<ThreeCode> code) {
            genc = new GenCodeCreator();
            labels = new Dictionary<string, Label>();
            variables = new Dictionary<string, LocalBuilder>();
            varTypes = new Dictionary<string, type>();

            foreach (var v in SymbolTable.vars)
            {
                type t = v.Value;
                variables.Add(v.Key, CreateVariable(t));
                varTypes.Add(v.Key, t);
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
                        || command.operation == ThreeOperator.IfGoto
                        || command.operation == ThreeOperator.Println)
                    continue;

                if (command.result == null || command.result.Length == 0)
                    throw new Exception("Variable name is null");

                // Если операция принимает 1 аргумент
                if (command.operation == ThreeOperator.Assign
                    || command.operation == ThreeOperator.Logic_not)
                    AddVariable(command.result, command.arg1);
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

                    AddVariable(command.result, command.arg1, command.arg2, isLogic);
                }
            }
        }

        private void GenBinOpCode(ThreeOperator op)
        {
            switch (op)
            {
                case ThreeOperator.Plus:
                    genc.Emit(OpCodes.Add);
                    break;
                case ThreeOperator.Minus:
                    genc.Emit(OpCodes.Sub);
                    break;
                case ThreeOperator.Mult:
                    genc.Emit(OpCodes.Mul);
                    break;
                case ThreeOperator.Div:
                    genc.Emit(OpCodes.Div);
                    break;
                case ThreeOperator.Logic_and:
                    genc.Emit(OpCodes.And);
                    break;
                case ThreeOperator.Logic_or:
                    genc.Emit(OpCodes.Or);
                    break;
                case ThreeOperator.Logic_equal:
                    genc.Emit(OpCodes.Ceq);
                    break;
                case ThreeOperator.Logic_greater:
                    genc.Emit(OpCodes.Cgt);
                    break;
                case ThreeOperator.Logic_less:
                    genc.Emit(OpCodes.Clt);
                    break;
                case ThreeOperator.Logic_geq:
                    genc.Emit(OpCodes.Clt);
                    genc.Emit(OpCodes.Not);
                    break;
                case ThreeOperator.Logic_leq:
                    genc.Emit(OpCodes.Cgt);
                    genc.Emit(OpCodes.Not);
                    break;
                case ThreeOperator.Logic_neq:
                    genc.Emit(OpCodes.Ceq);
                    genc.Emit(OpCodes.Not);
                    break;
            }
        }

        private enum CastOptions { No, ToDouble, ToInt }

        private void LoadValue(ThreeAddressValueType v, CastOptions opts = CastOptions.No)
        {
            switch (v)
            {
                case ThreeAddressStringValue sv:
                    genc.Emit(OpCodes.Ldloc, variables[sv.Value]);
                    break;
                case ThreeAddressIntValue iv:
                    genc.Emit(OpCodes.Ldc_I4, iv.Value);
                    break;
                case ThreeAddressDoubleValue dv:
                    genc.Emit(OpCodes.Ldc_R8, dv.Value);
                    break;
                case ThreeAddressLogicValue lv:
                    genc.Emit(lv.Value ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
                    break;
            }
            if (opts != CastOptions.No)
                CastToResult(DetectType(v), opts);
        }

        private CastOptions CastToResultOpt(type res_type)
        {
            switch (res_type)
            {
                case type.treal: return CastOptions.ToDouble;
                case type.tint: return CastOptions.ToInt;
                default: return CastOptions.No;
            }
        }

        private void CastToResult(type t, CastOptions opts)
        {
            if (opts == CastOptions.ToDouble && t == type.tint)
                genc.Emit(OpCodes.Conv_R8);
            else if (opts == CastOptions.ToInt && t == type.treal)
                genc.Emit(OpCodes.Conv_I4);
        }

        private type ResultType(ThreeAddressValueType v1, ThreeAddressValueType v2)
        {
            type type1 = DetectType(v1);
            type type2 = DetectType(v2);

            if (type1 == type2)
                return type1;

            // int + double = double
            if ((type1 == type.tint && type2 == type.treal)
                    || (type1 == type.treal && type2 == type.tint))
                return type.treal;

            throw new Exception("Unknown situation");
        }

        public void Generate(LinkedList<ThreeCode> code)
        {
            Init(code);
            foreach (var command in code)
            {
                if (command.label?.Length > 0)
                    genc.MarkLabel(labels[command.label]);
                switch (command.operation)
                {
                    case ThreeOperator.None:
                        continue;
                    case ThreeOperator.Goto:
                        genc.Emit(OpCodes.Br, labels[command.arg1.ToString()]);
                        continue;
                    case ThreeOperator.IfGoto:
                        LoadValue(command.arg1);
                        genc.Emit(OpCodes.Brtrue_S, labels[command.arg2.ToString()]);
                        continue;
                    case ThreeOperator.Println:
                        LoadValue(command.arg1);
                        genc.EmitWriteLine(DetectType(command.arg1));
                        continue;

                    case ThreeOperator.Assign:
                        LoadValue(command.arg1/*, CastToResultOpt(varTypes[command.result])*/);
                        break;
                    case ThreeOperator.Logic_not:
                        LoadValue(command.arg1);
                        genc.Emit(OpCodes.Not);
                        break;
                    default:
                        type rt = ResultType(command.arg1, command.arg2);
                        CastOptions castOpt = CastToResultOpt(rt);
                        LoadValue(command.arg1, castOpt);
                        LoadValue(command.arg2, castOpt);
                        GenBinOpCode(command.operation);
                        //CastToResult(rt, CastToResultOpt(varTypes[command.result]));
                        break;
                }
                genc.Emit(OpCodes.Stloc, variables[command.result]);
            }
            genc.EndProgram();
        }

        public string Execute()
        {
            return genc.RunProgram();
        }
    }
}