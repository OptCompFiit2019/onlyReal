using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;
using SimpleLang.Visitors;

namespace SimpleLang.ThreeCodeOptimisations
{
    public class NonZero_JTJOpt : ThreeCodeOptimiser
    {
        public List<LinkedList<ThreeCode>> code;
        public LinkedList<ThreeCode> realCode=new LinkedList<ThreeCode>();
        private bool _apply = false;

        public void Apply(ref LinkedList<ThreeCode> program)
        {

        }

        public void Apply(ref List<LinkedList<ThreeCode>> res)
        {
            _apply = false;
            code = res;
            DeleteEmptyOp();
            DeleteJumpThroughJump();
            var tmp = new List<LinkedList<ThreeCode>>();
            tmp.Add(realCode);
            res = tmp;
            realCode = new LinkedList<ThreeCode>();
        }

        public bool Applyed()
        {
            return _apply;
        }

        public void DeleteEmptyOp() //очистка от пустых операторов
        {

            for (var k = 0; k < code.Count; k++)
            {
                var bl = code[k].ToList();
                foreach (var l in bl)
                {
                    realCode.AddLast(l);
                }

            }
            var line = realCode.First;
            while (line.Next != null)
            {
                if (line.Next.Value.arg1 == null && line.Next.Value.arg2 == null && line.Next.Value.result == null)
                {
                    line = line.Next.Next;
                    _apply = true;
                    continue;
                }
                line = line.Next;
            }
          
        }

        public void DeleteJumpThroughJump() //устранение переходов через переходы
        {
            realCode = new LinkedList<ThreeCode>();
            for (var k = 0; k < code.Count; k++)
            {
                var bl = code[k].ToList();
                foreach (var l in bl)
                {
                    realCode.AddLast(l);
                }
                
            }
                var line = realCode.First;
                string lblOld = "", lblNew = "";
                bool inIf = false;
                while (line != null)
                {
                    if (inIf && line.Value.operation == ThreeOperator.Goto && line.Value.arg1.ToString() == lblNew)
                    {
                        var nextLine = line.Next;
                        realCode.Remove(line);
                        line = nextLine;
                        _apply = true;
                        continue;
                    }

                    if (line.Value.operation == ThreeOperator.IfGoto && (line.Value.arg1 is ThreeAddressLogicValue) && line.Next.Value.operation == ThreeOperator.Goto)
                    {
                        inIf = true;

                        var tmp = line.Value.arg1 as ThreeAddressLogicValue;
                        var t = !tmp.Value;
                        (line.Value.arg1 as ThreeAddressLogicValue).Value = t;

                        if (!string.IsNullOrEmpty(line.Value.arg2.ToString()))
                        {
                            var ee = line.Value.arg2.ToString();
                            lblOld = line.Value.arg2.ToString();
                            var nn = line.Next.Value.arg1;
                            (line.Value.arg2 as ThreeAddressStringValue).Value = (line.Next.Value.arg1 as ThreeAddressStringValue).Value;
                            realCode.Remove(line.Next);
                            realCode.Remove(line.Next);
                            line = line.Next;
                        }
                        line = line.Next.Next;
                        _apply = true;

                        continue;
                    }
                    line = line.Next;
                }
        }

        public bool NeedFullCode()
        {
            return true;
        }
    }
}
