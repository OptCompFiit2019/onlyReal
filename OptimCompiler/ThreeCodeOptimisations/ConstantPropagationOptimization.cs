using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CFG = SimpleLang.ControlFlowGraph.ControlFlowGraph;
using SimpleLang.Visitors;
using SimpleLang.GenericIterativeAlgorithm;

namespace SimpleLang.ThreeCodeOptimisations
{
    using ConstPropBlockInfo = BlockInfo<KeyValuePair<string, ConstPropSemilatticeEl>>;
    using ConstPropKeyValue = KeyValuePair<string, ConstPropSemilatticeEl>;

    public partial class ConstantPropagationOptimizer
    {
        private CFG controlFlowGraph;
        private List<HashSet<ConstPropKeyValue>> Ins;

        public void IterativeAlgorithm(List<LinkedList<ThreeCode>> blocks)
        {
            // построение CFG по блокам
            controlFlowGraph = new CFG(blocks.ToList());
            // создание информации о блоках
            var blocksInfo = new List<ConstPropBlockInfo>();
            var m = new Dictionary<string, ConstPropSemilatticeEl>();
            for (int i = 0; i < blocks.Count; i++)
            {
                foreach (var c in blocks[i].Where(com =>
                    com.operation != ThreeOperator.Goto && com.operation != ThreeOperator.IfGoto))
                {
                    string[] vars = new string[]
                        { c.result
                        , (c.arg1 as ThreeAddressStringValue)?.Value
                        , (c.arg2 as ThreeAddressStringValue)?.Value };

                    foreach (var v in vars)
                        if (v != null && v != "" && !m.ContainsKey(v))
                            m[v] = new ConstPropSemilatticeEl(ValConstType.Undef);
                }
            }
            for (int i = 0; i < blocks.Count; i++)
                blocksInfo.Add(new ConstPropBlockInfo(blocks[i]));

            // оператор сбора в задаче о распространении констант
            Func<List<ConstPropBlockInfo>, CFG, int, ConstPropBlockInfo> meetOperator =
                (blocksInfos, graph, index) =>
                {
                    var inputIndexes = graph.cfg.GetInputNodes(index);
                    var resInfo = new ConstPropBlockInfo(blocksInfos[index]);
                    foreach (var i in inputIndexes)
                    {
                        var resIn = resInfo.IN.ToDictionary(e => e.Key);
                        foreach (var Out in blocksInfos[i].OUT)
                            if (resIn[Out.Key].Value.Constantness == ValConstType.Undef)
                                resIn[Out.Key] = new ConstPropKeyValue(Out.Key, Out.Value);
                            else if (resIn[Out.Key].Value.Constantness == ValConstType.NAC
                                        || Out.Value.Constantness == ValConstType.NAC
                                        || (resIn[Out.Key].Value.Constantness == ValConstType.Const
                                                && Out.Value.Constantness == ValConstType.Const
                                                && resIn[Out.Key].Value.Value != Out.Value.Value))
                                resIn[Out.Key] = new ConstPropKeyValue(Out.Key,
                                    new ConstPropSemilatticeEl(ValConstType.NAC));

                        resInfo.IN = new HashSet<ConstPropKeyValue>(resIn.Values);
                    }
                    return resInfo;
                };

            var transferFunction = TransferFunction();

            // создание объекта итерационного алгоритма
            var iterativeAlgorithm = new IterativeAlgorithm<ConstPropKeyValue>(blocksInfo,
                controlFlowGraph, meetOperator, true, new HashSet<ConstPropKeyValue>(m),
                new HashSet<ConstPropKeyValue>(m), transferFunction);

            // выполнение алгоритма
            iterativeAlgorithm.Perform();
            Ins = iterativeAlgorithm.GetINs();
        }

        public CFG ApplyOptimization(List<LinkedList<ThreeCode>> blocks)
        {
            IterativeAlgorithm(blocks);
            var bs = controlFlowGraph.blocks;

            for (int i = 0; i < bs.Count; ++i)
            {
                var m = Ins[i].ToDictionary(e => e.Key);
                for (var it = bs[i].First; true; it = it.Next)
                {
                    var command = it.Value;
                    if (command.arg1 is ThreeAddressStringValue v1 && m.ContainsKey(v1.Value)
                            && m[v1.Value].Value.Constantness == ValConstType.Const)
                        command.arg1 = new ThreeAddressIntValue(m[v1.Value].Value.Value);

                    if (command.arg2 is ThreeAddressStringValue v2 && m.ContainsKey(v2.Value)
                            && m[v2.Value].Value.Constantness == ValConstType.Const)
                        command.arg2 = new ThreeAddressIntValue(m[v2.Value].Value.Value);
                    if (it == bs[i].Last)
                        break;
                }
            }
            return controlFlowGraph;
        }
    }
}
