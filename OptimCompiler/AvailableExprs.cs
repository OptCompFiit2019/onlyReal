using System;
using System.Collections.Generic;
using System.Linq;
using ProgramTree;
using System.Text;
using SimpleLang.Block;

namespace SimpleLang.Visitors
{
    // arg1 op arg2
    using ExprSet = HashSet<(String, String, String)>;
    using KillerSet = HashSet<String>;
    
    public static class AvaliableExprs
    {
        public static bool IsDefinition(ThreeOperator opType)
        {
            return opType != ThreeOperator.Logic_not && opType != ThreeOperator.Goto && opType != ThreeOperator.IfGoto;
        }
        public static ExprSet GetGenExprSet(LinkedList<ThreeCode> bblock)
        {
            
            var ret = new ExprSet();
            foreach (var line in bblock)
            {
                if (line.operation == ThreeOperator.Goto || line.operation == ThreeOperator.Println) continue;

                ret.RemoveWhere(x => x.Item1 == line.result || x.Item3 == line.result);

                if (line.operation == ThreeOperator.Plus || line.operation == ThreeOperator.Mult || line.operation == ThreeOperator.Minus || line.operation == ThreeOperator.Logic_or || line.operation == ThreeOperator.Logic_not || line.operation == ThreeOperator.Logic_neq || line.operation == ThreeOperator.Logic_less || line.operation == ThreeOperator.Logic_leq || line.operation == ThreeOperator.Logic_greater || line.operation == ThreeOperator.Logic_geq || line.operation == ThreeOperator.Logic_equal || line.operation == ThreeOperator.Logic_and || line.operation == ThreeOperator.Div)
                {
                    ret.Add((line.arg1.ToString(), line.operation.ToString(), line.arg2.ToString()));
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

        public static (List<ExprSet>, List<KillerSet>) GetGenAndKillerSets(List<LinkedList<ThreeCode>> bblocks)
        {
            return (bblocks.Select(b => GetGenExprSet(b)).ToList(),
                    bblocks.Select(b => GetKillerSet(b)).ToList());

        }

        public static ExprSet TransferByGenAndKiller(ExprSet X, ExprSet gen, KillerSet kill)
        {
            if (X == null) return gen;
            return new ExprSet(X.Where(e => !kill.Contains(e.Item1) && !kill.Contains(e.Item3))
                               .Union(gen));
        }
    }
}
