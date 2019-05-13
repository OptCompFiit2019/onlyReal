using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    class PrettyPrintVisitor: Visitor
    {
        public string Text = "";
        private int Indent = 0;

        private string IndentStr()
        {
            return new string(' ', Indent);
        }
        private void IndentPlus()
        {
            Indent += 4;
        }
        private void IndentMinus()
        {
            Indent -= 4;
        }
        public override void VisitIdNode(IdNode id) 
        {
            Text += id.Name;
        }
        public override void VisitIntNumNode(IntNumNode num) 
        {
            Text += num.Num.ToString();
        }
        public override void VisitBinOpNode(BinOpNode binop) 
        {
            Text += "(";
            binop.Left.Visit(this);
            Text += " " + binop.Op + " ";
            binop.Right.Visit(this);
            Text += ")";
        }
        public override void VisitAssignNode(AssignNode a) 
        {
            Text += IndentStr();
            a.Id.Visit(this);
            Text += " = ";
            a.Expr.Visit(this);
            Text += ";";
        }
        public override void VisitDoubleNumNode(DoubleNumNode dnum)
        {
            Text += dnum.Num.ToString(CultureInfo.InvariantCulture);
        }
        public override void VisitBooleanNode(BooleanNode lnum)
        {
            Text += lnum.Val.ToString().ToLower();
        }
        public override void VisitLogicIdNode(LogicIdNode lid)
        {
            Text += lid.Name;
        }
        public override void VisitLogicNotNode(LogicNotNode lnot)
        {
            Text += "!";
            lnot.LogExpr.Visit(this);
        }
        public override void VisitLogicOpNode(LogicOpNode lop)
        {
            Text += "(";
            lop.Left.Visit(this);
            Text += " " + lop.Operation + " ";
            lop.Right.Visit(this);
            Text += ")";
        }
        public override void VisitWhileNode(WhileNode w)
        {
            Text += IndentStr() + "while (";
            w.Expr.Visit(this);
            Text += ")\n";

            if (!(w.Stat is BlockNode))
                IndentPlus();
            w.Stat.Visit(this);
            if (!(w.Stat is BlockNode))
                IndentMinus();
        }
        public override void VisitForNode(ForNode f)
        {
            Text += IndentStr() + "for (" + f.Id.Name + " = ";
            f.Start.Visit(this);
            Text += " to ";
            f.End.Visit(this);
            Text += ")\n";

            if (!(f.Stat is BlockNode))
                IndentPlus();
            f.Stat.Visit(this);
            if (!(f.Stat is BlockNode))
                IndentMinus();
        }
        public override void VisitIfNode(IfNode ifn)
        {
            Text += IndentStr() + "if (";
            ifn.Cond.Visit(this);
            Text += ")\n";

            if (!(ifn.If is BlockNode))
                IndentPlus();
            ifn.If.Visit(this);
            if (!(ifn.If is BlockNode))
                IndentMinus();

            if (ifn.Else != null)
            {
                Text += "\n" + IndentStr() + "else\n";

                if (!(ifn.Else is BlockNode))
                    IndentPlus();
                ifn.Else.Visit(this);
                if (!(ifn.Else is BlockNode))
                    IndentMinus();
            }
        }
        public override void VisitCycleNode(CycleNode c) 
        {
            Text += IndentStr() + "cycle ";
            c.Expr.Visit(this);
            Text += Environment.NewLine;

            if (!(c.Stat is BlockNode))
                IndentPlus();
            c.Stat.Visit(this);
            if (!(c.Stat is BlockNode))
                IndentMinus();
        }
        public override void VisitBlockNode(BlockNode bl) 
        {
            Text += IndentStr() + "{" + Environment.NewLine;
            IndentPlus();

            var Count = bl.StList.Count;

            if (Count > 0)
                bl.StList[0].Visit(this);
            for (var i = 1; i < Count; i++)
            {
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
            Text += ");";
        }
        public override void VisitVarDefNode(VarDefNode w) 
        {
            Text += IndentStr() + "var " + w.vars[0].Name;
            for (int i = 1; i < w.vars.Count; i++)
                Text += ',' + w.vars[i].Name;
            Text += ";";
        }
    }
}
