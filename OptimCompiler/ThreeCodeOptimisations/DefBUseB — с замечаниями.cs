using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleLang.Visitors;

namespace SimpleLang.ThreeCodeOptimisations
// или namespace SimpleLang.ControlFlowOptimisations , т.к. больше относится к оптимизациям на основе анализа потоков данных 
{
    class DefBUseBBlocks 
    {
        public LinkedList<ThreeCode> Program { get; } // Зачем нужно это поле, если оно не используется?
        private ThreeAddressCodeVisitor ThreeCodeVisitor; // зачем public?
        public List<HashSet<string>> DefBs { get; }
        public List<HashSet<string>> UseBs { get; }

        public DefBUseBBlocks(ThreeAddressCodeVisitor codeVisitor)
        {
            ThreeCodeVisitor = codeVisitor;
            Program = codeVisitor.GetCode();
            DefBs = new List<HashSet<string>>();
            UseBs = new List<HashSet<string>>();
            MakeSets();
            /*вычисление def и use в конструкторе для упрощения использования класса
             ведь проще написать 
             ...
             var DefUse = new DefBUseBBlocks(treecode);
             var use = DefUse.DefBs.ToArray();
             ...
             чем
             ...
             var DefUse = new DefBUseBBlocks(treecode);
             DefUse.MakeSets(); - интуитивно неясно, что этот метод нужно вызвать для изменения состояния объекта DefUse
             var use = DefUse.DefBs.ToArray();
             ...
             то же самое можно сделать и для отдельного блока DefBUseB
             */
        }

        /* MakeSets() не создает фиктивных блоков входа и выхода из программы. 
        эти блоки далее могут быть необходимы при анализе и оптимизациях.
        Если они не будут добавлены, то этот пункт необходимо обозначить в документации.
        Добавление фиктивных блоков можно сделать после получения кода визитором следующим образом:
        
            var entryPoint = new LinkedList<ThreeCode>();
            entryPoint.AddLast(new ThreeCode("entry", null, ThreeOperator.None, null, null));
            var exitPoint = new LinkedList<ThreeCode>();
            exitPoint.AddLast(new ThreeCode("exit", null, ThreeOperator.None, null, null));
            blocks.Insert(0, entryPoint);
            blocks.Add(exitPoint);
         
         */
        private void MakeSets()
        {
            var DefUse = new DefBUseBBlocks(ThreeCodeVisitor);
            DefUse.DefBs.ToArray();
            var blocks = new SimpleLang.Block.Block(ThreeCodeVisitor).GenerateBlocks();

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
