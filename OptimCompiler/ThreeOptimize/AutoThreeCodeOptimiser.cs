using System;
using System.Collections.Generic;
using SimpleLang.Visitors;
using SimpleLang.Block;

namespace SimpleLang.ThreeCodeOptimisations{

    public class AutoThreeCodeOptimiser: ThreeCodeOptimiser{
        private List<ThreeCodeOptimiser> dd = new List<ThreeCodeOptimiser>();
        private bool b = false;

        public void Add(ThreeCodeOptimiser a) { dd.Add(a);  }
        public void Apply(ref LinkedList<ThreeCode> program) {
            bool need = true;
            while (need) {
                need = false;
                for (int i = 0; i < dd.Count; i++) {
                    dd[i].Apply(ref program);
                    if (dd[i].Applyed()) {
                        need = true;
                        b = true;
                        break;
                    }
                }
            }
        }
        public bool NeedFullCode() { return true; }
        public void Apply(ref List<LinkedList<ThreeCode>> res) {
            bool need = true;
            while (need)
            {
                b = false;
                need = false;
                for (int i = 0; i < dd.Count; i++)
                    if (dd[i].NeedFullCode()) {
                        dd[i].Apply(ref res);
                        if (dd[i].Applyed()) {
                            need = true;
                            b = true;
                            break;
                        }
                    }
                if (need) continue;
                for (int i = 0; i < res.Count; i++) {
                    LinkedList<ThreeCode> tmp = res[i];
                    Apply(ref tmp);
                    res[i] = tmp;
                }
                if (b)
                    need = true;
            }
        }

        public List<LinkedList<ThreeCode>> Apply(ThreeAddressCodeVisitor visit) {
            Block.Block bl = new Block.Block(visit);
            List<LinkedList<ThreeCode>> res = bl.GenerateBlocks();

            Apply(ref res);

            return res;
        }
        public bool Applyed() {
            return b;
        }
    }
}
