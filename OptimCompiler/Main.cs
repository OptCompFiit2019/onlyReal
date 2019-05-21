using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using SimpleScanner;
using SimpleParser;
using SimpleLang;
using SimpleLang.Visitors;
using SimpleLang.ThreeCodeOptimisations;
using CFG = SimpleLang.ControlFlowGraph.ControlFlowGraph;
using SimpleLang.Block;
using SimpleLang.ThreeCodeOptimisations;

namespace SimpleCompiler
{
    public class SimpleCompilerMain
    {
        public static void Main(string[] args)
        {

            string FileName = @"../../../data/OptLVN.txt";
            if (args.Length > 0)
                FileName = args[0];
            try
            {
                string Text = File.ReadAllText(FileName);

                Scanner scanner = new Scanner();
                scanner.SetSource(Text, 0);

                Parser parser = new Parser(scanner);

                var b = parser.Parse();
                if (!b)
                    Console.WriteLine("Ошибка");
                else
                {
                    Console.WriteLine("Синтаксическое дерево построено");
                    var r = parser.root;

                    FillParentVisitor generateParent = new FillParentVisitor();
                    r.Visit(generateParent);

                    Opt2Visitor opt2 = new Opt2Visitor();
                    r.Visit(opt2);
                    PrettyPrintVisitor ppvis = new PrettyPrintVisitor();
                    r.Visit(ppvis);
                    Console.WriteLine(ppvis.Text);

                    Console.WriteLine("\nGenerate Three address code");

                    ThreeAddressCodeVisitor threeCode = new ThreeAddressCodeVisitor();
                    r.Visit(threeCode);


                    var blocks = new Block(threeCode).GenerateBlocks();

                    // добавление фиктивных блоков входа и выхода программы
                    var entryPoint = new LinkedList<ThreeCode>();
                    entryPoint.AddLast(new ThreeCode("entry", "", ThreeOperator.None, null, null));
                    var exitPoint = new LinkedList<ThreeCode>();
                    exitPoint.AddLast(new ThreeCode("exit", "", ThreeOperator.None, null, null));
                    blocks.Insert(0, entryPoint);
                    blocks.Add(exitPoint);


                    // построение CFG по блокам
                    CFG controlFlowGraph = new CFG(blocks);

                    Console.WriteLine("Блоки трехадресного кода до оптимизации");
                    foreach (var block in controlFlowGraph.blocks)
                        foreach (var line in block)
                            Console.WriteLine(line);


                    foreach (var block in controlFlowGraph.blocks)
                    {
                        var replace = LVNOptimization.LVNOptimize(block);
                        block.Clear();
                        foreach (var line in replace)
                            block.AddLast(line);
                    }

                    Console.WriteLine("\nПосле применения LVN\n");
                    foreach (var block in controlFlowGraph.blocks)
                        foreach (var line in block)
                            Console.WriteLine(line);

                    // полученный controlFlowGraph можно обратно преобразовывать в исходный код
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Файл {0} не найден", FileName);
            }
            catch (Exception e)
            {
                Console.WriteLine("{0}", e);
            }

            Console.ReadLine();
        }

    }
}
