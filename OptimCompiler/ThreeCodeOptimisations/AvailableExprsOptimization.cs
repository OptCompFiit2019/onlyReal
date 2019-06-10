using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CFG = SimpleLang.ControlFlowGraph.ControlFlowGraph;
using SimpleLang.Visitors;
using SimpleLang.GenericIterativeAlgorithm;
using SimpleLang;
using SimpleLang.ThreeCodeOptimisations;

namespace SimpleLang.ExprOptimisations
{
    using Expr = ValueTuple<ThreeAddressValueType, ThreeOperator, ThreeAddressValueType>;

    class AvailableExprsOptimizer : ThreeCodeOptimiser
	{
        private CFG controlFlowGraph;

        private int currentTempVarIndex = 0;

        public List<HashSet<Expr>> Ins { get; private set; }
        public List<HashSet<Expr>> Outs { get; private set; }

        public void IterativeAlgorithm(List<LinkedList<ThreeCode>> blocks)
        {
            var bb = new LinkedList<ThreeCode>();
            bb.AddLast(new ThreeCode("entry", "", ThreeOperator.None, null, null));
            var bs = blocks.ToList();
            // добавление пустого входного блока - необходимо для корректной работы ит. алгоритма
            bs.Insert(0, bb);
            // построение CFG по блокам
            controlFlowGraph = new CFG(bs);
            // создание информации о блоках
            var blocksInfo = new List<BlockInfo<Expr>>();
            for (int i = 0; i < bs.Count; i++)
                blocksInfo.Add(new BlockInfo<Expr>(bs[i]));

            // оператор сбора для доступных выражений
            Func<List<BlockInfo<Expr>>, CFG, int, BlockInfo<Expr>> meetOperator =
                (blocksInfos, graph, index) =>
                {
                    var inputIndexes = graph.cfg.GetInputNodes(index);
                    var resInfo = new BlockInfo<Expr>(blocksInfos[index]);
                    resInfo.IN = resInfo.OUT; // универсальное множество
                    foreach (var i in inputIndexes)
                        resInfo.IN.IntersectWith(blocksInfos[i].OUT);
                    return resInfo;
                };

            var transferFunction = AvaliableExprsAdaptor.TransferFunction();

            var U = new HashSet<Expr>(blocks.Select(b => b.Where(c =>
                    AvaliableExprs.IsDefinition(c.operation))
                .Select(c => (c.arg1, c.operation, c.arg2)))
                .Aggregate((s1, s2) => s1.Union(s2)));

            // создание объекта итерационного алгоритма
            var iterativeAlgorithm = new IterativeAlgorithm<Expr>(blocksInfo,
                controlFlowGraph, meetOperator, true, new HashSet<Expr>(), U, transferFunction);

            // выполнение алгоритма
            iterativeAlgorithm.Perform();
            Ins = iterativeAlgorithm.GetINs();
            Outs = iterativeAlgorithm.GetOUTs();

            //var itAlg = new SimpleCompiler.IterationAlgorithm.IterativeAlgAvailableExprs();
            //(Ins, Outs) = itAlg.GenerateInputOutputAvaliableExpr(bs);

            //Func<IEnumerable<BlockInfo<Expr>>, BlockInfo<Expr>, BlockInfo<Expr>> meetOp =
            //    (blocksInfos, bInfo) =>
            //    {
            //        var resInfo = new BlockInfo<Expr>(bInfo);
            //        resInfo.IN = resInfo.OUT; // универсальное множество
            //        foreach (var bi in blocksInfos)
            //            resInfo.IN.IntersectWith(bi.OUT);
            //        return resInfo;
            //    };

            //var mopAlg = new MOPAlgorithm<Expr>(blocksInfo,
            //    controlFlowGraph, meetOp, true, new HashSet<Expr>(), U, transferFunction);

            //mopAlg.Perform();
            //Ins = mopAlg.GetINs();
            //Outs = mopAlg.GetOUTs();
        }

		public void Apply(ref LinkedList<ThreeCode> program)
		{
			// заглушка
		}

		public bool NeedFullCode() => true;

