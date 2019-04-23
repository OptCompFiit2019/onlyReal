using ProgramTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleLang.Visitors;

namespace SimpleLang.Optimisations
{
    /// <summary>
    /// Выполняет замену на null узла WhileNode в случае 
    /// while (false) st;
    /// </summary>
    class OptWhileVisitor: ChangeVisitor
    {
        public bool IsPerformed { get; set; }

        /// <summary>
        /// Выполняет замену <see cref="StatementNode"/> <paramref name="from"/> на <paramref name="to"/> 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void ReplaceStat(StatementNode from, StatementNode to)
        {
            var p = from.Parent;
            if (p is AssignNode || p is ExprNode || p is PrintlnNode || p is EmptyNode)
            {
                throw new Exception("Родительский узел не содержит операторов");
            }
            if (to != null)
                to.Parent = p;
            if (p is BlockNode bln) // Можно переложить этот код на узлы!
            {
                //bln.StList = bln.StList.Select(bl => bl == from ? to : bl).ToList();
                for (var i = 0; i < bln.StList.Count; i++)
                    if (bln.StList[i] == from)
                    {
                        bln.StList[i] = to;
                        break;
                    }
            }
            //else if (p is IfNode ifn)
            //{
            //    if (ifn.BlockIf == from) // Поиск подузла в Parent
            //        ifn.BlockIf = to;
            //    else if (ifn. == from)
            //        ifn.BlockElse = to;
            //}
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
                
        }
    }
}
