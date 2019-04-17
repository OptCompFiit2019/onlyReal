using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
	class CycleNestedToIfVisitor : AutoVisitor
	{
		public bool HasCycleNestedToIf = false;
		private bool WithinIf = false;
		public override void VisitIfNode(IfNode ifn)
		{
			WithinIf = true;
			base.VisitIfNode(ifn);
			WithinIf = false;
		}
		public override void VisitCycleNode(CycleNode c)
		{
			if (WithinIf)
				HasCycleNestedToIf = true;
			base.VisitCycleNode(c);
		}
		public override void VisitWhileNode(WhileNode w)
		{
			if (WithinIf)
				HasCycleNestedToIf = true;
			base.VisitWhileNode(w);
		}
		public override void VisitForNode(ForNode f)
		{
			if (WithinIf)
				HasCycleNestedToIf = true;
			base.VisitForNode(f);
		}
		public override void VisitPrintlnNode(PrintlnNode w)
		{
		}
		public override void VisitVarDefNode(VarDefNode w)
		{
		}
	}
}