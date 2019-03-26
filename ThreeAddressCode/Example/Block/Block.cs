using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using ProgramTree;
using SimpleLang.Visitors;

namespace SimpleLang.Block
{
    public class Block
    {
        public LinkedList<ThreeCode> code;
        public Block(ThreeAddressCodeVisitor _code)
        {
            this.code = _code.GetCode();
        }

        public List<int> FindLiders()
        {
            var Liders = new List<int>();
            int i = 1;

            bool PreviousIsGoto = false;

            foreach (var line in this.code)
            {
                if (i == 1)
                    Liders.Add(i);
                else
                    if (!String.IsNullOrEmpty(line.label))
                        Liders.Add(i);
                    else
                        if (PreviousIsGoto)
                            Liders.Add(i);

                PreviousIsGoto = line.operation == ThreeOperator.Goto || line.operation == ThreeOperator.IfGoto;
                
                i += 1;
            }

            return Liders;
        }

        public List<List<ThreeCode>> GenerateBlocks()
        {
            var Liders = FindLiders();
            int i = 1;
            int LiderInd = 0;
            
            var Blocks = new List<List<ThreeCode>>();

            foreach (var line in this.code)
            {
                if (i == Liders[LiderInd])
                {
                    Blocks.Add(new List<ThreeCode>());
                    LiderInd += 1;
                }
                Blocks.Last().Add(line);
                i += 1;
            }

            return Blocks;
        }
    }
}