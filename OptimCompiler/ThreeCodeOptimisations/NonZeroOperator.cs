using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    public class NonZero_JTJ
    {
        public LinkedList<ThreeCode> code;
        public NonZero_JTJ(ThreeAddressCodeVisitor Code)
        {
            this.code = Code.GetCode();
        }

        public void DeleteEmptyOp() //очистка от пустых операторов
        {
            var line = this.code.First;
            while (line.Next != null)
            {
                if (line.Next.Value.arg1 == null && line.Next.Value.arg2 == null && line.Next.Value.result == null)
                {
                    line = line.Next.Next;
                    continue;
                }
                line = line.Next;
            }
        }

        public void DeleteJumpThroughJump() //устранение переходов через переходы
        {
            var line = this.code.First;
            string lblOld = "", lblNew = "";
            bool inIf = false;
            while (line != null)
            {
                if (inIf && line.Value.operation == ThreeOperator.Goto && line.Value.arg1.ToString() == lblNew)
                {
                    var nextLine = line.Next;
                    code.Remove(line);
                    line = nextLine;
                    continue;
                }

                if (line.Value.operation == ThreeOperator.IfGoto)
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
                        code.Remove(line.Next);
                        line = line.Next;                      
                    }
                    line = line.Next.Next;

                    continue;
                }

                line = line.Next;
            }
        }
    }
}
