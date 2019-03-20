using System;
namespace SimpleLang.ThreeOptimize
{
    public interface ThreeCodeApplyer
    {
        void Apply(System.Collections.Generic.LinkedList<Visitors.ThreeCode> program);
        bool Applyed();
    }
}
