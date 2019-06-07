using ProgramTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleLang.Visitors;

namespace SimpleLang.Visitors
{
    /// <summary>
    /// Выполняет замену на null узла WhileNode в случае 
    /// while (false) st;
    /// </summary>
    public class OptWhileVisitor: AutoApplyVisitorInterface
    {
        private bool IsPerformed { get; set; }


        /// <summary>
        /// Посещает узел типа WhileNode <paramref name="wn"/> и в случае
        /// константы false в условии, заменяет его на null в AST
        /// </summary>
        /// <param name="wn"></param>
        public override void VisitWhileNode(WhileNode wn)
        {
            IsPerformed = false;
            if (wn.Expr is BooleanNode bnn && !bnn.Val)
            {
                ReplaceStat(wn, null);
                IsPerformed = true;
            }
            else
                wn.Stat?.Visit(this);
        }
    }
}
