using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
	class IfNestedToCycleVisitor : AutoVisitor
	{
		public bool HasIfNestedToCycle = false;
		private bool WithinCycle = false;
		public override void VisitIfNode(IfNode ifn)
		{
			if (WithinCycle)
				HasIfNestedToCycle = true;
			base.VisitIfNode(ifn);
		}
		public override void VisitCycleNode(CycleNode c)
		{
			WithinCycle = true;
			base.VisitCycleNode(c);
			WithinCycle = false;
		}
		public override void VisitWhileNode(WhileNode w)
		{
            WithinCycle = true;
            base.VisitWhileNode(w);
            WithinCycle = false;
        }
		public override void VisitForNode(ForNode f)
		{
            WithinCycle = true;
            base.VisitForNode(f);
            WithinCycle = false;
        }
		public override void VisitPrintlnNode(PrintlnNode w)
		{
		}
		public override void VisitVarDefNode(VarDefNode w)
		{
		}
	}
}