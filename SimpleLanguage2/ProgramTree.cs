using System.Collections.Generic;
using System.Globalization;
using SimpleLang.Visitors;

namespace ProgramTree
{
    public enum AssignType { Assign, AssignPlus, AssignMinus, AssignMult, AssignDivide };

    public abstract class Node // базовый класс для всех узлов    
    {
        public Node Parent;
        public abstract void Visit(Visitor v);
    }

    public abstract class ExprNode : Node // базовый класс для всех выражений
    {
    }

    public class IdNode : ExprNode
    {
        public string Name { get; set; }
        public IdNode(string name) { Name = name; }
        public override void Visit(Visitor v)
        {
            v.VisitIdNode(this);
        }
        public override string ToString() => Name;
    }
    public class DoubleNumNode : ExprNode
    {
        public double Num { get; set; }
        public DoubleNumNode(double num) { Num = num; }
        public override void Visit(Visitor v)
        {
			v.VisitDoubleNumNode(this);
        }
        public override string ToString() => Num.ToString(CultureInfo.InvariantCulture);

    }

    public class IntNumNode : ExprNode
    {
        public int Num { get; set; }
        public IntNumNode(int num) { Num = num; }
        public override void Visit(Visitor v)
        {
            v.VisitIntNumNode(this);
        }
        public override string ToString() => Num.ToString();
    }

    public class BinOpNode : ExprNode
    {
        public ExprNode Left { get; set; }
        public ExprNode Right { get; set; }
        public char Op { get; set; }
        public BinOpNode(ExprNode Left, ExprNode Right, char op) 
        {
            this.Left = Left;
            this.Right = Right;
            this.Op = op;
        }
        public override void Visit(Visitor v)
        {
            v.VisitBinOpNode(this);
        }
        public override string ToString()
        {
            return "(" + Left.ToString() + " " + Op + " " + Right.ToString() + ")";
        }
    }

    public abstract class StatementNode : Node // базовый класс для всех операторов
    {
    }

    public class AssignNode : StatementNode
    {
        public IdNode Id { get; set; }
        public ExprNode Expr { get; set; }
        public AssignType AssOp { get; set; }
        public AssignNode(IdNode id, ExprNode expr, AssignType assop = AssignType.Assign)
        {
            Id = id;
            Expr = expr;
            AssOp = assop;
        }
        public override void Visit(Visitor v)
        {
            v.VisitAssignNode(this);
        }
        public override string ToString()
        {
            return Id.ToString() + " = " + Expr.ToString() + ";";
        }
    }
    public abstract class LogicExprNode : Node { }
    public class BooleanNode : LogicExprNode
    {
        public bool Val { get; set; }
        public BooleanNode(bool val) { Val = val; }
        public override void Visit(Visitor v)
        {
			v.VisitBooleanNode(this);
        }
        public override string ToString()
        {
            return Val.ToString().ToLower();
        }
    }
    public class LogicIdNode : LogicExprNode
    {
        public IdNode Name { get; set; }
        public LogicIdNode(IdNode val) { Name = val; }
        public override void Visit(Visitor v)
        {
			v.VisitLogicIdNode(this);
        }
        public override string ToString()
        {
            return Name.ToString();
        }
    }

    public class LogicOpNode : LogicExprNode
    {
        public LogicExprNode Left { get; set; }
        public LogicExprNode Right { get; set; }
        public string Operation { get; set; }
        public LogicOpNode(LogicExprNode Left, LogicExprNode Right, string op)
        {
            this.Left = Left;
            this.Right = Right;
            Operation = op;
        }
        public override void Visit(Visitor v)
        {
			v.VisitLogicOpNode(this);
		}
        public override string ToString()
        {
            return "(" + Left.ToString() + " " + Operation + " " + Right.ToString() + ")";
        }
    }

