using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    public class Opt7Visitor : AutoApplyVisitorInterface {
        public override void VisitLogicOpNode (LogicOpNode lop) 
        {
            if (lop.Left is ExprNode)
                lop.Left.Visit(this); 

            if (lop.Right is ExprNode)
                lop.Right.Visit(this);

            if (lop.Left is IntNumNode l && lop.Right is IntNumNode r && (lop.Operation == "==")) {
                bool nbool = l.Num == r.Num;
                if (lop.Parent is IfNode ifn) {
                    ifn.Cond = new BooleanNode(nbool);
                    SetApply();
                } else
                    if (lop.Parent is WhileNode w) {
                    w.Expr = new BooleanNode(nbool);
                    SetApply();
                    } else
                        ReplaceExpr(lop, new BooleanNode(nbool));
            } else {
                base.VisitLogicOpNode(lop);
            }
        }
    }

    public class Opt12Visitor : AutoApplyVisitorInterface {
        public override void VisitIfNode(IfNode ifn) {
            if (ifn.Cond is BooleanNode bn && bn.Val == false) {
                if (ifn.Else != null)
                    ifn.Else.Visit(this);
                SetApply();
                ReplaceStat(ifn, ifn.Else);
            } else
                base.VisitIfNode(ifn);
        }
    }
}
