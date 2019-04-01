using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    class AssignCountVisitor : AutoVisitor
    {
        public int Count = 0;
        public override void VisitAssignNode(AssignNode a)
        {
            Count += 1;
        }
        public override void VisitWriteNode(WriteNode w)
        {
        }
        public override void VisitVarDefNode(VarDefNode w)
        {
        }
    }

    class StatCountVisitor : AutoVisitor
    {
        public Dictionary<string, int> counter = new Dictionary<string, int>();

        public StatCountVisitor()
        {
            counter["if"] = 0;
            counter["for"] = 0;
            counter["while"] = 0;
        }

        public override void VisitIfNode(IfNode a)
        {
            counter["if"]++;

            a.StatIf.Visit(this);
            if (a.StatElse != null)
                a.StatElse.Visit(this);
        }

        public override void VisitForNode(ForNode f)
        {
            counter["for"]++;

            f.Stat.Visit(this);
        }

        public override void VisitWhileNode(WhileNode w)
        {
            counter["while"]++;

            w.Stat.Visit(this);
        }

    }

    class MaxOpCountVisitor : AutoVisitor
    {
        public int Count = 0;
        public int Max = 0;
        public BinOpNode startOp = null;
        public override void VisitBinOpNode(BinOpNode a)
        {
            if (startOp == null)
                startOp = a;
            if (a is BinOpNode)
                Count += 1;
            a.Left.Visit(this);
            a.Right.Visit(this);
            if (a == startOp)
            {
                Max = (Count > Max) ? Count : Max;
                Count = 0;
                startOp = null;
            }
        }
    }

    class NestedLoopVisitor : AutoVisitor
    {
        public bool isNested = false;
        public ForNode startOpFor = null;
        public WhileNode startOpWhile = null;

        public void VisitBlockTemp(BlockNode b)
        {
            foreach (var st in b.StList)
            {
                if (st is ForNode || st is WhileNode)
                {
                    isNested = true;
                    return;
                }
            }
        }

        public override void VisitForNode(ForNode a)
        {
            if (startOpFor == null)
                startOpFor = a;
            if (a.Stat is BlockNode)
            {
                VisitBlockTemp(a.Stat as BlockNode);
            }
            else if ((a != startOpFor) && (a is ForNode))
            {
                isNested = true;
                return;
            }
            a.Stat.Visit(this);
        }

        public override void VisitWhileNode(WhileNode a)
        {
            if (startOpWhile == null)
                startOpWhile = a;
            if (a.Stat is BlockNode)
            {
                VisitBlockTemp(a.Stat as BlockNode);
            }
            else if ((a != startOpWhile) && (a is WhileNode))
            {
                isNested = true;
                return;
            }

            a.Stat.Visit(this);
        }
    }

    class IfInCycleVisitor : AutoVisitor
    {
        public bool flag = false;
        public ForNode startOpFor = null;
        public WhileNode startOpWhile = null;

        public void VisitBlockTemp(BlockNode b)
        {
            foreach (var st in b.StList)
            {
                if (st is IfNode)
                {
                    flag = true;
                    return;
                }
            }
        }

        public override void VisitForNode(ForNode a)
        {
            if (startOpFor == null)
                startOpFor = a;
            if (a.Stat is BlockNode)
            {
                VisitBlockTemp(a.Stat as BlockNode);
            }
            else if (a.Stat is IfNode)
            {
                flag = true;
                return;
            }

            a.Stat.Visit(this);
        }

        public override void VisitWhileNode(WhileNode a)
        {
            if (startOpWhile == null)
                startOpWhile = a;
            if (a.Stat is BlockNode)
            {
                VisitBlockTemp(a.Stat as BlockNode);
            }
            else if (a.Stat is IfNode)
            {
                flag = true;
                return;
            }

            a.Stat.Visit(this);
        }
    }

    class CycleInIfVisitor : AutoVisitor
    {
        public bool flag = false;
        public IfNode startOp = null;

        public void VisitBlockTemp(BlockNode b)
        {
            foreach (var st in b.StList)
            {
                if (st is ForNode || st is WhileNode)
                {
                    flag = true;
                    return;
                }
            }
        }

        public override void VisitIfNode(IfNode a)
        {
            if (startOp == null)
                startOp = a;
            if (a.StatIf is BlockNode)
            {
                VisitBlockTemp(a.StatIf as BlockNode);
            }
            else if (a.StatIf is ForNode || a.StatIf is WhileNode)
            {
                flag = true;
                return;
            }

            if (a.StatElse != null)
            {
                if (a.StatElse is BlockNode)
                {
                    VisitBlockTemp(a.StatElse as BlockNode);
                }
                else if (a.StatElse is ForNode || a.StatElse is WhileNode)
                {
                    flag = true;
                    return;
                }
            }

            a.StatIf.Visit(this);
            if (a.StatElse != null)
                a.StatElse.Visit(this);
        }
    }

    class MaxDepthNestedVisitor : AutoVisitor
    {
        public ForNode startOpFor = null;
        public WhileNode startOpWhile = null;
        public int Count = 0;
        public int Max = 0;

        public void VisitBlockTemp(BlockNode b)
        {
            foreach (var st in b.StList)
            {
                if (st is ForNode || st is WhileNode)
                {
                    Count += 1;
                    st.Visit(this);
                }
            }
        }

        public override void VisitForNode(ForNode a)
        {
            if (startOpFor == null)
                startOpFor = a;
            if (a.Stat is BlockNode)
            {
                VisitBlockTemp(a.Stat as BlockNode);
                Max = (Count > Max) ? Count : Max;
                Count = 0;
            }
            else
            {
                if ((a != startOpFor) && (a is ForNode))
                {
                    Count += 1;
                }
                else
                {
                    Max = (Count > Max) ? Count : Max;
                    Count = 1;
                }
            }

            a.Stat.Visit(this);
        }

        public override void VisitWhileNode(WhileNode a)
        {
            if (startOpWhile == null)
                startOpWhile = a;
            if (a.Stat is BlockNode)
            {
                VisitBlockTemp(a.Stat as BlockNode);
                Max = (Count > Max) ? Count : Max;
                Count = 0;
            }
            else
            {
                if ((a != startOpWhile) && (a is WhileNode))
                {
                    Count += 1;
                }
                else
                {
                    Max = (Count > Max) ? Count : Max;
                    Count = 1;
                }
            }

            a.Stat.Visit(this);
        }
    }

    class ThreeAddressVisitor : AutoVisitor
    {
        public int tmpVar = 1;
        public int tmpLabel = 1;
        public List<string> commands = new List<string>();
        public string genTmpName()
        {
            string tmpName = "t" + tmpVar.ToString();
            tmpVar++;
            return tmpName;
        }
        public string genTmpLabel()
        {
            string tmpName = "L" + tmpLabel.ToString();
            tmpLabel++;
            return tmpName;
        }
        public void genCommand(string cmd)
        {
            this.commands.Add(cmd);
        }
        public string gen(ExprNode ex)
        {
            if (ex is BinOpNode)
            {
                var bin = (BinOpNode)ex;
                string tmp1 = gen(bin.Left);
                string tmp2 = gen(bin.Right);
                string tmp = genTmpName();
                genCommand(tmp + " = " + tmp1 + bin.Op + tmp2);
                return tmp;
            }
            else
            {
                return ex.ToString();
            }
        }

        public void genStatementListCommand(StatementNode st)
        {
            if (st is BlockNode)
            {
                var stList = (BlockNode)st;
                foreach (var stat in stList.StList)
                    stat.Visit(this);
            }
            else
                st.Visit(this);
        }
        public override void VisitIfNode(IfNode n)
        {
            string tmp = gen(n.Expr);
            string L1 = genTmpLabel();
            string L2 = genTmpLabel();
            genCommand("if " + tmp + " goto " + L1);
            if (n.StatElse != null)
                n.StatElse.Visit(this);
            genCommand("goto " + L2);
            genCommand(L1 + ": ");
            n.StatIf.Visit(this);
            genCommand(L2 + ": ");
        }

        public override void VisitAssignNode(AssignNode a)
        {
            string tmp = gen(a.Expr);
            genCommand(a.Id + " = " + tmp);
        }
        public override void VisitForNode(ForNode f)
        {
            string loopIter = f.Assign.Id.ToString();
            string L1 = genTmpLabel();
            string tmp = gen(new BinOpNode(f.Assign.Id, f.Expr, "<="));
            genCommand(L1 + ": " + f.Assign.ToString());
            genStatementListCommand(f.Stat);
            genCommand(loopIter + " = " + loopIter + " + 1");
            genCommand("if " + tmp + " goto " + L1);
        }

        public override void VisitWhileNode(WhileNode w)
        {
            string L1 = genTmpLabel();
            string L2 = genTmpLabel();
            string L3 = genTmpLabel();
            string tmp = gen(w.Expr);
            genCommand(L1 + ": if " + tmp + " goto " + L2);
            genCommand("goto " + L3);
            genCommand(L2 + ": ");
            genStatementListCommand(w.Stat);
            genCommand("goto " + L1);
            genCommand(L3 + ": ");
        }
    }

    class FillParentVisitor : AutoVisitor
    {
        Stack<Node> st = new Stack<Node>(); // можно заменить на List
        public FillParentVisitor()
        {
            st.Push(null);
        }
        public override void VisitBinOpNode(BinOpNode binop)
        {
            binop.Parent = st.Peek();
            st.Push(binop);
            base.VisitBinOpNode(binop);
            st.Pop();
        }
        public override void VisitAssignNode(AssignNode a)
        {
            a.Parent = st.Peek();
            st.Push(a);
            base.VisitAssignNode(a);
            st.Pop();
        }
        public override void VisitIfNode(IfNode ifn)
        {
            ifn.Parent = st.Peek();
            st.Push(ifn);
            base.VisitIfNode(ifn);
            st.Pop();
        }

        public override void VisitWhileNode(WhileNode w)
        {
            w.Parent = st.Peek();
            st.Push(w);
            base.VisitWhileNode(w);
            st.Pop();
        }
    }

    class ChangeVisitor : AutoVisitor
    {
        public void ReplaceExpr(ExprNode from, ExprNode to)
        {
            var p = from.Parent;
            if (p == null)
                from = to;
            if (to.Parent != null)
                to.Parent = p;
            if (p is AssignNode assn)
            {
                assn.Expr = to;
            }
            else if (p is BinOpNode binopn)
            {
                if (binopn.Left == from) // Поиск подузла в Parent
                    binopn.Left = to;
                else if (binopn.Right == from)
                    binopn.Right = to;
            }
            else if (p is BlockNode)
            {
                throw new Exception("Родительский узел не содержит выражений");
            }
        }

        public void ReplaceStat(StatementNode from, StatementNode to)
        {
            var p = from.Parent;
            if (p is AssignNode || p is ExprNode)
            {
                //throw new Exception("Родительский узел не содержит операторов");

            }
            to.Parent = p;
            if (p is BlockNode bln) // Можно переложить этот код на узлы!
            {
                for (var i = 0; i < bln.StList.Count - 1; i++)
                    if (bln.StList[i] == from)
                        bln.StList[i] = to;
            }
            else if (p is IfNode ifn)
            {
                if (ifn.StatIf == from) // Поиск подузла в Parent
                    ifn.StatIf = to;
                else if (ifn.StatElse == from)
                    ifn.StatElse = to;
            }
        }
    }

    class Opt1Visitor : ChangeVisitor
    {
        public override void VisitBinOpNode(BinOpNode binop)
        {
            if ((binop.Left is IdNode) && (binop.Right is IdNode) &&
                (binop.Left as IdNode).Name == (binop.Right as IdNode).Name &&
                (binop.Op == "==" || binop.Op == ">="))
            {
                if (binop.Parent is IfNode ifn)
                    ifn.Expr = new BoolNode(true);
                else if (binop.Parent is WhileNode w)
                    w.Expr = new BoolNode(true);
                else
                    ReplaceExpr(binop, new BoolNode(true));
            }
            else if ((binop.Left is ExprNode) && (binop.Right is ExprNode) &&
                     (binop.Left.ToString() == binop.Right.ToString()) &&
                     (binop.Op == "==" || binop.Op == ">="))
            {
                if (binop.Parent is IfNode ifn)
                    ifn.Expr = new BoolNode(true);
                else if (binop.Parent is WhileNode w)
                    w.Expr = new BoolNode(true);
                else
                    ReplaceExpr(binop, new BoolNode(true));
            }
            else
            {
                base.VisitBinOpNode(binop); // Обойти потомков обычным образом
            }
        }
        public override void VisitIfNode(IfNode ifn)
        {
            ifn.Expr.Visit(this);
        }
        public override void VisitWhileNode(WhileNode w)
        {
            w.Expr.Visit(this);
        }
    }
    class Opt2Visitor : ChangeVisitor
    {
        public override void VisitBlockNode(BlockNode bl)
        {
            for (int i = 0; i < bl.StList.Count; i++)
                if (bl.StList[i] is IfNode ifn)
                {
                    var stlist1 = ifn.StatIf as BlockNode;
                    var stlist2 = ifn.StatElse as BlockNode;
                    bool null1, null2;
                    null1 = null2 = false;
                    if (stlist1.StList.Count == 1 & stlist1.StList[0] is NullNode)
                        null1 = true;
                    if (stlist2.StList.Count == 1 & stlist2.StList[0] is NullNode)
                        null2 = true;

                    if (null1 && null2)
                        bl.StList[i] = new NullNode();
                    else
                        base.VisitIfNode(ifn);
                }
        }
    }
}

    
    
