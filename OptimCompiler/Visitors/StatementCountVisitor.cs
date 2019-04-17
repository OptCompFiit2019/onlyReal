using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
	class StatementCountVisitor : AutoVisitor
	{
		public int Count = 0;
		public override void VisitAssignNode(AssignNode a)
		{
			Count += 1;
		}
		public override void VisitCycleNode(CycleNode c)
		{
			Count += 1;
			base.VisitCycleNode(c);
		}
		public override void VisitIfNode(IfNode ifn)
		{
			Count += 1;
			base.VisitIfNode(ifn);
		}
		public override void VisitWhileNode(WhileNode w)
		{
			Count += 1;
			base.VisitWhileNode(w);
		}
		public override void VisitForNode(ForNode f)
		{
			Count += 1;
			base.VisitForNode(f);
		}
		/*public override void VisitBlockNode(BlockNode b)
		{
			Count += 1;
			base.VisitBlockNode(b);
		}*/
		public override void VisitPrintlnNode(PrintlnNode w)
		{
			Count += 1;
		}
		public override void VisitVarDefNode(VarDefNode w)
		{
			Count += 1;
		}
	}
}