		public void Apply(ref List<LinkedList<ThreeCode>> res)
		{
			var old = res;
			var availableExprsOptimizer = new AvailableExprsOptimizer();
			CFG cfg = availableExprsOptimizer.ApplyOptimization(old);
			res = cfg.blocks;
			for (int i = 0; i < old.Count; ++i)
			{
				var it1 = old[i].First;
				var it2 = res[i].First;
				for (int j = 0; j < old[i].Count; ++j)
				{
					if (it1.Value.ToString() != it2.Value.ToString())
					{
						Applied = true;
						return;
					}
					it1 = it1.Next;
					it2 = it2.Next;
				}
			}
		}

		private bool Applied = false;
		public bool Applyed() => Applied;

		public CFG ApplyOptimization(List<LinkedList<ThreeCode>> blocks)
        {
            IterativeAlgorithm(blocks);
            var bs = controlFlowGraph.blocks;

            for (int i = 1; i < bs.Count; ++i)
            {
                for (var it = bs[i].First; true; it = it.Next)
                {
                    var command = it.Value;
                    var expr = (command.arg1, command.operation, command.arg2);
                    if (Ins[i].Contains(expr))
                    {
                        string t = GenTempVariable();
                        it.Value = new ThreeCode(command.result, new ThreeAddressStringValue(t));
                        ApplyOptToAncestors(i, expr, t);
                        if (Outs[i].Contains(expr))
                            ApplyOptToDescendents(i, expr, t);
                    }

                    Ins[i].ExceptWith(Ins[i].Where(e => e.Item1.ToString() == command.result
                        || e.Item3?.ToString() == command.result).ToList());
                    if (it == bs[i].Last)
                        break;
                }
            }
            return controlFlowGraph;
        }

        private void ApplyOptToAncestors(int index, Expr expr, string tempVar)
        {
            var bs = controlFlowGraph.blocks;

            var inputIndexes = controlFlowGraph.cfg.GetInputNodes(index);
            foreach (var ii in inputIndexes)
            {
                if (!Outs[ii].Contains(expr))
                    continue;
                var ancestor_it = bs[ii].Last;
                bool applied = false;
                bool notFirst;
                while (notFirst = ancestor_it.Value.result != expr.Item1.ToString()
                        && ancestor_it.Value.result != expr.Item3.ToString())
                {
                    if (!ExprEq((ancestor_it.Value.arg1,
                                ancestor_it.Value.operation,
                                ancestor_it.Value.arg2), expr))
                        if (ancestor_it == bs[ii].First)
                        {
                            ApplyOptToAncestors(ii, expr, tempVar);
                            break;
                        }
                        else
                            ancestor_it = ancestor_it.Previous;
                    else
                    {
                        ancestor_it.Value = new ThreeCode(ancestor_it.Value.result,
                            new ThreeAddressStringValue(tempVar));
                        applied = true;
                        Outs[ii].Remove(expr);
                    }
                }
                if (applied)
                    if (notFirst)
                        bs[ii].AddBefore(ancestor_it, new ThreeCode(tempVar, expr.Item2,
                                    expr.Item1, expr.Item3));
                    else
                        bs[ii].AddAfter(ancestor_it, new ThreeCode(tempVar, expr.Item2,
                                    expr.Item1, expr.Item3));
            }
        }

        private void ApplyOptToDescendents(int index, Expr expr, string tempVar)
        {
            var bs = controlFlowGraph.blocks;

            var outputIndexes = controlFlowGraph.cfg.GetOutputNodes(index);
            foreach (var ii in outputIndexes)
            {
                var ancestor_it = bs[ii].First;
                while (ancestor_it.Value.result != expr.Item1.ToString()
                        && ancestor_it.Value.result != expr.Item3.ToString())
                {
                    if (ExprEq((ancestor_it.Value.arg1,
                                ancestor_it.Value.operation,
                                ancestor_it.Value.arg2), expr))
                        ancestor_it.Value = new ThreeCode(ancestor_it.Value.result,
                            new ThreeAddressStringValue(tempVar));
                    if (ancestor_it == bs[ii].Last)
                        break;
                    ancestor_it = ancestor_it.Next;
                }
                if (Outs[ii].Contains(expr))
                    ApplyOptToDescendents(ii, expr, tempVar);
            }
        }

        private bool ExprEq(Expr left, Expr right)
        {
            return left.Item1?.ToString() == right.Item1?.ToString()
                && left.Item2 == right.Item2
                && left.Item3?.ToString() == right.Item3?.ToString();
        }

        private string GenTempVariable()
        {
            string res = "#t" + currentTempVarIndex.ToString();
            currentTempVarIndex++;
            return res;
        }
    }
}
