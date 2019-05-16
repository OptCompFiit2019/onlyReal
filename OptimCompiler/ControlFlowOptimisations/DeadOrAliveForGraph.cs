using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleLang.ThreeCodeOptimisations;

namespace SimpleLang.ControlFlowOptimisations
{

    class ControlFlowOptimisations
    {
        //    пройти по списку блоков и используя OUT как начальные сведения о "живости" переменных произвести оптимизацию
        public static void DeadOrAliveOnGraph(List<List<string>> OUT, ControlFlowGraph.ControlFlowGraph graph)
        {
            if (OUT.Count != graph.blocks.Count)
                throw new ArgumentException("Число элементов во множествах OUT, graph.blocks должно совпадать");

            for (int i = graph.blocks.Count; i > 0; i--)
            {
                var variables = new Dictionary<string, bool>();
                foreach (var v in OUT[i])
                    variables[v] = true;

                DeadOrAliveOptimization.DeleteDeadVariables(graph.blocks[i], variables);
            }
        }
    }
}
