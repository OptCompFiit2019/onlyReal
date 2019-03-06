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
			base.VisitWhileNode(w);
		}
		public override void VisitForNode(ForNode f)
		{
			base.VisitForNode(f);
		}
		public override void VisitWriteNode(WriteNode w)
		{
		}
		public override void VisitVarDefNode(VarDefNode w)
		{
		}
	}
}