using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang.Visitors
{
    class DeleteOfDeadCode
    {
        public LinkedList<ThreeCode> Program { set; get; }

        public ThreeAddressCodeVisitor treeСode;

        public DeleteOfDeadCode(ThreeAddressCodeVisitor code) {
            treeСode = code;
            Program = code.GetCode();
        }
        public void DeleteDeadCode() {
            string a;
            bool abool;
            List<int> removeIndexList = new List<int>();
            var list = Program.ToList();
            int i = list.Count - 1;
            while (i > 0) {
                bool arg2IsNull = true;
                if (list[i].arg2 != null) {
                    a = list[i].arg2.ToString();
                    abool = true;
                    Deleting(list, i, ref removeIndexList, a, abool);
                    arg2IsNull = false;
                }

                a = list[i].arg1.ToString();
                abool = true;
                Deleting(list, i, ref removeIndexList, a, abool);

                if (arg2IsNull) {
                    if (list[i].result != list[i].arg1.ToString()) {
                        a = list[i].result;
                        abool = false;
                        Deleting(list, i, ref removeIndexList, a, abool);
                    }
                } else {
                    if (list[i].result != list[i].arg1.ToString() && list[i].result != list[i].arg2.ToString()) {
                        a = list[i].result;
                        abool = false;
                        Deleting(list, i, ref removeIndexList, a, abool);
                    }
                }
                i--;
            }
            List<ThreeCode> newlist = new List<ThreeCode>();
            for (int ii = 0; ii < Program.Count; ii++) {
                if (removeIndexList.IndexOf(ii) == -1)
                    newlist.Add(list[ii]);
            }
            Program = new LinkedList<ThreeCode>(newlist);
        }

        public void Deleting(List<ThreeCode> list, int i, ref List<int> listInt, string a, bool abool) {
            int j = i - 1;
            bool ab = abool;
            while (j >= 0) {
                bool leftOrRightEqualsA = list[j].arg1.ToString() == a;
                bool resultNotLikeLeftOrRight = list[j].result != list[j].arg1.ToString();
                if (list[j].arg2 != null) {
                    leftOrRightEqualsA = leftOrRightEqualsA || list[j].arg2.ToString() == a;
                    resultNotLikeLeftOrRight = resultNotLikeLeftOrRight && (list[j].result != list[j].arg2.ToString());
                }
                if (leftOrRightEqualsA)
                    ab = true;
                if ((list[j].result == a) && resultNotLikeLeftOrRight) {
                    if (ab == true)
                        ab = false;
                    else
                        listInt.Add(j);
                }
                j--;
            }
        }
    }
}
