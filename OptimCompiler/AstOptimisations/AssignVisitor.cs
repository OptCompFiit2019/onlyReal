using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    class AssignVisitor: ChangeVisitor
    {
        public override void VisitAssignNode(AssignNode a)
        {
            if ((a.Expr is IdNode) && String.Equals(a.Id.Name, (a.Expr as IdNode).Name))
            {
                ReplaceStat(a, null);
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
