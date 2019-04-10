using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using ProgramTree;
using SimpleLang.Visitors;

namespace SimpleLang.Optimisations
{
    class ChangeVisitor : AutoVisitor {
        public void ReplaceExpr(ExprNode from, ExprNode to) {
            var p = from.Parent;
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
        
        public void ReplaceStat(StatementNode from, StatementNode to) {
            var p = from.Parent;
            if (p is AssignNode || p is ExprNode) {
                throw new Exception("Родительский узел не содержит операторов");
            }
            to.Parent = p;
            if (p is BlockNode bln) // Можно переложить этот код на узлы!
            {
                for (var i=0; i<bln.StList.Count-1; i++)
                    if (bln.StList[i] == from) {
                        bln.StList[i] = to;
                        break;
                    }
            }
            else if (p is IfNode ifn) {
                if (ifn.If == from) // Поиск подузла в Parent
                    ifn.Else = to;
                else if (ifn.Else == from)
                    ifn.Else = to;
            }
        }
    }
    
    class OptSimilarDifference : ChangeVisitor
    {
        public override void VisitBinOpNode(BinOpNode binop)
        {
            if ((binop.Left is IdNode) && (binop.Right is IdNode) &&
                String.Equals((binop.Left as IdNode).Name, (binop.Right as IdNode).Name) &&
                (binop.Op == '-'))
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
    
    class OptSimilarAssignment : ChangeVisitor
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