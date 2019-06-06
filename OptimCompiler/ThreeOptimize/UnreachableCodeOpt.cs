using System;
using SimpleLang.Visitors;
using System.Collections.Generic;

namespace SimpleLang.ThreeCodeOptimisations
{
    public class UnreachableCodeOpt : ThreeCodeOptimiser
    {
        public UnreachableCodeOpt() { }

        private bool _apply = false;
        public bool Applyed()
        {
            return _apply;
        }
        public bool NeedFullCode() { return false; }
        public void Apply(ref System.Collections.Generic.List<System.Collections.Generic.LinkedList<SimpleLang.Visitors.ThreeCode>> res) { throw new Exception("Not implemented"); }

        public void Apply(ref LinkedList<ThreeCode> program)
        {
            _apply = false;
            program = DeleteUnreachCode(program);
        }

        private HashSet<object> FindAllTargetLabels(LinkedList<ThreeCode> tac)
        {
            var setOfLables = new HashSet<object>();
            foreach (var line in tac) {
                if (line.operation is ThreeOperator.IfGoto)
                    setOfLables.Add(line.arg2);
                if (line.operation is ThreeOperator.Goto)
                    setOfLables.Add(line.arg1);
            }
            return setOfLables;
        }


        private bool CheckLabels(LinkedListNode<ThreeCode> ifNode, object targetLabel, List<ThreeCode> NodesToRemove)
        {
            var currentNode = ifNode;
            var line = currentNode.Value;

            while (!Equals(line.label, targetLabel))
            {
                NodesToRemove.Add(line);
                currentNode = currentNode.Next;
                line = currentNode.Value;
            }

            return true;
        }

        public ThreeCode ConvertIfGotoToGoto(ThreeCode code)
        {
            var new_code = code;
            new_code.arg1 = code.arg2;
            new_code.arg2 = null;
            new_code.operation = ThreeOperator.Goto;
            return new_code;
        }


        public LinkedList<ThreeCode> DeleteUnreachCode(LinkedList<ThreeCode> code)
        {
            var targetLabels = FindAllTargetLabels(code);

            var currentNode = code.First;
            var linesToDelete = new List<ThreeCode>();

            while (currentNode != null)
            {
                var line = currentNode.Value;

                if (line.operation is ThreeOperator.IfGoto && line.arg1.ToString().Equals("True"))
                {
                    if (CheckLabels(currentNode.Next, line.arg2.ToString(), linesToDelete))
                    {
                        line = ConvertIfGotoToGoto(line);
                        foreach (var line_del in linesToDelete)
                            code.Remove(line_del);
                        linesToDelete.Clear();
                        _apply = true;
                    }
                }

                currentNode = currentNode.Next;
            }

            return code;
        }
    }
}
