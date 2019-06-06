using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang.Visitors
{
    class PullOfCopies
    {
        public LinkedList<ThreeCode> Program { set; get; }

        public ThreeAddressCodeVisitor treeСode;

        public PullOfCopies(ThreeAddressCodeVisitor code) {
            treeСode = code;
            Program = code.GetCode();
        }

        public void PullCopies() {
            var list = Program.ToList();
            for (int i = 0; i < Program.Count - 1; i++) {

                string left;
                ThreeAddressValueType right;

                if (list[i].arg2 == null) {
                    left = list[i].result;
                    right = list[i].arg1;
                } else continue;

                for (int j = i + 1; j < Program.Count; j++) {
                    if (list[j].result == left)
                        break;
                    if (list[j].arg1.ToString() == left)
                        list[j].arg1 = right;

                    if (list[i].arg2 != null && list[j].arg2.ToString() == left)
                        list[j].arg2 = right;
                }
            }
        }
    }
}
