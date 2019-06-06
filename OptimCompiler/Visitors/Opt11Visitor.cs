using System;
using ProgramTree;

namespace SimpleLang.Visitors
{
    public class Opt11Visitor: AutoApplyVisitorInterface
    {
        public Opt11Visitor()
        {
        }

        public override void VisitIfNode(IfNode ifn)
        {
            if (ifn.Cond is BooleanNode bn && bn.Val == true)
            {
                if (ifn.If != null)
                    ifn.If.Visit(this);
                SetApply();
                ReplaceStat(ifn, ifn.If);
            }
            else
                base.VisitIfNode(ifn);
        }
    }
}
