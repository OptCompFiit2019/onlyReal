using SimpleLang.Visitors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SimpleLang.ThreeCodeOptimisations
{
    class LVNOptimization
    {
        /// <summary>
        /// Возвращает результат применения к блоку <paramref name="sourceBlock"/> оптимизацию LVN как новый блок.
        /// </summary>
        /// <param name="sourceBlock"></param>
        public static LinkedList<ThreeCode> LVNOptimize(LinkedList<ThreeCode> sourceBlock)
        {
            var block = new LinkedList<ThreeCode>(sourceBlock);
            Dictionary<string, int> dict = new Dictionary<string, int>();
            Dictionary<int, LinkedListNode<ThreeCode>> ValueDict = new Dictionary<int, LinkedListNode<ThreeCode>>();

            int i = 1, n = 0, tmpNum = 1;
            string Key;
            var strCode = block.First;
            while (strCode != null)
            {
                n++;
                // условие для проверки строки 3-х адр. кода на соответствие виду бин. операции
                if (strCode.Value.operation == Visitors.ThreeOperator.None
                    || strCode.Value.operation == Visitors.ThreeOperator.IfGoto
                    || strCode.Value.operation == Visitors.ThreeOperator.Goto)
                {
                    strCode = strCode.Next;
                    continue;
                }

                if (!dict.Keys.Contains(strCode.Value.arg1.ToString()))
                    dict[strCode.Value.arg1.ToString()] = i++;

                Key = dict[strCode.Value.arg1.ToString()].ToString();
                if (strCode.Value.arg2 != null && !dict.Keys.Contains(strCode.Value.arg2.ToString()))
                    dict[strCode.Value.arg2.ToString()] = i++;

                Key = strCode.Value.arg2 == null? strCode.Value.arg1.ToString() : 
                    dict[strCode.Value.arg1.ToString()].ToString() + strCode.Value.operation + dict[strCode.Value.arg2.ToString()];

                if (!dict.Keys.Contains(Key))
                    dict[Key] = i++;

                if (ValueDict.Keys.Contains(dict[Key]))
                {
                    if (dict[ValueDict[dict[Key]].Value.result] != dict[Key])
                    {
                        block.AddAfter(ValueDict[dict[Key]],
                            new ThreeCode("t" + tmpNum++, ValueDict[dict[Key]].Value.arg1));
                        ValueDict[dict[Key]] = ValueDict[dict[Key]].Next;
                    }
                    strCode.Value.operation = ThreeOperator.Assign;
                    strCode.Value.arg2 = null;
                    strCode.Value.arg1 = new ThreeAddressStringValue(ValueDict[dict[Key]].Value.result);
                }

                dict[strCode.Value.result] = dict[Key];
                ValueDict[dict[Key]] = strCode;
                strCode = strCode.Next;
            }

            return block;
        }

        /// <summary>
        /// Возвращает результат применения к графу <paramref name="graph"/> оптимизацию LVN как новый граф.
        /// </summary>
        /// <param name="graph"></param>
        public static ControlFlowGraph.ControlFlowGraph LVNOptimize(ControlFlowGraph.ControlFlowGraph graph)
        {
            var resGraph = new ControlFlowGraph.ControlFlowGraph(
                new List<LinkedList<Visitors.ThreeCode>>(graph.blocks));

            foreach (var block in resGraph.blocks)
            {
                var replace = LVNOptimization.LVNOptimize(block);
                block.Clear();
                foreach (var line in replace)
                    block.AddLast(line);
            }

            return resGraph;
        }
    }
}