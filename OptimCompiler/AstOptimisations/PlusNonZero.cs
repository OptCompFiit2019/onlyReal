using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    class PlusNonZero : AutoApplyVisitorInterface
    {
        public override void VisitBinOpNode(BinOpNode binop)
        {
            if (binop.Left is IntNumNode && (binop.Left as IntNumNode).Num == 0 &&
            binop.Op[0] == '+')
            {
                binop.Right.Visit(this);
                ReplaceExpr(binop, binop.Right);
            }
            else
            {
                if (binop.Right is IntNumNode && (binop.Right as IntNumNode).Num == 0 &&
            binop.Op[0] == '+')
                {
                    binop.Left.Visit(this);
                    ReplaceExpr(binop, binop.Left);
                }
                else
                {
                    base.VisitBinOpNode(binop);
                }

            }
        }
    }
}
