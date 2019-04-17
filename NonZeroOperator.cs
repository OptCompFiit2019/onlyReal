using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    public class Task1
    {
        public LinkedList<ThreeCode> code;
        public Task1(ThreeAddressCodeVisitor Code)
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

                if (line.Value.label == lblNew)
                {
                    line.Value.label = "";
                }

                if (line.Value.label == lblOld)
                {
                    line.Value.label = lblNew;
                }

                if (line.Value.operation == ThreeOperator.IfGoto)
                {
                    inIf = true;
                    var tmp = line.Value.arg1 as ThreeAddressLogicValue;
                    var t = !tmp.Value;
                    (line.Value.arg1 as ThreeAddressLogicValue).Value = t;

                    if (!string.IsNullOrEmpty(line.Value.arg2.ToString()))
                    {
                        lblOld = line.Value.arg2.ToString();
                        var fp = String.Concat(lblOld.TakeWhile(x => !char.IsDigit(x)));
                        var spstr = String.Concat(lblOld.SkipWhile(x => !char.IsDigit(x)).TakeWhile(x => char.IsDigit(x)));
                        var sp = (int.Parse(spstr) + 1).ToString();
                        lblNew = fp + sp;
                        (line.Value.arg2 as ThreeAddressStringValue).Value = lblNew;

                    }
                    line = line.Next.Next;
                    //code.Remove(line.Next);

                    continue;
                }

                line = line.Next;
            }
        }
    }
}
