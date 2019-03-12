using System;
using System.Collections.Generic;
using ProgramTree;

namespace SimpleLang.Visitors
{
    public enum ThreeOperator {  None, Assign, Minus, Plus, Mult, Div, Goto, IfGoto, Logic_or, Logic_and, Logic_less, Logic_equal };
    public class ThreeCode
    {
        public string label;
        public ThreeOperator operation = ThreeOperator.None;
        public string result;
        public string arg1;
        public string arg2;

        public ThreeCode(string l, string res, ThreeOperator op, string a1, string a2){
            label = l;
            result = res;
            arg1 = a1;
            arg2 = a2;
            operation = op;
        }
        public ThreeCode(string res, ThreeOperator op, string a1, string a2) { 
            label = "";
            result = res;
            arg1 = a1;
            arg2 = a2;
            operation = op;
        }
        public ThreeCode(string res, string a1)
        {
            label = "";
            result = res;
            arg1 = a1;
            arg2 = "";
            operation = ThreeOperator.Assign;
        }

        public static string GetOperatorString(ThreeOperator op) {
            switch (op)
            {
                case ThreeOperator.Assign:
                    return "=";
                case ThreeOperator.Div:
                    return "/";
                case ThreeOperator.Minus:
                    return "-";
                case ThreeOperator.Mult:
                    return "*";
                case ThreeOperator.Plus:
                    return "+";
                case ThreeOperator.Logic_and:
                    return "&&";
                case ThreeOperator.Logic_less:
                    return "<";
                case ThreeOperator.Logic_or:
                    return "||";
                case ThreeOperator.Logic_equal:
                    return "==";
            }
            return "UNKNOW";
        }
        public static ThreeOperator ParseOperator(char c) => ParseOperator(c.ToString());
        public static ThreeOperator ParseOperator(string s)
        {
            switch (s)
            {
                case "=":
                    return ThreeOperator.Assign;
                case "&&":
                    return ThreeOperator.Logic_and;
                case "||":
                    return ThreeOperator.Logic_or;
                case "/":
                    return ThreeOperator.Div;
                case "-":
                    return ThreeOperator.Minus;
                case "+":
                    return ThreeOperator.Plus;
                case "*":
                    return ThreeOperator.Mult;
                case "==":
                    return ThreeOperator.Logic_equal;
            }
            return ThreeOperator.None;
        }

        public override string ToString()
        {
            string res = "";
            if (label.Length > 0)
                res = label + ":";
            res += "\t";

            if (operation == ThreeOperator.Goto) {
                return res + "goto " + arg1;
            }
            if (operation == ThreeOperator.IfGoto) {
                res += "if " + arg1 + " goto " + arg2;
                return res;
            }

            res += result + " = " + arg1;
            if (operation != ThreeOperator.Assign)
                res += ThreeCode.GetOperatorString(operation) + arg2;

            return res;
        }
    }
    public class ThreeAddressCodeVisitor : Visitor
    {
        /*private int currentStep = 0;
        private string CurrentTab() {
            return new string('\t', currentStep);
        }
        private void AddStep() { currentStep++;  }
        private void SubStep() { currentStep--;  }*/

        List<ThreeCode> program = new List<ThreeCode>();
        private string currentLabel = "";
        private void AddCode(ThreeCode c) {
            if (currentLabel.Length == 0 || currentLabel.Equals("")) {
                program.Add(c);
                return;
            }
            if (c.label.Length != 0 && (!currentLabel.Equals(c.label)))
                throw new Exception("Core error");
            c.label = currentLabel;
            currentLabel = "";
            program.Add(c);
        }
        private int currentTempVarIndex = 0;
        private int currentLabelIndex = 0;
        public override string ToString() {
            string res = "";

            foreach (ThreeCode it in program)
                res += it.ToString() + "\n";

            return res;
        }

        public override void VisitIdNode(IdNode id) {
            throw new Exception("Logic error");
        }

        public override void VisitIntNumNode(IntNumNode num) {
            throw new Exception("Logic error");
        }

        public override void VisitBinOpNode(BinOpNode binop) {
            throw new Exception("Logic error");
        }

        public override void VisitDoubleNumNode(DoubleNumNode dnum) {
            throw new Exception("Logic error");
        }
        public override void VisitLogicNumNode(BooleanNode lnum) {
            throw new Exception("Logic error");
        }
        public override void VisitLogicIdNode(LogicIdNode lnum) {
            throw new Exception("Logic error");
        }
        public override void VisitLogicOperationNode(LogicOperationNode lop) {
            throw new Exception("Logic error");
        }

