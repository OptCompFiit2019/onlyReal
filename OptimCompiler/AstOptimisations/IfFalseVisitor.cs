using ProgramTree;

namespace SimpleLang.Visitors
{
    public class IfFalseVisitor : AutoApplyVisitorInterface
    {
        public override void VisitIfNode(IfNode ifn)
        {
            if (ifn.Cond is BooleanNode boolVal && !boolVal.Val)
            {
                ifn.Else.Visit(this);
                ReplaceStat(ifn, ifn.Else);
            }
        }
    }
}