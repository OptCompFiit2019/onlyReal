using SimpleLang.Visitors;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleLang.ThreeCodeOptimisations
{
    class DeadOrAliveOptimization
    {
        /// <summary>
        /// Выполняет каскадное удаление живых и мертвых переменных в рамках одного блока <paramref name="block"/>.
        /// </summary>
        /// <param name="block"></param>
        public static void DeleteDeadVariables(LinkedList<ThreeCode> block)
        {
            var variables = new Dictionary<string, bool>();
            AddThreeCodeLine(block.Last.Value, variables);
            foreach(var line in block)
                if(!line.result.StartsWith("temp_"))
                    variables[line.result] = true;
            DeleteDeadVariables(block, variables);
        }

        /// <summary>
        /// Выполняет каскадное удаление живых и мертвых переменных в рамках одного блока <paramref name="block"/>
        /// при использовании предварительной информации о "живости" переменных в словаре <paramref name="variables"/>
        /// </summary>
        /// <param name="block"></param>
        /// <param name="variables"></param>
        public static void DeleteDeadVariables(LinkedList<ThreeCode> block, Dictionary<string, bool> variables)
        {
            LinkedListNode<ThreeCode> current = block.Last;

            while (current != null)
            {
                var previous = current.Previous;
                if (!(variables.ContainsKey(current.Value.result) && variables[current.Value.result]) && current.Value.result != "")
                    block.Remove(current);
                else
                    AddThreeCodeLine(current.Value, variables);
                current = previous;
            }
        }

        private static void AddThreeCodeLine(ThreeCode line, Dictionary<string, bool> variables)
        {
            variables[line.result] = false;

            if (line.arg1 != null)
                variables[line.arg1.ToString()] = true;

            if (line.arg2 != null)
                variables[line.arg2.ToString()] = true;
        }
    }
}