        public override void VisitPrintlnNode(PrintlnNode w) {
            throw new Exception("Is not supported");
        }
        public override void VisitVarDefNode(VarDefNode w) {
            // В трехадресном коде нет операции создания переменной
        }


        public override void VisitAssignNode(AssignNode a) {
            AddCode(new ThreeCode(a.Id.ToString(), GenVariable(a.Expr)));
        }

        public override void VisitWhileNode(WhileNode w) {
            string expr = GenTempVariable();
            string label_start = currentLabel.Length > 0 ? currentLabel : GenLabel();
            AddCode(new ThreeCode(label_start, expr, ThreeOperator.Assign, GenVariable(w.Expr), ""));
            string label_middle = GenLabel();
            string label_end = GenLabel();
            AddCode(new ThreeCode("", ThreeOperator.IfGoto, expr, label_middle));
            AddCode(new ThreeCode("", ThreeOperator.Goto, label_end, ""));

            currentLabel = label_middle;
            w.Stat.Visit(this);

            AddCode(new ThreeCode("", ThreeOperator.Goto, label_start, ""));
            currentLabel = label_end;
        }
        public override void VisitForNode(ForNode f) {
            string expr = GenVariable(f.StartValue);
            AddCode(new ThreeCode(f.Id.Name, expr));
            string label_start = GenLabel();
            string label_middle = GenLabel();
            string label_end = GenLabel();
            string b = GenTempVariable();

            currentLabel = label_start;
            string val = GenVariable(f.End);

            AddCode(new ThreeCode(b, ThreeOperator.Logic_less, f.Id.Name, val));
            AddCode(new ThreeCode("", ThreeOperator.IfGoto, b, label_middle));
            AddCode(new ThreeCode("", ThreeOperator.Goto, label_end, ""));

            currentLabel = label_middle;
            f.Stat.Visit(this);

            AddCode(new ThreeCode(f.Id.Name, ThreeOperator.Plus, f.Id.Name, "1"));
            AddCode(new ThreeCode("", ThreeOperator.Goto, label_start, ""));
            currentLabel = label_end;
        }

        public override void VisitIfNode(IfNode ifn) {
            string label_true = GenLabel();
            string label_false = GenLabel();
            string label_end = GenLabel();
            string expr = GenVariable(ifn.Expr);
            if (ifn.Else == null)
                label_false = label_end;
            AddCode(new ThreeCode("", ThreeOperator.IfGoto, expr, label_true));
            AddCode(new ThreeCode("", ThreeOperator.Goto, label_false, ""));

            currentLabel = label_true;
            ifn.If.Visit(this);
            AddCode(new ThreeCode("", ThreeOperator.Goto, label_end, ""));

            if (ifn.Else != null){
                currentLabel = label_false;
                ifn.Else.Visit(this);
            }
            currentLabel = label_end;
        }

        public override void VisitBlockNode(BlockNode bl) {
            foreach (var st in bl.StList)
                st.Visit(this);
        }

       


        private string GenVariable(ExprNode expr) {

            if (expr is IdNode)
                return (expr as IdNode).Name;

            if (expr is DoubleNumNode)
                return (expr as DoubleNumNode).Num.ToString();
            if (expr is IntNumNode)
                return (expr as IntNumNode).Num.ToString();

            if (expr is BinOpNode) {
                BinOpNode op = expr as BinOpNode;
                string res = GenTempVariable();
                string arg1 = GenVariable(op.Left);
                string arg2 = GenVariable(op.Right);
                ThreeOperator p = ThreeCode.ParseOperator(op.Op);
                AddCode(new ThreeCode(res, p, arg1, arg2));
                return res;
            }

            return "UNKNOW";
        }
        private string GenVariable(LogicExprNode expr)
        {
            if (expr is BooleanNode)
                return (expr as BooleanNode).Val.ToString();
            if (expr is LogicIdNode)
                return (expr as LogicIdNode).Name.Name;

            if (expr is LogicOperationNode)
            {
                LogicOperationNode op = expr as LogicOperationNode;
                string res = GenTempVariable();
                string arg1 = GenVariable(op.Left);
                string arg2 = GenVariable(op.Right);
                ThreeOperator p = ThreeCode.ParseOperator(op.Operation);
                AddCode(new ThreeCode(res, p, arg1, arg2));
                return res;
            }

            return "UNKNOW";
        }
        private string GenTempVariable(){
            string res = "temp_" + currentTempVarIndex.ToString();
            currentTempVarIndex++;
            return res;
        }
        private string GenLabel(){
            string res = "label_" + currentLabelIndex.ToString();
            currentLabelIndex++;
            return res;
        }

    }
}
