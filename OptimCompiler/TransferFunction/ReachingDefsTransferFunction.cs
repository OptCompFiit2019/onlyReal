using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleLang.Visitors;
using CFG = SimpleLang.ControlFlowGraph.ControlFlowGraph;
using SimpleLang.GenericIterativeAlgorithm;
using GenericTransferFunction;

namespace SimpleLang
{
    using BasicBlock = LinkedList<ThreeCode>;

    public class ReachingDefsAdaptor
    {
        private ReachingDefsTransferFunction tf;

        public ReachingDefsAdaptor(CFG cfg)
            => tf = new ReachingDefsTransferFunction(cfg);

        public TransferFunction<BlockInfo<ThreeCode>> TransferFunction()
            => new TransferFunction<BlockInfo<ThreeCode>>(bi =>
            {
                var newIn = new BlockInfo<ThreeCode>(bi);
                newIn.IN = tf.BlockTransferFunction(bi.Commands)
                    (new HashSet<ThreeCode>(bi.OUT));
                return newIn;
            });
    }

    public class ReachingDefsTransferFunction
    {
        private CFG cfg;

        public ReachingDefsTransferFunction(CFG cfg) => this.cfg = cfg;

        private IEnumerable<ThreeCode> Definitions(BasicBlock bb)
            => bb.Where(tc => tc.operation != ThreeOperator.Goto
                && tc.operation != ThreeOperator.IfGoto
                && tc.operation != ThreeOperator.None);

        private List<HashSet<ThreeCode>> InstructionGens(BasicBlock bb)
            => Definitions(bb).Select(tc =>
                new HashSet<ThreeCode>(new ThreeCode[] { tc })
            ).ToList();

        private List<HashSet<ThreeCode>> InstructionKills(BasicBlock bb)
            => Definitions(bb).Select(tc =>
                new HashSet<ThreeCode>(cfg.blocks.Where(b => b != bb)
                    .Select(b => Definitions(b))
                    .SelectMany(e => e).Where(tc_cur => tc_cur.result == tc.result))
            ).ToList();

		public HashSet<ThreeCode> Kill(BasicBlock bb)
		{
			var kills = InstructionKills(bb);
			if (kills.Count == 0)
				return new HashSet<ThreeCode>();
			else
				return kills.Aggregate((s1, s2) =>
					new HashSet<ThreeCode>(s1.Union(s2))
			);
		}

        public HashSet<ThreeCode> Gen(BasicBlock bb)
        {
            var gens = InstructionGens(new LinkedList<ThreeCode>
				(bb.GroupBy(tc => tc.result, (tc, e) => e.Last())));
            var kills = InstructionKills(bb);

            int n = gens.Count;
			if (n <= 0)
				return new HashSet<ThreeCode>();

            HashSet<ThreeCode> gen = gens[n - 1];
            for (int i = n - 2; i >= 0; --i)
            {
                var gen_cur = gens[i];
                for (int j = i + 1; j < n; ++j)
                    gen_cur.ExceptWith(kills[j]);
                gen.UnionWith(gen_cur);
            }
            return gen;
        }

        private IEnumerable<InstructionTransferFunction> InstructionTransferFunctions(BasicBlock bb)
        {
            var tf = InstructionGens(bb).Zip(InstructionKills(bb), (g, k) =>
                new InstructionTransferFunction(g, k));
            if (tf.Count() == 0)
                tf = new InstructionTransferFunction[] { new InstructionTransferFunction() };
            return tf;
        }

        public Func<HashSet<ThreeCode>, HashSet<ThreeCode>> BlockTransferFunction(BasicBlock bb)
            => InstructionTransferFunctions(bb).Aggregate((f, g) => f * g).Func;
    }

    public class InstructionTransferFunction
    {
        public Func<HashSet<ThreeCode>, HashSet<ThreeCode>> Func;

        public InstructionTransferFunction()
            => Func = defs => defs;

        public InstructionTransferFunction(HashSet<ThreeCode> gen, HashSet<ThreeCode> kill)
            => Func = defs => new HashSet<ThreeCode>(gen.Union(defs.Except(kill)));

        public InstructionTransferFunction(Func<HashSet<ThreeCode>, HashSet<ThreeCode>> Func)
            => this.Func = Func;

        public static InstructionTransferFunction operator *
                (InstructionTransferFunction f, InstructionTransferFunction g)
            => new InstructionTransferFunction(defs => f.Func(g.Func(defs)));
    }
}
