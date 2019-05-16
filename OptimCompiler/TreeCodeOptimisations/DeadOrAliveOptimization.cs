using SimpleLang.Visitors;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleLang.ThreeCodeOptimisations
{
    class DeadOrAliveOptimization
    {
        /// <summary>
        /// Выполняет каскадное удаление живых и мертвых переменных в рамках одного блока <paramref name="block"/>
        /// </summary>
        /// <param name="block"></param>
        public static void DeleteDeadVariables(LinkedList<ThreeCode> block)
        {
            var variables = new Dictionary<string, bool>();
            AddThreeCodeLine(block.Last.Value, variables);
            variables[block.Last.Value.result] = true;
            DeleteDeadVariables(block, variables);
        }

        /// <summary>
        /// Выполняет каскадное удаление живых и мертвых переменных в рамках одного блока <paramref name="block"/>
        /// при использовании предварительной информации о "животноводстве" переменных в словаре <paramref name="variables"/>
        /// </summary>
        /// <param name="block"></param>
        /// <param name="variables"></param>
        public static void DeleteDeadVariables(LinkedList<ThreeCode> block, Dictionary<string, bool> variables)
        {
            LinkedListNode<ThreeCode> current = block.Last;
            #region
            //while (current == null)
            //{
            //    //if (current.Value.result == "" || current.Value.arg1.ToString() == "" || current.Value.arg2.ToString() == "")
            //    //    continue;
            //    variables[current.Value.result] = false;

            //    if (current.Value.arg1 != null)
            //        variables[current.Value.arg1.ToString()] = true;

            //    if (current.Value.arg2 != null)
            //        variables[current.Value.arg2.ToString()] = true;

            //    current = current.Previous;
            //}
            #endregion
            // Удаление мертвых переменных
            current = block.Last;

            //variables[block.Last.Value.result] = false;
            //if (current.Value.arg1 != null)
            //    variables[current.Value.arg1.ToString()] = true;

            //if (current.Value.arg2 != null)
            //    variables[current.Value.arg2.ToString()] = true;

            //current = current.Previous;

            while (current != null)
            {
                var prev = current.Previous;

                if (variables.ContainsKey(current.Value.result))
                    AddThreeCodeLine(current.Value, variables);

                //bool inVariables = variables.TryGetValue(current.Value.result, out bool currentResult);
                if (!variables.ContainsKey(current.Value.result) || !variables[current.Value.result])
                {

                    //if (current.Value.arg1 != null)
                    //    variables[current.Value.arg1.ToString()] = false;

                    //if (current.Value.arg2 != null)
                    //    variables[current.Value.arg2.ToString()] = false;

                    block.Remove(current);
                    current = prev;
                    continue;
                }

                //if(!inVariables && current.Value.result.StartsWith("temp_"))

                current = prev;
            }
        }

        private static void AddThreeCodeLine(ThreeCode line, Dictionary<string, bool> variables)
        {
            bool isInVariables = variables.TryGetValue(line.result, out bool currentResult);

            variables[line.result] = false;

            if (line.arg1 != null)
                variables[line.arg1.ToString()] = true;

            if (line.arg2 != null)
                variables[line.arg2.ToString()] = true;
        }
    }
}
