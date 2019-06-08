using System;
using SimpleLang.Visitors;
using System.Linq;
using System.Collections.Generic;

namespace SimpleLang.ThreeCodeOptimisations
{
    public class EliminationTranToTranOpt : ThreeCodeOptimiser
    {
        public EliminationTranToTranOpt() { }

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
            program = TranToTranOpt(program);
        }

        private List<LinkedListNode<ThreeCode>> FindGotoNodes(LinkedList<ThreeCode> code)
        {
            var currentNode = code.First;
            var gotoNodes = new List<LinkedListNode<ThreeCode>>();
            var res = new List<LinkedListNode<ThreeCode>>();
            var targetLabels = new Dictionary<string, int>();

            while (currentNode != null)
            {
                var currentValue = currentNode.Value;

                if (currentValue.operation == ThreeOperator.Goto)
                {
                    var tacGoto = currentValue;
                    var label = tacGoto.arg1.ToString();
                    if (targetLabels.ContainsKey(label))
                        targetLabels[label]++;
                    else targetLabels.Add(label, 1);

                    gotoNodes.Add(currentNode);
                }

                currentNode = currentNode.Next;
            }

            var usingTargets = new HashSet<string>(targetLabels
               .Where(x => x.Value == 1)
               .Select(x => x.Key));

            foreach (var node in gotoNodes)
            {
                var gotoNode = node.Value;
                if (usingTargets.Contains(gotoNode.arg1.ToString()))
                    res.Add(node);
            }

            return gotoNodes;
        }

        public LinkedListNode<ThreeCode> FindLabel(LinkedList<ThreeCode> code, string lbl)
        {
            var currentNode = code.First;
            while (currentNode != null)
            {
                var line = currentNode.Value;

                if (Equals(line.label, lbl))
                {
                    return currentNode;
                }
                currentNode = currentNode.Next;
            }
            return null;
        }

        public LinkedListNode<ThreeCode> ConvertGotoToIfGotoWithoutLabel(LinkedListNode<ThreeCode> IfGoto)
        {
            var res = IfGoto;
            res.Value.label = null;
            return res;
        }

        public LinkedList<ThreeCode> TranToTranOpt(LinkedList<ThreeCode> code)
        {
            var currentNode = code.First;
            var gotoNodes = FindGotoNodes(code);
            LinkedListNode<ThreeCode> temp;

            while (currentNode != null)
            {
                var line = currentNode.Value;

                if (line.operation is ThreeOperator.Goto && (temp = FindLabel(code, line.arg1.ToString())).Value.operation is ThreeOperator.Goto)
                {
                    line.arg1 = temp.Value.arg1;
                }

                if (line.operation is ThreeOperator.IfGoto && (temp = FindLabel(code, line.arg2.ToString())).Value.operation is ThreeOperator.Goto)
                {
                    line.arg1 = temp.Value.arg1;
                }

                currentNode = currentNode.Next;
            }

            foreach (var gotoNode in gotoNodes)
            {
                var gotoValue = gotoNode.Value;
                var gotoNode2 = gotoNode;

                var targetNode = FindLabel(code, gotoValue.arg1.ToString());
                if (targetNode == null || targetNode.Value.operation != ThreeOperator.IfGoto)
                    continue;

                var nextNode = targetNode.Next;
                if (nextNode == null)
                    continue;
                
                gotoNode2 = ConvertGotoToIfGotoWithoutLabel(targetNode);
                code.Find(gotoNode.Value).Value = gotoNode2.Value;
                var label = new ThreeAddressStringValue(nextNode.Value.label);
                code.AddAfter(gotoNode2, new ThreeCode("", ThreeOperator.Goto, label));
                code.Remove(targetNode);
            }

            return code;
        }
    }
}
