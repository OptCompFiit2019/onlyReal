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
    class OptWhileVisitor: ChangeVisitor
    {
        private bool IsPerformed { get; set; }

        /// <summary>
        /// Выполняет замену <see cref="StatementNode"/> <paramref name="from"/> на <paramref name="to"/> 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        new public void ReplaceStat(StatementNode from, StatementNode to)
        {
            var p = from.Parent;
            if (p is AssignNode || p is ExprNode || p is PrintlnNode || p is EmptyNode)
            {
                throw new Exception("Родительский узел не содержит операторов");
            }
            if (to != null)
                to.Parent = p;
            if (p is BlockNode bln)
            {
                for (var i = 0; i < bln.StList.Count; i++)
                    if (bln.StList[i] == from)
                    {
                        bln.StList[i] = to;
                        break;
                    }
            }
        }

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
