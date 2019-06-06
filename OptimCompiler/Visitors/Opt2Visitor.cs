using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
	public class Opt2Visitor : AutoApplyVisitorInterface
    {
		public override void VisitBinOpNode(BinOpNode binop)
		{
			if (binop.Left is IntNumNode && (binop.Left as IntNumNode).Num == 0 &&
				binop.Op == "*")
			{
                SetApply();
				ReplaceExpr(binop, binop.Left); // Заменить себя на своё левое поддерево
			}
			else if (binop.Right is IntNumNode && (binop.Right as IntNumNode).Num == 0 &&
				binop.Op == "*")
			{
                SetApply();
				ReplaceExpr(binop, binop.Right); // Заменить себя на своё правое поддерево
			}
			else // Если оптимизаций нет, то
			{
				base.VisitBinOpNode(binop); // Обойти потомков обычным образом
			}
		}
	}
}
