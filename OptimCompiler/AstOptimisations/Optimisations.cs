using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using ProgramTree;
using SimpleLang.Visitors;

namespace SimpleLang.Optimisations
{
    public class OptSimilarDifference : AutoApplyVisitorInterface
    {
        public override void VisitBinOpNode(BinOpNode binop)
        {
            if ((binop.Left is IdNode) && (binop.Right is IdNode) &&
                String.Equals((binop.Left as IdNode).Name, (binop.Right as IdNode).Name) &&
                (binop.Op == "-"))
            {
                ReplaceExpr(binop, new IntNumNode(0));
            }
            else
            {
                base.VisitBinOpNode(binop); // Обойти потомков обычным образом
            }
        }
        public override void VisitIfNode(IfNode ifn)
        {
            ifn.Cond.Visit(this);
        }
        public override void VisitWhileNode(WhileNode w)
        {
            w.Expr.Visit(this);
        }

        public override string ToString()
        {
            return "";
        }
    }
    
    public class OptSimilarAssignment : AutoApplyVisitorInterface
    {
        public override void VisitAssignNode(AssignNode a)
        {
            if ((a.Expr is IdNode) && String.Equals(a.Id.Name, (a.Expr as IdNode).Name))
            {
                ReplaceStat(a, new EmptyNode());
            }
            else
            {
                base.VisitAssignNode(a);
            }
        }

        public override void VisitIfNode(IfNode ifn)
        {
            ifn.Cond.Visit(this);
        }
        public override void VisitWhileNode(WhileNode w)
        {
            w.Expr.Visit(this);
        }

        public override string ToString()
        {
            return "";
        }
    }
}