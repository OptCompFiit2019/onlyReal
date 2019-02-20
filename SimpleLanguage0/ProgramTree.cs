using System.Collections.Generic;

namespace ProgramTree
{
    public enum AssignType { Assign, AssignPlus, AssignMinus, AssignMult, AssignDivide };

    public class Node // базовый класс для всех узлов    
    {
    }

    public class ExprNode : Node // базовый класс для всех выражений
    {
    }

    public class IdNode : ExprNode
    {
        public string Name { get; set; }
        public IdNode(string name) { Name = name; }
    }

    public class IntNumNode : ExprNode
    {
        public int Num { get; set; }
        public IntNumNode(int num) { Num = num; }
    }

    public class StatementNode : Node // базовый класс для всех операторов
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
    }
    public class WriteNode : StatementNode
    {
        public ExprNode ID { get; set; }
        public WriteNode(ExprNode id)
        {
            ID = id;
        }
    }
    public class IfNode : StatementNode
    {
        public ExprNode Expr { get; set; }
        public StatementNode _IF { get; set; }
        public StatementNode _ELSE { get; set; }
        public IfNode(ExprNode Ex, StatementNode __IF, StatementNode __ELSE = null)
        {
            Expr = Ex;
            _IF = __IF;
            _ELSE = __ELSE;
        }
    }
    public class ParamsNode : ExprNode
    {
        public System.Collections.Generic.List<ExprNode> Exprs { get; set; }
        public ParamsNode(ExprNode ex, ParamsNode nod = null)
        {
            Exprs = new List<ExprNode>();
            Exprs.Add(ex);
            if (nod != null)
                for (int i = 0; i < nod.Exprs.Count; i++)
                {
                    Exprs.Add(nod.Exprs[i]);
                }
        }
    }
    public class OperationNode : ExprNode
    {
        public ExprNode Before { get; set; }
        public ExprNode After { get; set; }
        public SimpleParser.Tokens Operation { get; set; }
        public OperationNode(ExprNode b, ExprNode a, SimpleParser.Tokens t)
        {
            Before = b;
            After = a;
            Operation = t;
        }
    }
    public class VarNode : StatementNode
    {
        public ParamsNode Params { get; set; }
        public VarNode(ParamsNode pa)
        {
            Params = pa;
        }
    }

    public class WhileNode : StatementNode
    {
        public ExprNode Expr { get; set; }
        public StatementNode Stat { get; set; }
        public WhileNode(ExprNode expr, StatementNode stat)
        {
            Expr = expr;
            Stat = stat;
        }
    }
    public class RepeatNode : StatementNode
    {
        public ExprNode Expr { get; set; }
        public StatementNode Stat { get; set; }
        public RepeatNode(ExprNode expr, StatementNode stat)
        {
            Expr = expr;
            Stat = stat;
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
    }

    public class BlockNode : StatementNode
    {
        public List<StatementNode> StList = new List<StatementNode>();
        public BlockNode(StatementNode stat)
        {
            Add(stat);
        }
        public void Add(StatementNode stat)
        {
            StList.Add(stat);
        }
    }

}