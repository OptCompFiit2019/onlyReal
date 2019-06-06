using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleLang.ControlFlowGraph;

namespace SimpleLang.Dominators
{
    class DominatorsFinder
    {
        private readonly ControlFlowGraph.ControlFlowGraph cfg;

        private Dictionary<int, List<int>> dominators;
        public Dictionary<int, List<int>> Dominators
        {
            get
            {
                if (dominators == null)
                    dominators = Find();
                return dominators;
            }
        }

        public DominatorsFinder(ControlFlowGraph.ControlFlowGraph cfg)
        {
            this.cfg = cfg;
        }

        public Dictionary<int, List<int>> Find()
        {
            if (dominators != null)
                return dominators;

            dominators = new Dictionary<int, List<int>>
            {
                [0] = new List<int>() { 0 }
            };

            var allBlocks = Enumerable.Range(0, cfg.blocks.Count);
            var initOut = Enumerable.Range(1, cfg.blocks.Count - 1);
            for (int i = 1; i < cfg.blocks.Count; i++)
            {
                dominators[i] = new List<int>(allBlocks);
            }

            var adjMatr = cfg.GetAsAdjacencyMatrix();
            var blocksInputs = new List<List<int>>();
            for (int i = 1; i < cfg.blocks.Count; i++)
            {
                blocksInputs.Add(new List<int>());
                for (int j = 0; j < cfg.blocks.Count; j++)
                {
                    if (i != j && adjMatr[j, i] == 1)
                        blocksInputs.Last().Add(j);
                }
            }
            

            var f = true;
            while (f)
            {
                f = false;
                for (int i = 1; i < cfg.blocks.Count; i++)
                {
                    var _in = new List<int>(allBlocks);
                    foreach (var bl in blocksInputs[i - 1])
                    {
                        _in = _in.Intersect(dominators[bl]).ToList();
                    }

                    if (!_in.Contains(i))
                        _in.Add(i);
                    if (!f)
                    {
                        f = dominators[i].Count != _in.Count;
                        if (!f)
                            foreach (var item in _in)
                            {
                                f = !dominators[i].Contains(item);
                                if (f)
                                    break;
                            }
                    }
                    dominators[i] = _in;
                }
            }

            return dominators;
        }
    }
}
