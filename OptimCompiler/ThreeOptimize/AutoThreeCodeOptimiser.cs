using System;
using System.Collections.Generic;
using SimpleLang.Visitors;
using SimpleLang.Block;

namespace SimpleLang.ThreeCodeOptimisations{

    public class AutoThreeCodeOptimiser: ThreeCodeOptimiser{
        private List<ThreeCodeOptimiser> dd = new List<ThreeCodeOptimiser>();
        private bool b = false;

        public void Add(ThreeCodeOptimiser a) { dd.Add(a);  }
        public void Apply(LinkedList<ThreeCode> program) {
            bool need = true;
            while (need) {
                need = false;
                for (int i = 0; i < dd.Count; i++) {
                    dd[i].Apply(program);
                    if (dd[i].Applyed()) {
                        need = true;
                        b = true;
                        break;
                    }
                }
            }
        }
        public List<LinkedList<ThreeCode>> Apply(ThreeAddressCodeVisitor visit) {
            Block.Block bl = new Block.Block(visit);
            List<LinkedList<ThreeCode>> res = bl.GenerateBlocks();

            for (int i = 0; i < res.Count; i++) {
                Apply(res[i]);
            }

            return res;
        }
        public bool Applyed() {
            return b;
        }
    }
}
