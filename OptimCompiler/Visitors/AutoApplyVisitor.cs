using System;
using System.Collections.Generic;
namespace SimpleLang.Visitors
{
    public class AutoApplyVisitor
    {
        public AutoApplyVisitor() {}
        private LinkedList<AutoApplyVisitorInterface> _visitors = new LinkedList<AutoApplyVisitorInterface>();
        public void Add (AutoApplyVisitorInterface change) {
            _visitors.AddLast(change);
        }

        public void Apply(ProgramTree.BlockNode root) {
            bool need = true;
            while (need) {
                need = false;
                foreach(AutoApplyVisitorInterface visit in _visitors) {
                    visit.Reset();
                    root.Visit(visit);
                    if (visit.Applyed()) {
                        need = true;
                        break;
                    }
                }
            }
        }
    }
}
