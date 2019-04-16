using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang.ThreeCodeOptimisations
{
    class DeadOrAliveOptimization
    {
        public static void DeleteDeadVariables(LinkedList<Visitors.ThreeCode> block, Dictionary<string, bool> variables)
        {
            var current = block.Last;
            while (current != null)
            {
                variables[current.Value.result] = false;

                if (current.Value.arg1 != null)
                    variables[current.Value.arg1] = true;

                if (current.Value.arg2 != null)
                    variables[current.Value.arg2] = true;

                current = current.Previous;
            }

            current = block.Last;
            while (current != null)
            {
                var prev = current.Previous;
                if (!variables[current.Value.result])
                {
                    block.Remove(current);
                    current = prev;
                    continue;
                }

                variables[current.Value.result] = false;

                if (current.Value.arg1 != null)
                    variables[current.Value.arg1] = true;

                if (current.Value.arg2 != null)
                    variables[current.Value.arg2] = true;

                current = prev;
            }
        }
    }
}
