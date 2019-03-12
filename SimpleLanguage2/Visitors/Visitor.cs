using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    public abstract class Visitor
    {
        public virtual void VisitIdNode(IdNode id) { }
        public virtual void VisitIntNumNode(IntNumNode num) { }		
		public virtual void VisitBinOpNode(BinOpNode binop) { }
        public virtual void VisitAssignNode(AssignNode a) { }
		public virtual void VisitDoubleNumNode(DoubleNumNode dnum) { }
		public virtual void VisitBooleanNode(BooleanNode lnum) { }
		public virtual void VisitLogicIdNode(LogicIdNode lid) { }
        public virtual void VisitLogicNotNode(LogicNotNode lnot) { }
        public virtual void VisitLogicOpNode(LogicOpNode lop) { }		
		public virtual void VisitWhileNode(WhileNode w) { }
		public virtual void VisitForNode(ForNode f) { }
		public virtual void VisitIfNode(IfNode ifn) { }
		public virtual void VisitCycleNode(CycleNode c) { }
		public virtual void VisitBlockNode(BlockNode bl) { }
        public virtual void VisitPrintlnNode(PrintlnNode w) { }
        public virtual void VisitVarDefNode(VarDefNode w) { }		
		public virtual void VisitEmptyNode(EmptyNode w) { }
    }
}