    public class LogicNotNode : LogicExprNode
    {
        public LogicExprNode LogExpr { get; set; }
        public SimpleParser.Tokens Operation { get; set; }
        public LogicNotNode(LogicExprNode LogExpr)
        {
            this.LogExpr = LogExpr;
        }
        public override void Visit(Visitor v)
        {
            v.VisitLogicNotNode(this);
        }
        public override string ToString()
        {
            return "!" + LogExpr.ToString();
        }
    }

    public class CycleNode : StatementNode
    {
        public ExprNode Expr { get; set; }
        public StatementNode Stat { get; set; }
        public CycleNode(ExprNode expr, StatementNode stat)
        {
            Expr = expr;
            Stat = stat;
        }
        public override void Visit(Visitor v)
        {
            v.VisitCycleNode(this);
        }
    }

    public class IfNode : StatementNode
    {
        public LogicExprNode Cond { get; set; }
        public StatementNode If { get; set; }
        public StatementNode Else { get; set; }
        public IfNode(LogicExprNode Expr, StatementNode If, StatementNode Else = null)
        {
            this.Cond = Expr;
            this.If = If;
            this.Else = Else;
        }
        public override void Visit(Visitor v)
        {
			v.VisitIfNode(this);
		}
        public override string ToString()
        {
            string res = "if (" + Cond.ToString() + ")\n" + If.ToString();
            if (Else != null)
                res += "\nelse\n" + Else.ToString();
            return res;
        }
    }

    public class WhileNode : StatementNode
    {
        public LogicExprNode Expr { get; set; }
        public StatementNode Stat { get; set; }
        public WhileNode(LogicExprNode expr, StatementNode stat)
        {
            Expr = expr;
            Stat = stat;
        }
        public override void Visit(Visitor v)
        {
			v.VisitWhileNode(this);
		}
        public override string ToString()
        {
            return "while (" + Expr.ToString() + ")\n" + Stat.ToString();
        }
    }
    public class ForNode : StatementNode
    {
        public ExprNode Start { get; set; }
        public IdNode Id { get; set; }
        public ExprNode End { get; set; }
        public StatementNode Stat { get; set; }
        public ForNode(AssignNode start, ExprNode end, StatementNode stat)
        {
            Start = start.Expr;
            Id = start.Id;
            End = end;
            Stat = stat;
        }
        public override void Visit(Visitor v)
        {
			v.VisitForNode(this);
		}
        public override string ToString()
        {
            return "for (" + Id.ToString() + " = " + Start.ToString()
                + " to " + End.ToString() + ")\n" + Stat.ToString();
        }
    }

    public class BlockNode : StatementNode
    {
        public List<StatementNode> StList = new List<StatementNode>();
        public BlockNode(StatementNode stat)
        {
            Add(stat);
        }
        public BlockNode() { }
        public void Add(StatementNode stat)
        {
            StList.Add(stat);
        }
        public override void Visit(Visitor v)
        {
            v.VisitBlockNode(this);
        }
        public override string ToString()
        {
            string res = "{\n";
            foreach (var it in StList)
            {
                res += it.ToString() + "\n";
            }
            res += "}";
            return res;
        }
    }

    public class PrintlnNode : StatementNode
    {
        public ExprNode Expr { get; set; }
        public PrintlnNode(ExprNode Expr)
        {
            this.Expr = Expr;
        }
        public override void Visit(Visitor v)
        {
            v.VisitPrintlnNode(this);
        }
        public override string ToString() => $"println({Expr});";
    }

    public class EmptyNode : StatementNode
    {
        public override void Visit(Visitor v)
        {
            v.VisitEmptyNode(this);
        }
        public override string ToString() => "";
    }

    public class VarDefNode : StatementNode
    {
        public List<IdNode> vars = new List<IdNode>();
        public VarDefNode(IdNode id)
        {
            Add(id);
        }

        public void Add(IdNode id)
        {
            vars.Add(id);
        }
        public override void Visit(Visitor v)
        {
            v.VisitVarDefNode(this);
        }
        public override string ToString()
        {
            string s = "var " + vars[0].ToString();
            for (int i = 1; i < vars.Count; ++i)
                s += ", " + vars[i].ToString();
            s += ";";

            return s;            
        }
    }
}