using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleLang.Visitors;

namespace SimpleLang.ThreeCodeOptimisations
{
    class IterAlgoActiveVariables
    {
        /// <summary>
        /// Итерационный алгоритм для активных переменных. Находит для графа <paramref name="graph"/> множества
        /// IN (<paramref name="InBlocks"/>) и OUT (<paramref name="OutBlocks"/>) на основе ранее вычисленных
        /// Def (<paramref name="DefBlocks"/>) и Use (<paramref name="UseBlocks"/>).
        /// Граф должен содержать фиктивный узел ВЫХОД.
        /// </summary>
        /// <param name="DefBlocks"></param>
        /// <param name="UseBlocks"></param>
        /// <param name="InBlocks"></param>
        /// <param name="OutBlocks"></param>
        /// <param name="graph"></param>
        public static void IterativeAlgorithm(DefUseBlocks defBUseBBlocks,
            List<List<string>> InBlocks, List<List<string>> OutBlocks, ControlFlowGraph.ControlFlowGraph graph)
        {
            bool isInChanged = true;
            for (int i = 0; i < InBlocks.Count(); i++)
            {
                InBlocks[i] = new List<string>();
                OutBlocks[i] = new List<string>();
            }

            while(isInChanged)
                for(int i = 0; i < InBlocks.Count - 1; i++)
                {
                    var previousIn = new string[InBlocks[i].Count];
                    InBlocks[i].CopyTo(previousIn);
                    OutBlocks[i] = MeetOperator(graph, i, InBlocks);
                    InBlocks[i] = defBUseBBlocks.UseBs[i].Union(OutBlocks[i].Except(defBUseBBlocks.DefBs[i])).ToList();
                    if (!InBlocks[i].SequenceEqual(InBlocks[i]))
                        isInChanged = false;
                }
        }

        /// <summary>
        /// Оператор сбора для задачи анализа активных переменных
        /// </summary>
        /// <param name="graph">Граф потоков управления</param>
        /// <param name="index">Индекс анализируемого блока B</param>
        /// <param name="InBlocks">Множества IN[B]</param>
        /// <returns></returns>
        private static List<string> MeetOperator(ControlFlowGraph.ControlFlowGraph graph, int index, List<List<string>> InBlocks)
        {
            var successors = graph.GetAsGraph().GetOutputNodes(index);
            var OutBlock = new List<string>();
            foreach (var i in successors)
                OutBlock = OutBlock.Union(InBlocks[i]).ToList();
            return OutBlock;
        }
    }
}
