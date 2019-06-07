using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    class DeleteNullVisitor: ChangeVisitor
    {
        public override void VisitBlockNode(BlockNode bln)
        {
            bln.StList = bln.StList.Where(x => x != null).ToList();
        }
        public override void VisitIfNode(IfNode ifn)
        {
            if (ifn.Else == null && ifn.If == null)
            {
                ReplaceStat(ifn, null);
            }
        }
    }
}
