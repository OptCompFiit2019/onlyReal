using System;
using System.Collections.Generic;
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
        public bool Applyed() {
            return b;
        }
    }
}
