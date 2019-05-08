using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    class AssignCountVisitor : AutoVisitor
    {
        public int Count = 0;
        public override void VisitAssignNode(AssignNode a)
        {
            Count += 1;
        }
        public override void VisitPrintlnNode(PrintlnNode w) 
        {
        }
        public override void VisitVarDefNode(VarDefNode w)
        { 
        }
    }
}
