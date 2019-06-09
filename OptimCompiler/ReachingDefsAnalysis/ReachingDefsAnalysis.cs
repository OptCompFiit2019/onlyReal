using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CFG = SimpleLang.ControlFlowGraph.ControlFlowGraph;
using SimpleLang.Visitors;
using SimpleLang.GenericIterativeAlgorithm;

namespace SimpleLang.ThreeCodeOptimisations
{
    public class ReachingDefsAnalysis
    {
        private CFG controlFlowGraph;

        public List<HashSet<ThreeCode>> Ins { get; private set; }
        public List<HashSet<ThreeCode>> Outs { get; private set; }

        public void IterativeAlgorithm(List<LinkedList<ThreeCode>> blocks)
        {
            // построение CFG по блокам
            controlFlowGraph = new CFG(blocks.ToList());
            // создание информации о блоках
            var blocksInfo = new List<BlockInfo<ThreeCode>>();
            for (int i = 0; i < blocks.Count; i++)
                blocksInfo.Add(new BlockInfo<ThreeCode>(blocks[i]));

            // оператор сбора в задаче о распространении констант
            Func<List<BlockInfo<ThreeCode>>, CFG, int, BlockInfo<ThreeCode>> meetOperator =
                (blocksInfos, graph, index) =>
                {
                    var inputIndexes = graph.cfg.GetInputNodes(index);
                    var resInfo = new BlockInfo<ThreeCode>(blocksInfos[index]);
                    foreach (var i in inputIndexes)
                        resInfo.IN.UnionWith(blocksInfos[i].OUT);
                    return resInfo;
                };

            var transferFunction = new ReachingDefsAdaptor(controlFlowGraph).TransferFunction();

            // создание объекта итерационного алгоритма
            var iterativeAlgorithm = new IterativeAlgorithm<ThreeCode>(blocksInfo,
                controlFlowGraph, meetOperator, true, new HashSet<ThreeCode>(),
                new HashSet<ThreeCode>(), transferFunction);

            // выполнение алгоритма
            iterativeAlgorithm.Perform();
            Ins = iterativeAlgorithm.GetINs();
            Outs = iterativeAlgorithm.GetOUTs();
        }

        public void PrintOutput()
        {
            Console.WriteLine("IN");
            PrintSets(Ins);
            Console.WriteLine(Environment.NewLine + "OUT");
            PrintSets(Outs);
        }

        public string GetOutput()
        {
            string result = "IN" + Environment.NewLine;
            for (int i = 0; i < Ins.Count; ++i)
            {
                var sets = Ins[i].Select(c => $"    {c.ToString()}" + Environment.NewLine);
                if (sets.Count() == 0)
                    sets = new string[] { "" };
                result += $"Block {i + 1} :" + Environment.NewLine + sets.Aggregate((s1, s2) => s1 + s2) + Environment.NewLine;
            }
            result += Environment.NewLine + "OUT" + Environment.NewLine;
            for (int i = 0; i < Outs.Count; ++i)
            {
                var sets = Outs[i].Select(c => $"    {c.ToString()}" + Environment.NewLine);
                if (sets.Count() == 0)
                    sets = new string[] { "" };
                result += $"Block {i + 1} :" + Environment.NewLine + sets.Aggregate((s1, s2) => s1 + s2) + Environment.NewLine;
            }
            return result;
        }
        private void PrintSets(List<HashSet<ThreeCode>> Sets)
        {
            for (int i = 0; i < Sets.Count; ++i)
            {
                var sets = Sets[i].Select(c => $"    {c.ToString()}" + Environment.NewLine);
                if (sets.Count() == 0)
                    sets = new string[] { "" };
                Console.WriteLine($"Block {i + 1} :" + Environment.NewLine + sets.Aggregate((s1, s2) => s1 + s2));
            }
        }
    }
}
