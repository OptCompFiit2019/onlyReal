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
            a.Parent = parent;
            st.Push(a);
            base.VisitAssignNode(a);
            st.Pop();
        }
    }
}
