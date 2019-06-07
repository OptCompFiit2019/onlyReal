using ProgramTree;
using SimpleLang.Visitors;

namespace SimpleLang.AstOptimisations
{
    public class FalseExprMoreAndNonEqualVisitor : AutoApplyVisitorInterface
    {
        public override void VisitLogicOpNode(LogicOpNode lop)
        {
            if ((lop.Operation == ">" || lop.Operation == "!=") && lop.Right.ToString() == lop.Left.ToString())
            {
                ReplaceExpr(lop, new BooleanNode(false));
            }
        }
    }
}
