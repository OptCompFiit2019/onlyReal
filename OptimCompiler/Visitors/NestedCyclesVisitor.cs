using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
	class NestedCyclesVisitor : AutoVisitor
	{
		public bool HasNestedCycles = false;
		private int CycleDepth = 0;
		public override void VisitCycleNode(CycleNode c)
		{
			CycleDepth += 1;
			base.VisitCycleNode(c);

			if (CycleDepth > 1)
				HasNestedCycles = true;
			CycleDepth = 0;
		}
		public override void VisitWhileNode(WhileNode w)
		{
			CycleDepth += 1;
			base.VisitWhileNode(w);

			if (CycleDepth > 1)
				HasNestedCycles = true;
		}
		public override void VisitForNode(ForNode f)
		{
			CycleDepth += 1;
			base.VisitForNode(f);

			if (CycleDepth > 1)
				HasNestedCycles = true;
		}
		public override void VisitPrintlnNode(PrintlnNode w)
		{
		}
		public override void VisitVarDefNode(VarDefNode w)
		{
		}
	}
}