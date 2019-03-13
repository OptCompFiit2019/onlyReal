using System;
using System.Collections.Generic;
using ProgramTree;
namespace SimpleLang.Visitors
{
    class FillParentVisitor : AutoVisitor
    {
        Stack<Node> st = new Stack<Node>(); // можно заменить на List
        FillParentVisitor() { st.Push(null);  }
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
