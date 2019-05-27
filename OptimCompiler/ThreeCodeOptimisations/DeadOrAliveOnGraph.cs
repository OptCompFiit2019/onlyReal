using System;
using System.Collections.Generic;
using SimpleLang.ThreeCodeOptimisations;

namespace SimpleLang.ThreeCodeOptimisations
{
    class ControlFlowOptimisations
    {
        /// <summary>
        /// Применяет к графу <paramref name="graph"/> удаление мертвых переменных на основе
        /// информации о множествах IN и OUT для блоков из <paramref name="InOut"/>.
        /// Возвращает новый граф с выполненной на нем оптимизацией без изменения исходного графа.
        /// </summary>
        /// <param name="InOut"></param>
        /// <param name="graph"></param>
        /// <returns></returns>
        public static ControlFlowGraph.ControlFlowGraph DeadOrAliveOnGraph(List<HashSet<string>> OutBlocks, ControlFlowGraph.ControlFlowGraph graph)
        {
            var resGraph = new ControlFlowGraph.ControlFlowGraph(
                new List<LinkedList<Visitors.ThreeCode>>(graph.blocks));

            if (OutBlocks.Count != resGraph.blocks.Count)
                throw new ArgumentException("The number of elements in the sets OUT, graph.blocks must be equal");

            for (int i = resGraph.blocks.Count-1; i >= 0; i--)
            {
                var variables = new Dictionary<string, bool>();
                foreach (var v in OutBlocks[i])
                    variables[v] = true;

                resGraph.blocks[i] = DeadOrAliveOptimization.DeleteDeadVariables(resGraph.blocks[i], variables);
            }
            return resGraph;
        }
    }
}
