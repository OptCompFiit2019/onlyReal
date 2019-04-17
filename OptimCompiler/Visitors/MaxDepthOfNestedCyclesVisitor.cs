using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    class MaxDepthOfNestedCyclesVisitor : AutoVisitor
    {
        public int Max = 0;
        private int Current = 0;
        public override void VisitCycleNode(CycleNode c)
        {
            ++Current;
            base.VisitCycleNode(c);

            if (Current > Max)
                Max = Current;
            --Current;
        }
        public override void VisitWhileNode(WhileNode w)
        {
            ++Current;
            base.VisitWhileNode(w);

            if (Current > Max)
                Max = Current;
            --Current;
        }
        public override void VisitForNode(ForNode f)
        {
            ++Current;
            base.VisitForNode(f);

            if (Current > Max)
                Max = Current;
            --Current;
        }
        public override void VisitPrintlnNode(PrintlnNode w)
        {
        }
        public override void VisitVarDefNode(VarDefNode w)
        {
        }
    }
}
