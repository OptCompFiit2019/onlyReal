using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    class FillParentVisitor : AutoVisitor
    {
        Stack<Node> st = new Stack<Node>(); // можно заменить на List
        public FillParentVisitor()
        {
            st.Push(null);
        }
        public override void VisitBinOpNode(BinOpNode binop)
        {
            binop.Parent = st.Peek();
            st.Push(binop);
            base.VisitBinOpNode(binop);
            st.Pop();
        }
        public override void VisitAssignNode(AssignNode a)
        {
            a.Parent = st.Peek();
            st.Push(a);
            base.VisitAssignNode(a);
            st.Pop();
        }
        public override void VisitIfNode(IfNode ifn)
        {
            ifn.Parent = st.Peek();
            st.Push(ifn);
            base.VisitIfNode(ifn);
            st.Pop();
        }

        public override void VisitWhileNode(WhileNode w)
        {
            w.Parent = st.Peek();
            st.Push(w);
            base.VisitWhileNode(w);
            st.Pop();
        }
    }

    class ChangeVisitor : AutoVisitor
    {
        public void ReplaceExpr(ExprNode from, ExprNode to)
        {
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

        public void ReplaceStat(StatementNode from, StatementNode to)
        {
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
                if (ifn.StatIf == from) // Поиск подузла в Parent
                    ifn.StatIf = to;
                else if (ifn.StatElse == from)
                    ifn.StatElse = to;
            }
        }
    }

    class Opt1Visitor : ChangeVisitor
    {
        public override void VisitBinOpNode(BinOpNode binop)
        {
            if ((binop.Left is IdNode) && (binop.Right is IdNode) &&
                (binop.Left as IdNode).Name == (binop.Right as IdNode).Name &&
                (binop.Op == "==" || binop.Op == ">="))
            {
                if (binop.Parent is IfNode ifn)
                    ifn.Expr = new BoolNode(true);
                else if (binop.Parent is WhileNode w)
                    w.Expr = new BoolNode(true);
                else
                    ReplaceExpr(binop, new BoolNode(true));
            }
            else if ((binop.Left is ExprNode) && (binop.Right is ExprNode) &&
                     (binop.Left.ToString() == binop.Right.ToString()) &&
                     (binop.Op == "==" || binop.Op == ">="))
            {
                if (binop.Parent is IfNode ifn)
                    ifn.Expr = new BoolNode(true);
                else if (binop.Parent is WhileNode w)
                    w.Expr = new BoolNode(true);
                else
                    ReplaceExpr(binop, new BoolNode(true));
            }
            else
            {
                base.VisitBinOpNode(binop); // Обойти потомков обычным образом
            }
        }
        public override void VisitIfNode(IfNode ifn)
        {
            ifn.Expr.Visit(this);
        }
        public override void VisitWhileNode(WhileNode w)
        {
            w.Expr.Visit(this);
        }
    }
    class Opt2Visitor : ChangeVisitor
    {
        public override void VisitBlockNode(BlockNode bl)
        {
            for (int i = 0; i < bl.StList.Count; i++)
                if (bl.StList[i] is IfNode ifn)
                {
                    var stlist1 = ifn.StatIf as BlockNode;
                    var stlist2 = ifn.StatElse as BlockNode;
                    bool null1, null2;
                    null1 = null2 = false;
                    if (stlist1.StList.Count == 1 & stlist1.StList[0] is NullNode)
                        null1 = true;
                    if (stlist2.StList.Count == 1 & stlist2.StList[0] is NullNode)
                        null2 = true;

                    if (null1 && null2)
                        bl.StList[i] = new NullNode();
                    else
                        base.VisitIfNode(ifn);
                }
        }
    }
}