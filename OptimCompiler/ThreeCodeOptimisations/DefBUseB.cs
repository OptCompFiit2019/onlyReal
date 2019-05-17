using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang.Visitors
{
    class DefBUseBBlocks 
    {
        private LinkedList<ThreeCode> program;
        public LinkedList<ThreeCode> Program
        {
            set { program = value; }
            get { return program; }
        }
        public ThreeAddressCodeVisitor treecode;
        public List<HashSet<string>> DefBs;
        public List<HashSet<string>> UseBs;

        public DefBUseBBlocks(ThreeAddressCodeVisitor code)
        {
            treecode = code;
            Program = code.GetCode();
            DefBs = new List<HashSet<string>>();
            UseBs = new List<HashSet<string>>();
        }

        public void MakeSets()
        {
            var blocks = new SimpleLang.Block.Block(treecode).GenerateBlocks();

            for (int i = 0; i < blocks.Count; i++)
            {
                var graph = new DefBUseB(blocks[i].ToList());
                graph.MakeSet();
                DefBs.Add(graph.DefB);
                UseBs.Add(graph.UseB);
            }
        }
    }

    class DefBUseB
    {
        public List<ThreeCode> program;
        public HashSet<string> DefB;
        public HashSet<string> UseB;

        public DefBUseB(List<ThreeCode> prog)
        {
            program = MakeProgram(prog);

            DefB = new HashSet<string>();
            UseB = new HashSet<string>();
        }

        public List<ThreeCode> MakeProgram(List<ThreeCode> prog)
        {
            var program = new List<ThreeCode>();
            int i = 0;
            for (i = 0; i < prog.Count - 1; i++)
            {
                if (prog[i + 1].arg1 != null && prog[i].result.Equals(prog[i + 1].arg1.ToString()) && prog[i].result.Contains("temp_") && prog[i + 1].arg2 == null)
                {
                    if (prog[i].label != "")
                        program.Add(new ThreeCode(prog[i].label, prog[i + 1].result, prog[i].operation, prog[i].arg1, prog[i].arg2));
                    else
                        program.Add(new ThreeCode(prog[i + 1].result, prog[i].operation, prog[i].arg1, prog[i].arg2));
                    i++;
                }
                else
                    program.Add(prog[i]);
            }
            if (i == prog.Count - 1)
                program.Add(prog[i]);
            return program;
        }

        public void MakeSet()
        {
            for (int i = 0; i < program.Count; i++)
            {
                var cmd = program[i];
                if (cmd.arg1 != null && cmd.arg1 is ThreeAddressStringValue && !cmd.arg1.ToString().Contains("temp_") && !DefB.Contains(cmd.arg1.ToString()))
                    UseB.Add(cmd.arg1.ToString());
                if (cmd.arg2 != null && cmd.arg2 is ThreeAddressStringValue && !cmd.arg2.ToString().Contains("temp_") && !DefB.Contains(cmd.arg2.ToString()))
                    UseB.Add(cmd.arg2.ToString());
                if (!cmd.result.ToString().Contains("temp_"))
                    DefB.Add(cmd.result);
            }
        }
    }
}
