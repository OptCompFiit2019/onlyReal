using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleLang.Visitors;
using CFG = SimpleLang.ControlFlowGraph.ControlFlowGraph;

namespace SimpleLang.ThreeCodeOptimisations
{
    class DefUseBlocks
    {
        public List<HashSet<string>> DefBs { get; }
        public List<HashSet<string>> UseBs { get; }
        readonly CFG Graph;

        public DefUseBlocks(CFG graph)
        {
            DefBs = new List<HashSet<string>>();
            UseBs = new List<HashSet<string>>();
            Graph = graph;
            MakeSets();
        }

        private void MakeSets()
        {
            foreach (var block in Graph.blocks)
            {
                var DefB = new HashSet<string>();
                var UseB = new HashSet<string>();
                foreach (var cmd in block)
                {
                    if (cmd.arg1 != null && cmd.arg1.ToString() != "" && cmd.arg1 is ThreeAddressStringValue 
                        && !cmd.arg1.ToString().StartsWith("temp_") && !cmd.arg1.ToString().StartsWith("label") 
                        && !DefB.Contains(cmd.arg1.ToString()))
                        UseB.Add(cmd.arg1.ToString());
                    if (cmd.arg2 != null && cmd.arg2.ToString() != "" &&  cmd.arg2 is ThreeAddressStringValue 
                        && !cmd.arg2.ToString().StartsWith("temp_") && !cmd.arg2.ToString().StartsWith("label") 
                        && !DefB.Contains(cmd.arg2.ToString()))
                        UseB.Add(cmd.arg2.ToString());
                    if (cmd.result != null && cmd.result.ToString() != "" && !cmd.result.ToString().StartsWith("temp_") )
                        DefB.Add(cmd.result);
                }
                DefBs.Add(DefB);
                UseBs.Add(UseB);
            }
        }
    }

}
