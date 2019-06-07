using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProgramTree;

namespace SimpleLang.Visitors
{
    public class LessOptVisitor : AutoApplyVisitorInterface
    {
        public override void VisitLogicOpNode(LogicOpNode lop)
        {
            if (lop.Left is ExprNode)
                lop.Left.Visit(this); // Вначале сделать то же в левом поддереве

            if (lop.Right is ExprNode)
                lop.Right.Visit(this); // Затем в правом поддереве

            if (lop.Left is IntNumNode l && lop.Right is IntNumNode r 
                && (lop.Operation == ">" || lop.Operation == "<"))
            {
                //Посчитаем сам знак
                bool nbool = lop.Operation == ">" ? l.Num > r.Num : l.Num < r.Num;
                //Проверим, что выражение не является условиев if
                if (lop.Parent is IfNode ifn) {
                    ifn.Cond = new BooleanNode(nbool);
                    SetApply();
                }
                //Проверим, что выражение не является условиев while
                else if (lop.Parent is WhileNode w) { 
                    w.Expr = new BooleanNode(nbool);
                    SetApply();
                }else
                    ReplaceExpr(lop, new BooleanNode(nbool));
            }
            else // Если оптимизаций нет, то
            {
                base.VisitLogicOpNode(lop); // Обойти потомков обычным образом
            }
        }

    }
}
