using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
	class MaxCountExprOpsVisitor : AutoVisitor
	{
		public int Max = 0;
        private int Current = 0;
        public override void VisitAssignNode(AssignNode a)
        {
            a.Expr.Visit(this);

            if (Current > Max)
                Max = Current;
            Current = 0;
        }
        public override void VisitWhileNode(WhileNode w)
        {
            w.Expr.Visit(this);

            if (Current > Max)
                Max = Current;
            Current = 0;
        }
        public override void VisitIfNode(IfNode ifn)
        {
            ifn.Cond.Visit(this);

            if (Current > Max)
                Max = Current;
            Current = 0;
        }
        public override void VisitForNode(ForNode f)
        {
            f.End.Visit(this);

            if (Current > Max)
                Max = Current;
            Current = 0;
        }
        public override void VisitBinOpNode(BinOpNode bop)
		{
            ++Current;
            base.VisitBinOpNode(bop);
        }
        public override void VisitLogicOpNode(LogicOpNode lop)
        {
            ++Current;
            base.VisitLogicOpNode(lop);
        }
		public override void VisitLogicNotNode(LogicNotNode lnot)
		{
			++Current;
			base.VisitLogicNotNode(lnot);
		}
		public override void VisitPrintlnNode(PrintlnNode p)
		{
            base.VisitPrintlnNode(p);
            if (Current > Max)
                Max = Current;
            Current = 0;
        }
		public override void VisitVarDefNode(VarDefNode w)
		{
		}
	}
}
