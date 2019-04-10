using System;
using System.Collections.Generic;
using ProgramTree;

namespace SimpleLang.Visitors {
    //Operations

    public enum ThreeOperator {
        None, Assign, Minus, Plus, Mult, Div, Goto, IfGoto,
        Logic_or, Logic_and, Logic_less, Logic_equal, Logic_greater, Logic_geq, Logic_leq,
        Logic_not, Logic_neq
    };


    // Value Types
    public class ThreeAddressValueType {
        public override string ToString() => "UNKNOW TYPE";
    }

    public class ThreeAddressStringValue : ThreeAddressValueType {
        public string Value { get; set; }
        public ThreeAddressStringValue(string val = "") { Value = val; }
        public override string ToString() => Value;
        public override bool Equals(object obj) {
            if (obj is ThreeAddressStringValue val)
                return Value.Equals(val.Value);
            return false;
        }
        public override int GetHashCode() => Value.GetHashCode();
    }
    public class ThreeAddressIntValue : ThreeAddressValueType {
        public int Value { get; set; }
        public ThreeAddressIntValue(int val = 0) { Value = val; }
        public override string ToString() => Value.ToString();
        public override bool Equals(object obj) {
            if (obj is ThreeAddressIntValue val)
                return Value == val.Value;
            return false;
        }
        public override int GetHashCode() => Value.GetHashCode();
    }
    public class ThreeAddressLogicValue : ThreeAddressValueType {
        public bool Value { get; set; }
        public ThreeAddressLogicValue(bool val = false) { Value = val; }
        public override string ToString() => Value.ToString();
        public override bool Equals(object obj) {
            if (obj is ThreeAddressLogicValue val)
                return Value == val.Value;
            return false;
        }
        public override int GetHashCode() => Value.GetHashCode();
    }
    public class ThreeAddressDoubleValue : ThreeAddressValueType {
        public double Value { get; set; }
        public ThreeAddressDoubleValue(double val = 0) { Value = val; }
        public override string ToString() => Value.ToString();
        public override bool Equals(object obj) {
            if (obj is ThreeAddressDoubleValue val)
                return (Math.Abs(Value - val.Value)) < 0.000001;
            return false;
        }
        public override int GetHashCode() => Value.GetHashCode();
    }

    public class ThreeCode {
        public string label;
        public ThreeOperator operation = ThreeOperator.None;
        public string result;
        public ThreeAddressValueType arg1;
        public ThreeAddressValueType arg2;

        public ThreeCode(string l, string res, ThreeOperator op, ThreeAddressValueType a1, ThreeAddressValueType a2) {
            label = l;
            result = res;
            arg1 = a1;
            arg2 = a2;
            operation = op;
        }
        public ThreeCode(string res, ThreeOperator op, ThreeAddressValueType a1, ThreeAddressValueType a2 = null) {
            label = "";
            result = res;
            arg1 = a1;
            arg2 = a2;
            operation = op;
        }
        public ThreeCode(string res, ThreeAddressValueType a1) {
            label = "";
            result = res;
            arg1 = a1;
            arg2 = null;
            operation = ThreeOperator.Assign;
        }

        public static string GetOperatorString(ThreeOperator op) {
            switch (op) {
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
                case ThreeOperator.Logic_greater:
                    return ">";
                case ThreeOperator.Logic_geq:
                    return ">=";
                case ThreeOperator.Logic_leq:
                    return "<=";
                case ThreeOperator.Logic_neq:
                    return "!=";
                case ThreeOperator.Logic_not:
                    return "!";
            }
            return "UNKNOWN";
        }
        public static ThreeOperator ParseOperator(char c) => ParseOperator(c.ToString());
        public static ThreeOperator ParseOperator(string s) {
            switch (s) {
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
                case "<":
                    return ThreeOperator.Logic_less;
                case ">":
                    return ThreeOperator.Logic_greater;
                case ">=":
                    return ThreeOperator.Logic_geq;
                case "<=":
                    return ThreeOperator.Logic_leq;
                case "!=":
                    return ThreeOperator.Logic_neq;
                case "!":
                    return ThreeOperator.Logic_not;
            }
            return ThreeOperator.None;
        }

