using SimpleLang.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleLang.ControlFlowGraph;

namespace SimpleCompiler
{
    using ExprSet = HashSet<(ThreeAddressValueType, ThreeOperator, ThreeAddressValueType)>;

    public class IterativeAlgAvailableExprs
    {
        private IEnumerable<ThreeCode> Definitions(LinkedList<ThreeCode> bb)
            => bb.Where(tc => tc.operation != ThreeOperator.Goto
                && tc.operation != ThreeOperator.IfGoto
                && tc.operation != ThreeOperator.None);

        private List<HashSet<ThreeCode>> InstructionGens(LinkedList<ThreeCode> bb)
            => Definitions(bb).Select(tc =>
                new HashSet<ThreeCode>(new ThreeCode[] { tc })
            ).ToList();

        //Процедура вывода в консоль множества выражений, достпуных на входе и на выходе
        public void PrintInputOutputAvaliableExpr(List<LinkedList<ThreeCode>> _bblocks)
        {
            var ioAE = this.GenerateInputOutputAvaliableExpr(_bblocks);

            Console.WriteLine("A set of expressions available at the entrance:");
            if (ioAE.Item1 != null)
                foreach (var hashS in ioAE.Item1)
                {
                    if (hashS != null)
                        foreach (var hashlem in hashS)
                            Console.WriteLine("{0} {1} {2}", hashlem.Item1, hashlem.Item2, hashlem.Item3);
                    Console.WriteLine();
                }

            Console.WriteLine("\nA set of expressions available at the exit:");
            if (ioAE.Item2 != null)
                foreach (var hashS in ioAE.Item2)
                {
                    if (hashS != null)
                        foreach (var hashlem in hashS)
                            Console.WriteLine("{0} {1} {2}", hashlem.Item1, hashlem.Item2, hashlem.Item3);
                    Console.WriteLine();
                }
        }

        //Фукнция, возвращающая множества выражений, достпуных на входе и на выходе
        public (List<ExprSet>, List<ExprSet>) GenerateInputOutputAvaliableExpr(List<LinkedList<ThreeCode>> _bblocks)
        {
            //Множество выражений, доступных на входе IN[B] для всех ББл B
            var In = new List<ExprSet>();
            //Множество выражений, доступных на выходе OUT[B] для всех ББл B
            var Out = new List<ExprSet>();

            //Экземпляр класса AvaliableExprs для некоторых методов
            var ae = new AvaliableExprs();

            //e_genB
            Dictionary<int, ExprSet> _genExprByStart = new Dictionary<int, ExprSet>();

            Dictionary<int, List<HashSet<ThreeCode>>> _defByStart = new Dictionary<int, List<HashSet<ThreeCode>>>();

            for (int i = 0; i < _bblocks.Count; i++)
                _genExprByStart[i] = AvaliableExprs.GetGenExprSet(_bblocks[i]);

            for (int i = 0; i < _bblocks.Count; i++)
                _defByStart[i] = InstructionGens(_bblocks[i]);

            for (int i = 0; i < _bblocks.Count(); ++i)
            {
                In.Add(null);
                Out.Add(new ExprSet());

                if (i > 0)
                    Out[i] = new ExprSet(_genExprByStart.SelectMany(kv => kv.Value.ToList()));
            }

            //Внесены ли изменения в Out
            bool change = true;
            while (change)
            {
                change = false;

                //Каждый ББ отличный от входного
                for (int B = 1; B < _bblocks.Count(); ++B)
                {
                    In[B] = null;
                    var cfg = new ControlFlowGraph(_bblocks);
                    var inputIndexes = cfg.cfg.GetInputNodes(B);
                    foreach (var P in inputIndexes)
                        //Проверяем, что коллекция еще не создана
                        if (In[B] == null)
                            In[B] = new ExprSet(Out[P]);
                        else
                            In[B].IntersectWith(Out[P]);

                    int sz = Out[B].Count;

                    Out[B] = AvaliableExprs.TransferByGenAndKiller(In[B],
                        AvaliableExprs.GetGenExprSet(_bblocks[B]), AvaliableExprs.GetKillerSet(_bblocks[B]));

                    change |= sz != Out[B].Count;
                }

            }
            return (In, Out);
        }
    }
}
