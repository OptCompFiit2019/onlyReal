using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang.ThreeCodeOptimisations
{
    class DeadOrAliveOptimization
    {
        /// <summary>
        /// Выполняет каскадное удаление живых и мертвых переменных в рамках одного блока <paramref name="block"/>
        /// </summary>
        /// <param name="block"></param>
        public static void DeleteDeadVariables(Block.Block block) =>
            DeleteDeadVariables(block, new Dictionary<string, bool>());

        /// <summary>
        /// Выполняет каскадное удаление живых и мертвых переменных в рамках одного блока <paramref name="block"/>
        /// при использовании предварительной информации о "животноводстве" переменных в словаре <paramref name="variables"/>
        /// </summary>
        /// <param name="block"></param>
        /// <param name="variables"></param>
        public static void DeleteDeadVariables(Block.Block block, Dictionary<string, bool> variables)
        {
            LinkedListNode<Visitors.ThreeCode> current = block.code.Last;
            while (current != null)
            {
                variables[current.Value.result] = false;

                if (current.Value.arg1 != null)
                    variables[current.Value.arg1.ToString()] = true;

                if (current.Value.arg2 != null)
                    variables[current.Value.arg2.ToString()] = true;

                current = current.Previous;
            }

            current = block.code.Last;
            while (current != null)
            {
                var prev = current.Previous;
                if (!variables[current.Value.result])
                {
                    block.code.Remove(current);
                    current = prev;
                    continue;
                }

                variables[current.Value.result] = false;

                if (current.Value.arg1 != null)
                    variables[current.Value.arg1.ToString()] = true;

                if (current.Value.arg2 != null)
                    variables[current.Value.arg2.ToString()] = true;

                current = prev;
            }
        }
    }
}
