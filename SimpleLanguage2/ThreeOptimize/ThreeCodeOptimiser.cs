using System;
using System.Collections.Generic;
using SimpleLang.Visitors;

namespace SimpleLang.ThreeCodeOptimisations {

    public interface ThreeCodeOptimiser {
        void Apply(LinkedList<ThreeCode> program);
        bool Applyed();
    }
}
