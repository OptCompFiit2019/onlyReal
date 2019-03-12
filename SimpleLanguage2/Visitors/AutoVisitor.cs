using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    // базовая логика обхода без действий
    // Если нужны действия или другая логика обхода, то соответствующие методы надо переопределять
    // При переопределении методов для задания действий необходимо не забывать обходить подузлы
    class AutoVisitor: Visitor
    {
        public override void VisitBinOpNode(BinOpNode binop) 
        {
            binop.Left.Visit(this);
            binop.Right.Visit(this);
        }
        public override void VisitAssignNode(AssignNode a) 
        {
            // для каких-то визиторов порядок может быть обратный - вначале обойти выражение, потом - идентификатор
            a.Id.Visit(this);
            a.Expr.Visit(this);
        }		
		public override void VisitLogicOperationNode(LogicOperationNode lop)
		{
			lop.Left.Visit(this);
			lop.Right.Visit(this);
		}
        public override void VisitLogicNotNode(LogicNotNode lnot)
        {
            lnot.LogExpr.Visit(this);
        }
        public override void VisitWhileNode(WhileNode w)
		{
			w.Expr.Visit(this);
			w.Stat.Visit(this);
		}
		public override void VisitForNode(ForNode f)
		{
			f.StartValue.Visit(this);
			f.Id.Visit(this);
			f.End.Visit(this);
			f.Stat.Visit(this);
		}
		public override void VisitIfNode(IfNode ifn)
		{
			ifn.Expr.Visit(this);
			ifn.If.Visit(this);
			ifn.Else?.Visit(this);
		}
		public override void VisitCycleNode(CycleNode c) 
        {
            c.Expr.Visit(this);
            c.Stat.Visit(this);
        }
        public override void VisitBlockNode(BlockNode bl) 
        {
            foreach (var st in bl.StList)
                st.Visit(this);
        }
        public override void VisitPrintlnNode(PrintlnNode w) 
        {
            w.Expr.Visit(this);
        }
        public override void VisitVarDefNode(VarDefNode w) 
        {
            foreach (var v in w.vars)
                v.Visit(this);
        }
    }
}
