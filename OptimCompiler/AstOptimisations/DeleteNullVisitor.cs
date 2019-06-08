using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    public class DeleteNullVisitor: AutoApplyVisitorInterface
    {
        public override void VisitBlockNode(BlockNode bln)
        {
            bool r = bln.StList.Exists(x => x is null);
            if (r)
                SetApply(r);
            bln.StList = bln.StList.Where(x =>!(x is NullNode)).ToList();
        }
        public override void VisitIfNode(IfNode ifn)
        {
            if (ifn.Else is NullNode)
            {
                ReplaceStat(ifn, new NullNode());
            }
        }
    }
}
