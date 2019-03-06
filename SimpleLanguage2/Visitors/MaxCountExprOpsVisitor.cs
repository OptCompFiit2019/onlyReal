﻿using System;
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
		public override void VisitBinOpNode(BinOpNode bop)
		{
			Current += 1;
			base.VisitBinOpNode(bop);

			if (Current > Max)
				Max = Current;
			Current = 0;
		}
		public override void VisitWriteNode(WriteNode w)
		{
		}
		public override void VisitVarDefNode(VarDefNode w)
		{
		}
	}
}