using System;
using System.Collections.Generic;

namespace SimpleLang.ThreeCodeOptimisations
{
    class InOutActiveVariables
    {
        public List<HashSet<string>> InBlocks { get; } // множества IN для блоков
        public List<HashSet<string>> OutBlocks { get; } // множества OUT для блоков

        /// <summary>
        /// Итерационный алгоритм для активных переменных. Находит для графа <paramref name="graph"/> множества
        /// IN (<paramref name="InBlocks"/>) и OUT (<paramref name="OutBlocks"/>) на основе ранее вычисленных
        /// Def и Use (<paramref name="defUseBlocks"/>). IN и OUT сохраняются в объекте IterAlgoActiveVariables
        /// Граф должен содержать фиктивный узел ВЫХОД.
        /// </summary>
        /// <param name="defUseBlocks"></param>
        /// <param name="graph"></param>
        public InOutActiveVariables(DefUseBlocks defUseBlocks, ControlFlowGraph.ControlFlowGraph graph)
        {
            InBlocks = new List<HashSet<string>>();
            OutBlocks = new List<HashSet<string>>();
            for(int i = 0; i < defUseBlocks.DefBs.Count; i++)
            {
                InBlocks.Add(new HashSet<string>());
                OutBlocks.Add(new HashSet<string>());
            }
            bool isInChanged = true;

            while(isInChanged)
            {
                isInChanged = false;
                for(int i = 0; i < defUseBlocks.DefBs.Count - 1; i++)
                {
                    var previousIn = new string[InBlocks[i].Count];
                    InBlocks[i].CopyTo(previousIn);
                    OutBlocks[i] = MeetOperator(graph, i);
                    var Except = new HashSet<string>(OutBlocks[i]);
                    Except.ExceptWith(defUseBlocks.DefBs[i]);
                    InBlocks[i].UnionWith(Except);
                    InBlocks[i].UnionWith(defUseBlocks.UseBs[i]);
                    isInChanged = isInChanged || !InBlocks[i].SetEquals(previousIn);
                }
            }
        }

        /// <summary>
        /// Оператор сбора для задачи анализа активных переменных
        /// </summary>
        /// <param name="graph">Граф потоков управления</param>
        /// <param name="index">Индекс анализируемого блока B</param>
        /// <returns></returns>
        private HashSet<string> MeetOperator(ControlFlowGraph.ControlFlowGraph graph, int index)
        {
            var successors = graph.GetAsGraph().GetOutputNodes(index);
            var OutBlock = new HashSet<string>();
            foreach (var i in successors)
                OutBlock.UnionWith(InBlocks[i]);
            return OutBlock;
        }
    }
}
