using System;
using ProgramTree;

namespace SimpleLang.Visitors
{
    class Opt11Visitor: ChangeVisitor
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
                ReplaceStat(ifn, ifn.If);
            }
            else
                base.VisitIfNode(ifn);
        }
    }
}
