using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProgramTree;

namespace SimpleLang.Visitors
{
    class MultiplicationComputeVisitor : ChangeVisitor
    {
        public override void VisitBinOpNode(BinOpNode binop)
        {
            if (binop.Left is ExprNode el)
                el.Visit(this);
            if (binop.Right is ExprNode er)
                er.Visit(this);
            if (binop.Left is IntNumNode l && binop.Right is IntNumNode r && binop.Op == "*")
                ReplaceExpr(binop, new IntNumNode(l.Num * r.Num));
            else
                base.VisitBinOpNode(binop);
        }
    }
}
