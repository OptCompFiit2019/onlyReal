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
            bool r = bln.StList.Exists(x => x is NullNode);
            r = r || bln.StList.Exists(x => x == null);
            if (r)
                SetApply(r);
            bln.StList = bln.StList.Where(x =>!(x is NullNode)).ToList();
            bln.StList = bln.StList.Where(x => (x != null)).ToList();
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
