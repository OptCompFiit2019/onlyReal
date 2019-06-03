using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang.Visitors
{
    class CommonExprBlocks
    {
        private LinkedList<ThreeCode> program;
        public LinkedList<ThreeCode> Program
        {
            set { program = value; }
            get { return program; }
        }
        public ThreeAddressCodeVisitor treecode;

        public CommonExprBlocks(ThreeAddressCodeVisitor code)
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
                    var graph = new CommonExpr(blocks[i].ToList());
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


    class CommonExpr
    {
        public List<ThreeCode> program;

        public CommonExpr(List<ThreeCode> prog)
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
            var result = new List<ThreeCode>();

            for (int i = 0; i < program.Count - 1; i++)
            {
                if (program[i].arg1 == null && program[i].arg2 == null)
                    continue;
                else
                {
                    for (int j = i + 1; j < program.Count; j++)
                    {
                        if (program[j].arg1.ToString() == program[i].arg1.ToString() && program[j].arg2 != null && program[i].arg2 != null && program[j].arg2.ToString() == program[i].arg2.ToString() && program[j].operation == program[i].operation)
                        {
                            program[j].operation = ThreeOperator.Assign;
                            program[j].arg1 = new ThreeAddressStringValue(program[i].result);
                            program[j].arg2 = null;
                        }
                        if (program[j].result == program[i].result || program[j].result == program[i].arg1.ToString() || (program[i].arg2 != null && program[j].result == program[i].arg2.ToString()))
                            break;
                    }
                }
            }

            for (int i = 0; i < program.Count; i++)
                result.Add(program[i]);
            return new LinkedList<ThreeCode>(result);
        }
    }
}
