using System;
using SimpleLang.Visitors;
using System;
using System.Collections.Generic;


namespace SimpleLang.ThreeCodeOptimisations
{
    public class DeadOrAliveOptimizationAdapter: ThreeCodeOptimiser
    {
        public DeadOrAliveOptimizationAdapter()
        {
        }
        private bool _apply = false;
        public bool Applyed() {
            return _apply;
        }
        public bool NeedFullCode() { return false; }
        public void Apply(ref System.Collections.Generic.List<System.Collections.Generic.LinkedList<SimpleLang.Visitors.ThreeCode>> res) { throw new Exception("Not implemented"); }

        public void Apply(ref LinkedList<ThreeCode> program)
        {
            _apply = false;
            program = DeleteDeadVariables(program);
        }


        /// <summary>
        /// Возвращает результат каскадного удаления живых и мертвых переменных,
        /// выполненного над передаваемым блоком <paramref name="block"/>.
        /// </summary>
        /// <param name="block"></param>
        public LinkedList<ThreeCode> DeleteDeadVariables(LinkedList<ThreeCode> block)
        {
            var variables = new Dictionary<string, bool>();
            // сначала добавляются все аргументы, которые хоть раз были определены
            // это консервативное предположение о последующем использовании этих определений
            foreach (var line in block)
                if (!line.result.StartsWith("temp_"))
                    variables[line.result] = true;
            return DeleteDeadVariables(block, variables);
        }

        /// <summary>
        /// Возвращает результат каскадного удаления живых и мертвых переменных,
        /// выполненного над передаваемым блоком <paramref name="block"/>.
        /// При этом используется предварительная информация о "живости" переменных 
        /// из словаря <paramref name="variables"/>
        /// </summary>
        /// <param name="block"></param>
        /// <param name="variables"></param>
        public LinkedList<ThreeCode> DeleteDeadVariables(LinkedList<ThreeCode> block, Dictionary<string, bool> variables)
        {
            var result = new LinkedList<ThreeCode>(block);
            LinkedListNode<ThreeCode> current = result.Last;

            while (current != null)
            {
                var previous = current.Previous;
                if (!(variables.ContainsKey(current.Value.result) && variables[current.Value.result]) && current.Value.result != "")
                    if (current.Value.label != null && current.Value.label != "") { 
                        current.Value = new ThreeCode(current.Value.label, "", ThreeOperator.None, null, null);
                        _apply = true;
                    } else { 
                        result.Remove(current);
                        _apply = true;
                    }
                else
                    AddThreeCodeLine(current.Value, variables);
                current = previous;
            }
            return result;
        }

        /// <summary>
        /// Добавляет информацию о переменных строки кода <paramref name="line"/> в
        /// словарь <paramref name="variables"/>
        /// </summary>
        /// <param name="line"></param>
        /// <param name="variables"></param>
        private void AddThreeCodeLine(ThreeCode line, Dictionary<string, bool> variables)
        {
            variables[line.result] = false;

            if (line.arg1 != null && line.arg1 is ThreeAddressStringValue && line.arg1.ToString() != "")
                variables[line.arg1.ToString()] = true;

            if (line.arg2 != null && line.arg2 is ThreeAddressStringValue && line.arg2.ToString() != "")
                variables[line.arg2.ToString()] = true;
        }

        /// <summary>
        /// Возвращает результат каскадного удаления живых и мертвых переменных,
        /// выполненного над графом <paramref name="graph"/>.
        /// </summary>
        /// <param name="graph"></param>
        public static ControlFlowGraph.ControlFlowGraph DeleteDeadVariables(ControlFlowGraph.ControlFlowGraph graph)
        {
            var resGraph = new ControlFlowGraph.ControlFlowGraph(
                new List<LinkedList<Visitors.ThreeCode>>(graph.blocks));
            foreach (var block in resGraph.blocks)
            {
                var replace = DeadOrAliveOptimization.DeleteDeadVariables(block);
                block.Clear();
                foreach (var line in replace)
                    block.AddLast(line);
            }

            return resGraph;
        }
    }
}
