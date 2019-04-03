using System;
using System.Collections.Generic;
using SimpleLang.Visitors;
using SimpleLang.Block;
namespace SimpleLang.ThreeOptimize
{
    public class Applyer: ThreeCodeApplyer
    {
        private List<ThreeCodeApplyer> dd = new List<ThreeCodeApplyer>();
        private bool b = false;
        public Applyer() { }
        public void Add(ThreeCodeApplyer a) { dd.Add(a);  }
        public void Apply(System.Collections.Generic.LinkedList<Visitors.ThreeCode> program) {
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
            SimpleLang.Block.Block bl = new SimpleLang.Block.Block(visit);
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
