using System;
using System.Collections.Generic;
using System.Linq;
using ProgramTree;
using System.Text;
using SimpleLang.Block;
using SimpleLang.GenericIterativeAlgorithm;
using GenericTransferFunction;

namespace SimpleLang.Visitors
{
    // arg1 op arg2
    using Expr = ValueTuple<ThreeAddressValueType, ThreeOperator, ThreeAddressValueType>;
    using ExprSet = HashSet<(ThreeAddressValueType, ThreeOperator, ThreeAddressValueType)>;
    using KillerSet = HashSet<string>;

    // Для обратной совместимости с тестирующей системой
    using StringExprSet = HashSet<(string, string, string)>;

    public class AvaliableExprsAdaptor
    {
        public static TransferFunction<BlockInfo<Expr>> TransferFunction()
            => new TransferFunction<BlockInfo<Expr>>(bi =>
            {
                var Out = new BlockInfo<Expr>(bi);
                Out.OUT = AvaliableExprs.TransferByGenAndKiller(new ExprSet(bi.IN),
                    AvaliableExprs.GetGenExprSet(bi.Commands), AvaliableExprs.GetKillerSet(bi.Commands));
                return Out;
            });
    }

    public class AvaliableExprs
    {
        public static bool IsDefinition(ThreeOperator opType)
        {
            return opType != ThreeOperator.Println && opType != ThreeOperator.Goto
                && opType != ThreeOperator.IfGoto && opType != ThreeOperator.None;
        }
        public static ExprSet GetGenExprSet(LinkedList<ThreeCode> bblock)
        {
            
            var ret = new ExprSet();
            foreach (var line in bblock)
            {
                if (line.operation == ThreeOperator.Goto || line.operation == ThreeOperator.IfGoto
                        || line.operation == ThreeOperator.Println
                        || line.operation == ThreeOperator.None) continue;

                ret.RemoveWhere(x => x.Item1.ToString() == line.result
                    || x.Item3.ToString() == line.result);

                if (line.operation == ThreeOperator.Plus || line.operation == ThreeOperator.Mult || line.operation == ThreeOperator.Minus || line.operation == ThreeOperator.Logic_or || line.operation == ThreeOperator.Logic_not || line.operation == ThreeOperator.Logic_neq || line.operation == ThreeOperator.Logic_less || line.operation == ThreeOperator.Logic_leq || line.operation == ThreeOperator.Logic_greater || line.operation == ThreeOperator.Logic_geq || line.operation == ThreeOperator.Logic_equal || line.operation == ThreeOperator.Logic_and || line.operation == ThreeOperator.Div)
                {
                    ret.Add((line.arg1, line.operation, line.arg2));
                }
            }
            return ret;
        }

        public static KillerSet GetKillerSet(LinkedList<ThreeCode> bblock)
        {
            return new KillerSet(bblock
                                 .Where(l => IsDefinition(l.operation))
                                 .Select(l => l.result));
        }

        public (List<StringExprSet>, List<KillerSet>) GetGenAndKillerSets(List<LinkedList<ThreeCode>> bblocks)
        {
            return (bblocks.Select(b => new StringExprSet(GetGenExprSet(b)
                        .Select(e => (e.Item1.ToString(), e.Item2.ToString(), e.Item3.ToString()))))
                        .ToList(),
                    bblocks.Select(b => GetKillerSet(b)).ToList());
        }

        public static ExprSet TransferByGenAndKiller(ExprSet X, ExprSet gen, KillerSet kill)
        {
            if (X == null) return gen;
            return new ExprSet(X.Where(e => !kill.Contains(e.Item1.ToString())
                && !kill.Contains(e.Item3?.ToString())).Union(gen));
        }

        // Для обратной совместимости с тестирующей системой
        public StringExprSet TransferByGenAndKiller(StringExprSet X, StringExprSet gen, KillerSet kill)
        {
            if (X == null) return gen;
            return new StringExprSet(X.Where(e => !kill.Contains(e.Item1.ToString())
                && !kill.Contains(e.Item3.ToString())).Union(gen));
        }
    }
}
