using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{

    public class Opt1Visitor : AutoApplyVisitorInterface
    {
        public override void VisitBinOpNode(BinOpNode binop)
        {
            if ((binop.Left is IdNode) && (binop.Right is IdNode) &&
                (binop.Left as IdNode).Name == (binop.Right as IdNode).Name &&
                (binop.Op == "==" || binop.Op == ">="))
            {
                if (binop.Parent is IfNode ifn) {
                    ifn.Cond = new BooleanNode(true);
                    SetApply();
                } else if (binop.Parent is WhileNode w) {
                    SetApply();
                    w.Expr = new BooleanNode(true);
                }
                else
                    ReplaceExpr(binop, new BooleanNode(true));
            }
            else if ((binop.Left is ExprNode) && (binop.Right is ExprNode) &&
                     (binop.Left.ToString() == binop.Right.ToString()) &&
                     (binop.Op == "==" || binop.Op == ">="))
            {
                if (binop.Parent is IfNode ifn) { 
                    ifn.Cond = new BooleanNode(true);
                    SetApply(); ;
                } else if (binop.Parent is WhileNode w) { 
                    w.Expr = new BooleanNode(true);
                    SetApply();
                } else
                    ReplaceExpr(binop, new BooleanNode(true));
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
    }
    public class Opt2v2Visitor : AutoApplyVisitorInterface
    {
        public override void VisitBlockNode(BlockNode bl)
        {
            for (int i = 0; i < bl.StList.Count; i++)
                if (bl.StList[i] is IfNode ifn)
                {
                    var stlist1 = ifn.If as BlockNode;
                    var stlist2 = ifn.Else as BlockNode;
                    bool null1, null2;
                    null1 = null2 = false;
                    if (stlist1.StList.Count == 1 & stlist1.StList[0] is EmptyNode)
                        null1 = true;
                    if (stlist2.StList.Count == 1 & stlist2.StList[0] is EmptyNode)
                        null2 = true;

                    if (null1 && null2) { 
                        bl.StList[i] = new EmptyNode();
                        SetApply();
                    } else
                        base.VisitIfNode(ifn);
                }
        }
    }
}