        public override string ToString() {
            string res = "";
            string lbl = "";
            if (label.Length > 0)
                lbl = label + ":";
            res += $"{lbl,-11}";

            if (operation == ThreeOperator.None)
                return res;

            if (operation == ThreeOperator.Goto)
                return res + "goto " + arg1.ToString();

            if (operation == ThreeOperator.IfGoto) {
                res += "if " + arg1.ToString() + " goto " + arg2.ToString();
                return res;
            }

            res += result + " = ";
            if (operation == ThreeOperator.Logic_not)
                return res + "!" + arg1.ToString();
            if (operation == ThreeOperator.Assign)
                res += arg1.ToString();
            else
                res += arg1.ToString() + " " + ThreeCode.GetOperatorString(operation) + " " + arg2.ToString();

            return res;
        }
    }


    //Visitor
    public class ThreeAddressCodeVisitor : Visitor {
        LinkedList<ThreeCode> program = new LinkedList<ThreeCode>();

        public LinkedList<ThreeCode> GetCode() {
            if (currentLabel.Length > 0) {
                program.AddLast(new ThreeCode(currentLabel, "", ThreeOperator.None, null, null));
                currentLabel = "";
            }
            return program;
        }

        private string currentLabel = "";
        private void AddCode(ThreeCode c) {
            if (currentLabel.Length == 0) {
                program.AddLast(c);
                return;
            }
            if (c.label.Length != 0 && currentLabel != c.label)
                throw new Exception("Core error");
            c.label = currentLabel;
            currentLabel = "";
            program.AddLast(c);
        }
        private int currentTempVarIndex = 0;
        private int currentLabelIndex = 0;
        public override string ToString() {
            string res = "";

            foreach (ThreeCode it in program)
                res += it.ToString() + "\n";

            if (currentLabel.Length > 0)
                res += currentLabel + ":\n";
            return res;
        }

