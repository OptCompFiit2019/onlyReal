using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang.Visitors
{
    class PullCopiesVisitor : Visitor
    {
        private LinkedList<ThreeCode> program;
        public LinkedList<ThreeCode> Program
        {
            set { program = value; }
            get { return program; }
        }

        public PullCopiesVisitor(LinkedList<ThreeCode> prog)
        {
            Program = prog;
        }

        public void Optimize()
        {
            var list = Program.ToList();
            for (int i = 0; i < Program.Count - 1; i++)
            {
                string def;
                ThreeAddressValueType newArg;
                if (list[i].arg2  == null)
                {
                    def = list[i].result;
                    newArg = list[i].arg1;
                }
                else
                    continue;
                for (int j = i + 1; j < Program.Count; j++)
                {
                    if (list[j].result == def)
                        break;
                    if (list[j].arg1.ToString() == def)
                        list[j].arg1 = newArg;

                    if (list[j].arg2 != null && list[j].arg2.ToString() == def)
                        list[j].arg2 = newArg;
                }
            }
            Program = new LinkedList<ThreeCode>(list);
        }

        public List<List<ThreeCode>> BaseBlocks()
        {
            List<List<ThreeCode>> blocks = new List<List<ThreeCode>>();
            var list = Program.ToList();
            var leader = list[0];
            int j = 0;
            blocks.Add(new List<ThreeCode>());
            blocks[0].Add(leader);

            for (int i = 1; i < Program.Count; i++)
            {
                if (list[i].label != "" || list[i - 1].operation is ThreeOperator.Goto || list[i - 1].operation is ThreeOperator.IfGoto)
                {
                    j++;
                    leader = list[i];
                    blocks.Add(new List<ThreeCode>());
                    blocks[j].Add(leader);
                }
                else
                    blocks[j].Add(list[i]);
            }

            return blocks;
        }

        public void Optimize2()
        {
            var blocks = BaseBlocks();
            var list = Program.ToList();
            int num = 0;
            //идем по базовым блокам
            for (int i = 0; i < blocks.Count; i++)
            {
                if (blocks[i].Count > 1)
                {
                    var block = blocks[i];
                    //идем по каждой команде в блоке
                    for (int j = 0; j < block.Count; j++)
                    {
                        var mainCommand = block[j];
                        //использовалась ли раньше такая команда в этом блоке?
                        for (int k = 0; k < j; k++)
                        {
                            var tempCommand = block[k];
                            if (mainCommand.arg2 != null && tempCommand.arg2!=null && 
                                mainCommand.arg1.Equals(tempCommand.arg1) && mainCommand.operation.Equals(tempCommand.operation) && mainCommand.arg2.Equals(tempCommand.arg2))
                            {
                                //если да, то в промежутке между этими определениями менялись ли arg1 или arg2?
                                for (int m = k + 1; m < j; m++)
                                {
                                    var def = block[m];
                                    if (!(def.result == tempCommand.arg1.ToString() || def.result == tempCommand.arg2.ToString()))
                                        list[num] = new ThreeCode(blocks[i][j].result, block[k + 1].arg1);
                                }
                            }
                        }
                        num++;
                    }
                }
                else num++;
            }
            Program = new LinkedList<ThreeCode>(list);
        }
        
    }
}