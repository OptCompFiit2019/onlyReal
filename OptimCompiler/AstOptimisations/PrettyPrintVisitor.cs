using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;
using SimpleLang.Visitors;

namespace SimpleLang.Optimisations
{
    /// <summary>
    /// Производит форматированный вывод на экран синтаксического дерева
    /// </summary>
    class PrettyPrintVisitor: Visitor
    {
        public string Text = "";
        private int Indent = 0;

        public PrettyPrintVisitor() { }

        private string IndentStr()
        {
            return new string(' ', Indent);
        }
        private void IndentPlus()
        {
            Indent += 2;
        }
        private void IndentMinus()
        {
            Indent -= 2;
        }
        public override void VisitIdNode(IdNode id)
        {
            Text += id.Name;
        }
        public override void VisitIntNumNode(IntNumNode num)
        {
            Text += num.Num.ToString();
        }
        public override void VisitBooleanNode(BooleanNode node)
        {
            Text += node.Val.ToString().ToLower();
        }

        public override void VisitBinOpNode(BinOpNode binop)
        {
            //Text += "(";
            binop.Left.Visit(this);
            Text += " " + binop.Op + " ";
            binop.Right.Visit(this);
            //Text += ")";
        }
        public override void VisitAssignNode(AssignNode a)
        {
            Text += IndentStr();
            a.Id.Visit(this);
            Text += " = ";
            a.Expr.Visit(this);
        }
        public override void VisitWhileNode(WhileNode c)
        {
            Text += IndentStr() + "while (";
            c.Expr.Visit(this);
            Text += ")" + Environment.NewLine;
            c.Stat.Visit(this);
        }

        public override void VisitForNode(ForNode c)
        {
            Text += IndentStr() + "for (";
            c.Start.Visit(this);
            Text += " to ";
            c.End.Visit(this);
            Text += ")" + Environment.NewLine;
            c.Stat.Visit(this);
        }

        public override void VisitLogicOpNode(LogicOpNode lop)
        {
            lop.Left.Visit(this);
            Text += lop.Operation;
            lop.Right.Visit(this);
        }

        public override void VisitIfNode(IfNode c)
        {
            Text += IndentStr() + "if (";
            c.Cond.Visit(this);
            Text += ")" + Environment.NewLine;
            c.If.Visit(this);
            if (c.Else != null)
            {
                Text += "else";
                c.Else.Visit(this);
            }
        }

        public override void VisitBlockNode(BlockNode bl)
        {
            Text += IndentStr() + "{" + Environment.NewLine;
            IndentPlus();

            var Count = bl.StList.Count;

            if (Count > 0)
                bl.StList[0]?.Visit(this);
            for (var i = 1; i < Count; i++)
            {
                if (bl.StList[i] == null) continue;
                Text += ';';
                if (!(bl.StList[i] is EmptyNode))
                    Text += Environment.NewLine;
                bl.StList[i].Visit(this);
            }
            IndentMinus();
            Text += Environment.NewLine + IndentStr() + "}";
        }

        public override void VisitPrintlnNode(PrintlnNode w)
        {
            Text += IndentStr() + "println(";
            w.Expr.Visit(this);
            Text += ")\n";
        }

        public override void VisitVarDefNode(VarDefNode w)
        {
            Text += IndentStr() + "var " + w.vars[0].Name;
            for (int i = 1; i < w.vars.Count; i++)
                Text += ',' + w.vars[i].Name;
        }

        public override void VisitDoubleNumNode(DoubleNumNode dnum)
        {
            Text += dnum.Num;
        }

        public override void VisitLogicIdNode(LogicIdNode lid)
        {
            Text += lid.Name;
        }

        public override void VisitLogicNotNode(LogicNotNode lnot)
        {
            lnot.LogExpr.Visit(this);
        }

        public override void VisitCycleNode(CycleNode c) { }

        public override void VisitEmptyNode(EmptyNode w) { }
    }
}
