using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleLang.Visitors;
using GenericTransferFunction;
using CFG = SimpleLang.ControlFlowGraph.ControlFlowGraph;

namespace SimpleLang.GenericIterativeAlgorithm
{
    class ConstantPropagationItA
    {
        public List<Dictionary<string, string>> IN, OUT;
        CFG Graph;

        static string UNDEF = "UNDEF", NAC = "NAC";

        public static bool IsConstant(Dictionary<string, string> vars, string var) =>
             (vars[var] != UNDEF && vars[var] != NAC);


        public ConstantPropagationItA(CFG graph)
        {
            Graph = graph;



            IN = new List<Dictionary<string, string>>();
            OUT = new List<Dictionary<string, string>>();
        }

        /// <summary>
        /// Передаточная функция для задачи о распространении констант
        /// </summary>
        Func<Dictionary<string, string>, LinkedList<ThreeCode>, Dictionary<string, string>> Function = (vars, block) =>
        {
            var res = new Dictionary<string, string>(vars);
            foreach (var line in block)
                if (line.result != null && line.result != "")
                    if (line.operation == ThreeOperator.Assign)
                    {
                        if (line.arg1 is ThreeAddressStringValue)
                            res[line.result] = res[line.arg1.ToString()];
                        else if (!res.ContainsKey(line.result) || res[line.result] != NAC)
                            res[line.result] = line.arg1.ToString();
                    }
                    else if (line.operation != ThreeOperator.None && line.operation != ThreeOperator.Println
                    && line.operation != ThreeOperator.Goto && line.operation != ThreeOperator.IfGoto)
                    {
						if (!res.ContainsKey(line.arg1.ToString())
                            || !res.ContainsKey(line.arg2.ToString()))
                        {
                            res[line.result] = UNDEF;
                        }
                        if (res.ContainsKey(line.arg1.ToString())
                                && res.ContainsKey(line.arg2.ToString())
                                && IsConstant(res, res[line.arg1.ToString()]) 
                                && IsConstant(res, res[line.arg2.ToString()]))
                            res[line.result] = line.arg1.ToString() + line.operation.ToString() + line.arg2.ToString();

                        if (!res.ContainsKey(line.arg1.ToString())
                                || !res.ContainsKey(line.arg2.ToString())
                                || res[line.arg1.ToString()] == NAC 
                                || res[line.arg2.ToString()] == NAC)
                            res[line.result] = NAC;
                        else
                            res[line.result] = UNDEF;
                    }
            return res;
        };

        /// <summary>
        /// Оператор сбора на полурешетке Vi для распространения констант
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        string MeetTwoVariables(string first, string second)
        {
            if (first == NAC || second == NAC)
                return NAC;
            if (first == UNDEF)
                return second;
            if (second == UNDEF)
                return first;
            if (first == second)
                return first;
            return NAC;
        }

        /// <summary>
        /// Оператор сбора для всех предков блока с индексом index
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="OUT"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        Dictionary<string, string> MeetOperator(int index)
        {
            var pred = Graph.cfg.GetInputNodes(index);
            var res = new Dictionary<string, string>(pred[0]);
            for (int i = 1; i < pred.Count; i++)
                foreach (var variable in res.Keys)
                    res[variable] = MeetTwoVariables(res[variable], OUT[i][variable]);
            return res;
        }

        /// <summary>
        /// Выполнение итерационного алгоритма для текущего графа Graph
        /// </summary>
        public void PerformAlgorithm()
        {
            //Начальная инициализация множеств
            var AllVariables = GetVariables();

            for (int i = 0; i < Graph.blocks.Count; i++)
            {
                IN.Add(new Dictionary<string, string>(AllVariables));
                OUT.Add(new Dictionary<string, string>(AllVariables));
            }

            // Сам итерационный алгоритм
            bool isChanged = true;
            while (isChanged)
            {
                isChanged = false;
                for (int i = 1; i < Graph.blocks.Count; i++)
                {
                    var prevOUT = new Dictionary<string, string>(OUT[i]);
                    IN[i] = MeetOperator(i);
                    OUT[i] = Function(IN[i], Graph.blocks[i]);
                    isChanged = isChanged || IsDictionariesEquals(prevOUT, OUT[i]);
                }
            }
        }

        /// <summary>
        /// Возвращает словарь с именами всех переменных, встреченных в программе 
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetVariables()
        {
            var variables = new Dictionary<string, string>();
            foreach (var block in Graph.blocks)
                foreach (var line in block)
                {
                    if (line.result != null && line.result != "" && !variables.ContainsKey(line.result))
                        variables.Add(line.result, UNDEF);
                    if (line.arg1 != null && line.arg1 is ThreeAddressStringValue && !variables.ContainsKey(line.arg1.ToString()))
                        variables.Add(line.arg1.ToString(), UNDEF);
                    if (line.arg2 != null && line.arg2 is ThreeAddressStringValue && !variables.ContainsKey(line.arg2.ToString()))
                        variables.Add(line.arg2.ToString(), UNDEF);
                }
            return variables;
        }

        static bool IsDictionariesEquals(Dictionary<string, string> first, Dictionary<string, string> second)
        {
            foreach (var v in first.Keys)
                if (!second.ContainsKey(v) || first[v] != second[v])
                    return false;
            return true;
        }
    }
}
