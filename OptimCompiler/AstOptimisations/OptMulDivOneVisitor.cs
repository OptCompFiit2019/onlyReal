using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;
using SimpleLang.Visitors;

namespace SimpleLang.Visitors
{
    /// <summary>
    /// Выполняет оптимизации по удалению алгебраических выражений вида:
    /// 1 * ex, ex * 1, ex / 1
    /// </summary>
    class OptMulDivOneVisitor : ChangeVisitor
    {
        public bool IsPerformed { get; set; }

        /// <summary>
        /// Посещает узлы <paramref name="binop"/> и применяет оптимизацию по 
        /// замене выражений вида: 1 * ex, ex * 1, ex / 1 на ex
        /// </summary>
        /// <param name="binop"></param>
        public override void VisitBinOpNode(BinOpNode binop)
        {
            IsPerformed = false;
            if (binop.Op[0] == '*' && binop.Right is IntNumNode innRight
                 && innRight.Num == 1)
            {
                binop.Left.Visit(this);
                ReplaceExpr(binop, binop.Left);
                IsPerformed = true;
            }
            else if (binop.Op[0] == '*' && binop.Left is IntNumNode innLeft
                && innLeft.Num == 1)
            {
                binop.Right.Visit(this);
                ReplaceExpr(binop, binop.Right);
                IsPerformed = true;
            }
            else if (binop.Op[0] == '/' && binop.Right is IntNumNode innRightDiv
                     && innRightDiv.Num == 1)
            {
                binop.Left.Visit(this);
                ReplaceExpr(binop, binop.Left);
                IsPerformed = true;
            }
            else
            {
                base.VisitBinOpNode(binop);
            }

        }
    }
}