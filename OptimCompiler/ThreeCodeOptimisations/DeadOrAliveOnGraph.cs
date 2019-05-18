using System;
using System.Collections.Generic;
using SimpleLang.ThreeCodeOptimisations;

namespace SimpleLang.ThreeCodeOptimisations
{
    class ControlFlowOptimisations
    {
        //    пройти по списку блоков и используя OUT как начальные сведения о "живости" переменных произвести оптимизацию
        public static void DeadOrAliveOnGraph(InOutActiveVariables InOut, ControlFlowGraph.ControlFlowGraph graph)
        {
            if (InOut.OutBlocks.Count != graph.blocks.Count)
                throw new ArgumentException("The number of elements in the sets OUT, graph.blocks must be equal");

            for (int i = graph.blocks.Count-1; i >= 0; i--)
            {
                var variables = new Dictionary<string, bool>();
                foreach (var v in InOut.OutBlocks[i])
                    variables[v] = true;

                DeadOrAliveOptimization.DeleteDeadVariables(graph.blocks[i], variables);
            }
        }
    }
}
