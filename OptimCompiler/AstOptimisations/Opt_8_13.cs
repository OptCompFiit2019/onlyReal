using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    public class ChangeVisitor2 : AutoApplyVisitorInterface
    {
        public void ReplaceExpr2(ExprNode from, ExprNode to)
        {
            SetApply();
            var p = from.Parent;
            if (p == null)
                from = to;
            if (to.Parent != null)
                to.Parent = p;
            if (p is AssignNode assn)
            {
                assn.Expr = to;
            }
            else if (p is BinOpNode binopn)
            {
                if (binopn.Left == from) // Поиск подузла в Parent
                    binopn.Left = to;
                else if (binopn.Right == from)
                    binopn.Right = to;
            }
            else if (p is BlockNode)
            {
                throw new Exception("Родительский узел не содержит выражений");
            }
        }

        public override void ReplaceStat(StatementNode from, StatementNode to)
        {
            SetApply();
            var p = from.Parent;
            if (p is AssignNode || p is ExprNode)
            {
                //throw new Exception("Родительский узел не содержит операторов");

            }
            to.Parent = p;
            if (p is BlockNode bln) // Можно переложить этот код на узлы!
            {
                for (var i = 0; i < bln.StList.Count - 1; i++)
                    if (bln.StList[i] == from)
                        bln.StList[i] = to;
            }
            else if (p is IfNode ifn)
            {
                if (ifn.If == from) // Поиск подузла в Parent
                    ifn.If = to;
                else if (ifn.Else == from)
                    ifn.Else = to;
            }
        }
    }

    public class OptVisitor_8 : ChangeVisitor2
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
                    w.Expr = new BooleanNode(true);
                    SetApply();
                }else
                    ReplaceExpr2(binop, new BooleanNode(true));
            }
            else if ((binop.Left is ExprNode) && (binop.Right is ExprNode) &&
                     (binop.Left.ToString() == binop.Right.ToString()) &&
                     (binop.Op == "==" || binop.Op == ">="))
            {
                if (binop.Parent is IfNode ifn) { 
                    ifn.Cond = new BooleanNode(true);
                    SetApply();
                }else if (binop.Parent is WhileNode w) { 
                    w.Expr = new BooleanNode(true);
                    SetApply();
                }else
                    ReplaceExpr2(binop, new BooleanNode(true));
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
    public class OptVisitor_13 : ChangeVisitor2
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
                    if (stlist1.StList.Count == 1 & stlist1.StList[0] is NullNode)
                        null1 = true;
                    if (stlist2.StList.Count == 1 & stlist2.StList[0] is NullNode)
                        null2 = true;

                    if (null1 && null2) { 
                        bl.StList[i] = new NullNode();
                        SetApply();
                    }else
                        base.VisitIfNode(ifn);
                }
        }
    }
}