        public static string ToString(LinkedList<ThreeCode> code) {
            string res = "";

            foreach (ThreeCode it in code)
                res += it.ToString() + "\n";

            return res;
        }
        public static string ToString(List<LinkedList<ThreeCode>> code) {
            string res = "";

            foreach (LinkedList<ThreeCode> it in code)
                res += ToString(it) + "\n";

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
        public override void VisitBooleanNode(BooleanNode lnum) {
            throw new Exception("Logic error");
        }
        public override void VisitLogicIdNode(LogicIdNode lnum) {
            throw new Exception("Logic error");
        }
        public override void VisitLogicOpNode(LogicOpNode lop) {
            throw new Exception("Logic error");
        }

        public override void VisitPrintlnNode(PrintlnNode w) {
            throw new Exception("Is not supported");
        }
        public override void VisitVarDefNode(VarDefNode w) {
            // ToDo В трехадресном коде нет операции создания переменной
        }


        public override void VisitAssignNode(AssignNode a) {
            AddCode(new ThreeCode(a.Id.ToString(), GenVariable(a.Expr)));
        }

        public override void VisitWhileNode(WhileNode w) {
            string expr = GenTempVariable();
            string label_start = currentLabel.Length > 0 ? currentLabel : GenLabel();

            currentLabel = label_start;
            ThreeAddressValueType val = GenVariable(w.Expr);

            AddCode(new ThreeCode(expr, ThreeOperator.Assign, val, null));
            string label_middle = GenLabel();
            string label_end = GenLabel();
            AddCode(new ThreeCode("", ThreeOperator.IfGoto, new ThreeAddressStringValue(expr), new ThreeAddressStringValue(label_middle)));
            AddCode(new ThreeCode("", ThreeOperator.Goto, new ThreeAddressStringValue(label_end), null));

            currentLabel = label_middle;
            w.Stat.Visit(this);

            AddCode(new ThreeCode("", ThreeOperator.Goto, new ThreeAddressStringValue(label_start), null));
            currentLabel = label_end;
        }
        public override void VisitForNode(ForNode f) {
            ThreeAddressValueType expr = GenVariable(f.Start);
            AddCode(new ThreeCode(f.Id.Name, expr));
            string label_start = GenLabel();
            string label_middle = GenLabel();
            string label_end = GenLabel();
            string b = GenTempVariable();

            currentLabel = label_start;
            ThreeAddressValueType val = GenVariable(f.End);

            AddCode(new ThreeCode(b, ThreeOperator.Logic_less, new ThreeAddressStringValue(f.Id.Name), val));
            AddCode(new ThreeCode("", ThreeOperator.IfGoto, new ThreeAddressStringValue(b), new ThreeAddressStringValue(label_middle)));
            AddCode(new ThreeCode("", ThreeOperator.Goto, new ThreeAddressStringValue(label_end), null));

            currentLabel = label_middle;
            f.Stat.Visit(this);

            AddCode(new ThreeCode(f.Id.Name, ThreeOperator.Plus, new ThreeAddressStringValue(f.Id.Name), new ThreeAddressIntValue(1)));
            AddCode(new ThreeCode("", ThreeOperator.Goto, new ThreeAddressStringValue(label_start), null));
            currentLabel = label_end;
        }

        public override void VisitIfNode(IfNode ifn) {
            string label_true = GenLabel();
            string label_end = GenLabel();
            ThreeAddressValueType expr = GenVariable(ifn.Cond);
            AddCode(new ThreeCode("", ThreeOperator.IfGoto, expr, new ThreeAddressStringValue(label_true)));
            if (ifn.Else != null)
                ifn.Else.Visit(this);

            AddCode(new ThreeCode("", ThreeOperator.Goto, new ThreeAddressStringValue(label_end), null));

            currentLabel = label_true;
            ifn.If.Visit(this);

            currentLabel = label_end;
        }

        public override void VisitBlockNode(BlockNode bl) {
            foreach (var st in bl.StList)
                st.Visit(this);
        }



        private ThreeAddressValueType GenVariable(ExprNode expr) {
            if (expr is IdNode)
                return new ThreeAddressStringValue((expr as IdNode).Name);

            if (expr is DoubleNumNode)
                return new ThreeAddressDoubleValue((expr as DoubleNumNode).Num);
            if (expr is IntNumNode)
                return new ThreeAddressIntValue((expr as IntNumNode).Num);

            if (expr is BinOpNode) {
                BinOpNode op = expr as BinOpNode;
                string res = GenTempVariable();
                ThreeAddressValueType arg1 = GenVariable(op.Left);
                ThreeAddressValueType arg2 = GenVariable(op.Right);
                ThreeOperator p = ThreeCode.ParseOperator(op.Op);
                AddCode(new ThreeCode(res, p, arg1, arg2));
                return new ThreeAddressStringValue(res);
            }

            throw new Exception("UNKNOW VALUE. Send autors of ThreeAddressCode");
        }
        private ThreeAddressValueType GenVariable(LogicExprNode expr) {
            if (expr is BooleanNode)
                return new ThreeAddressLogicValue((expr as BooleanNode).Val);
            if (expr is LogicIdNode)
                return new ThreeAddressStringValue((expr as LogicIdNode).Name.Name);

            if (expr is LogicOpNode) {
                LogicOpNode op = expr as LogicOpNode;
                string res = GenTempVariable();
                ThreeAddressValueType arg1 = GenVariable(op.Left);
                ThreeAddressValueType arg2 = GenVariable(op.Right);
                ThreeOperator p = ThreeCode.ParseOperator(op.Operation);
                AddCode(new ThreeCode(res, p, arg1, arg2));
                return new ThreeAddressStringValue(res);
            }

            if (expr is LogicNotNode) {
                LogicNotNode lnot = expr as LogicNotNode;
                string res = GenTempVariable();
                ThreeAddressValueType arg1 = GenVariable(lnot.LogExpr);
                AddCode(new ThreeCode(res, ThreeOperator.Logic_not, arg1));
                return new ThreeAddressStringValue(res);
            }

            throw new Exception("UNKNOW VALUE. Send autors of ThreeAddressCode");
        }

        private string GenTempVariable() {
            string res = "temp_" + currentTempVarIndex.ToString();
            currentTempVarIndex++;
            return res;
        }

        private string GenLabel() {
            string res = "label_" + currentLabelIndex.ToString();
            currentLabelIndex++;
            return res;
        }

    }
}
