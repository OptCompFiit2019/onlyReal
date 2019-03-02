using System.Collections.Generic;
using SimpleLang.Visitors;

namespace ProgramTree
{
    public enum AssignType { Assign, AssignPlus, AssignMinus, AssignMult, AssignDivide };

    public abstract class Node // базовый класс для всех узлов    
    {
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
            //v(this);
        }
        public override string ToString() => Num.ToString();

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
            return Left.ToString() + " " + Op + " " + Right.ToString();
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
            return Id.ToString() + " = " + Expr.ToString() + "\n";
        }
    }
    public abstract class LogicExprNode : Node { }
    public class LogicNumNode : LogicExprNode
    {
        public bool Val { get; set; }
        public LogicNumNode(bool val) { Val = val; }
        public override void Visit(Visitor v)
        {
        }
        public override string ToString()
        {
            return Val.ToString();
        }
    }
    public class LogicIdNode : LogicExprNode
    {
        public IdNode Val { get; set; }
        public LogicIdNode(IdNode val) { Val = val; }
        public override void Visit(Visitor v)
        {
        }
        public override string ToString()
        {
            return Val.ToString();
        }
    }
    public class LogicOperationNode : LogicExprNode
    {
        public LogicExprNode Before { get; set; }
        public LogicExprNode After { get; set; }
        public SimpleParser.Tokens Operation { get; set; }
        public LogicOperationNode(LogicExprNode b, LogicExprNode a, SimpleParser.Tokens t)
        {
            Before = b;
            After = a;
            Operation = t;
        }
        public override void Visit(Visitor v)
        {
        }
        public override string ToString()
        {
            return Before.ToString() + " " + Operation.ToString() + " " + After.ToString();
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
        public LogicExprNode Expr { get; set; }
        public StatementNode _IF { get; set; }
        public StatementNode _ELSE { get; set; }
        public IfNode(LogicExprNode Ex, StatementNode __IF, StatementNode __ELSE = null)
        {
            Expr = Ex;
            _IF = __IF;
            _ELSE = __ELSE;
        }
        public override void Visit(Visitor v)
        {
        }
        public override string ToString()
        {
            string res = "if ( " + Expr.ToString() + " )\n\t" + _IF.ToString();
            if (_ELSE != null)
                res += "\nelse\n\t" + _ELSE.ToString();
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
        }
        public override string ToString()
        {
            return "while ( " + Expr.ToString() + " )\n\t" + Stat.ToString();
        }
    }
    public class ForNode : StatementNode
    {
        public ExprNode StartValue { get; set; }
        public IdNode ID { get; set; }
        public ExprNode End { get; set; }
        public StatementNode Stat { get; set; }
        public ForNode(AssignNode start, ExprNode end, StatementNode stat)
        {
            StartValue = start.Expr;
            ID = start.Id;
            End = end;
            Stat = stat;
        }
        public override void Visit(Visitor v)
        {
        }
        public override string ToString()
        {
            return "for ( " + ID.ToString() + " = " + StartValue.ToString() + " to " + End.ToString() + " )\n\t" + Stat.ToString();
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
                res += "\t" + it.ToString() + "\n";
            }
            res += "}\n";
            return res;
        }
    }

    public class WriteNode : StatementNode
    {
        public ExprNode Expr { get; set; }
        public WriteNode(ExprNode Expr)
        {
            this.Expr = Expr;
        }
        public override void Visit(Visitor v)
        {
            v.VisitWriteNode(this);
        }
    }

    public class EmptyNode : StatementNode
    {
        public override void Visit(Visitor v)
        {
            v.VisitEmptyNode(this);
        }
        public override string ToString()
        {
            return "";
        }
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
    }
}