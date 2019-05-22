using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang.Visitors
{
    class PullCopiesBlocks
    {
        private LinkedList<ThreeCode> program;
        public LinkedList<ThreeCode> Program
        {
            set { program = value; }
            get { return program; }
        }
        public ThreeAddressCodeVisitor treecode;

        public PullCopiesBlocks(ThreeAddressCodeVisitor code)
        {
            treecode = code;
            Program = code.GetCode();
        }

        public void Optimize()
        {
            var blocks = new SimpleLang.Block.Block(treecode).GenerateBlocks();
            var result = new List<ThreeCode>();
            for (int i = 0; i < blocks.Count; i++)
            {
                if (blocks[i].Count > 1)
                {
                    var graph = new PullCopies(blocks[i].ToList());
                    var prog = graph.Optimize();
                    foreach (var cmd in prog)
                        result.Add(cmd);
                }
                else
                    result.Add(blocks[i].First.Value);
            }
            Program = new LinkedList<ThreeCode>(result);
        }
    }

    class PullCopies
    {
        public List<ThreeCode> program;

        public PullCopies(List<ThreeCode> prog)
        {
            program = MakeProgram(prog);
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

        public LinkedList<ThreeCode> Optimize()
        {
            for (int i = 0; i < program.Count - 1; i++)
            {
                string def;
                ThreeAddressValueType newArg;
                if (program[i].arg2 == null)
                {
                    def = program[i].result;
                    newArg = program[i].arg1;
                }
                else
                    continue;
                for (int j = i + 1; j < program.Count; j++)
                {
                    if (program[j].result == def)
                        break;
                    if (program[j].arg1 != null && program[j].arg1.ToString() == def)
                        program[j].arg1 = newArg;

                    if (program[j].arg2 != null && program[j].arg2.ToString() == def)
                        program[j].arg2 = newArg;
                }
            }
            return new LinkedList<ThreeCode>(program);
        }
    }
}