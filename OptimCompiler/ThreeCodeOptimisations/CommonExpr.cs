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
            Program = code.program;
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

            //идем по каждой команде в блоке
            for (int j = 0; j < program.Count; j++)
            {
                var mainCommand = program[j];
                result.Add(mainCommand);
                //использовалась ли раньше такая команда в этом блоке?
                for (int k = j - 1; k >= 0; k--)
                {
                    var flag = false;
                    var tempCommand = program[k];
                    if (mainCommand.arg2 != null && tempCommand.arg2 != null &&
                        mainCommand.arg1.Equals(tempCommand.arg1) && mainCommand.operation.Equals(tempCommand.operation) && mainCommand.arg2.Equals(tempCommand.arg2))
                    {
                        //если да, то в промежутке между этими определениями менялись ли arg1 или arg2?
                        for (int m = k + 1; m < j; m++)
                        {
                            var def = program[m];
                            if (!(def.result == tempCommand.arg1.ToString() || def.result == tempCommand.arg2.ToString()))
                            {
                                result[result.Count - 1] = new ThreeCode(mainCommand.result, new ThreeAddressStringValue(tempCommand.result));//какой тип должен быть??????
                                flag = true;
                                break;
                            }
                        }
                        //если вдруг одинаковые команды идут друг за другом
                        if (k + 1 == j)
                            result[result.Count - 1] = new ThreeCode(mainCommand.result, new ThreeAddressStringValue(tempCommand.result));
                    }
                    if (flag) break;
                }
            }
            
            return new LinkedList<ThreeCode>(result);
        }
    }
}
