using System;
using System.Collections.Generic;
using ProgramTree;
namespace SimpleLang.Visitors
{
    public class FillParentVisitor : AutoVisitor
    {
        Stack<Node> st = new Stack<Node>(); // можно заменить на List
        public FillParentVisitor() { st.Push(null);  }
        public override void VisitBinOpNode(BinOpNode binop)
        {
            Node parent = st.Pop();
            st.Push(parent);
            binop.Parent = parent;
            st.Push(binop);
            base.VisitBinOpNode(binop);
            st.Pop();
        }
        public override void VisitAssignNode(AssignNode a)
        {
            Node parent = st.Pop();
            st.Push(parent);
			if (a != null)
				a.Parent = parent;
            st.Push(a);
            base.VisitAssignNode(a);
            st.Pop();
        }
        public override void VisitLogicOpNode(LogicOpNode lop)
        {
            if (lop == null)
                return;
            Node parent = st.Pop();
            st.Push(parent);
            lop.Parent = parent;
            st.Push(lop);
            base.VisitLogicOpNode(lop);
            st.Pop();
        }
        public override void VisitLogicNotNode(LogicNotNode lnot)
        {
            if (lnot == null)
                return;
            Node parent = st.Pop();
            st.Push(parent);
            lnot.Parent = parent;
            st.Push(lnot);
            base.VisitLogicNotNode(lnot);
            st.Pop();
        }
        public override void VisitWhileNode(WhileNode w)
        {
            if (w == null)
                return;
            Node parent = st.Pop();
            st.Push(parent);
            w.Parent = parent;
            st.Push(w);
            base.VisitWhileNode(w);
            st.Pop();
        }
        public override void VisitForNode(ForNode f)
        {
            if (f == null)
                return;
            Node parent = st.Pop();
            st.Push(parent);
            f.Parent = parent;
            st.Push(f);
            base.VisitForNode(f);
            st.Pop();
        }
		public override void VisitIfNode(IfNode ifn)
		{
			Node parent = st.Pop();
			st.Push(parent);
			if (ifn != null)
				ifn.Parent = parent;
			st.Push(ifn);
			base.VisitIfNode(ifn);
			st.Pop();
		}
		public override void VisitBlockNode(BlockNode bl)
		{
			Node parent = st.Pop();
			st.Push(parent);
			if (bl != null)
				bl.Parent = parent;
			st.Push(bl);
			base.VisitBlockNode(bl);
			st.Pop();
		}
		public override void VisitPrintlnNode(PrintlnNode p)
		{
			Node parent = st.Pop();
			st.Push(parent);
			if (p != null)
				p.Parent = parent;
			st.Push(p);
			base.VisitPrintlnNode(p);
			st.Pop();
		}
		public override void VisitVarDefNode(VarDefNode w)
		{
			Node parent = st.Pop();
			st.Push(parent);
			if (w != null)
				w.Parent = parent;
			st.Push(w);
			base.VisitVarDefNode(w);
			st.Pop();
		}
	}
}
