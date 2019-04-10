using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using SimpleScanner;
using SimpleParser;
using SimpleLang.Visitors;
using SimpleLang.Block;
using SimpleLang.Optimisations;
using SimpleLang.ControlFlowGraph;
using SimpleLang.ThreeCodeOptimisations;

namespace SimpleCompiler
{
    public class SimpleCompilerMain
    {
        public static void Main(string[] args)
        {
            string FileName = @"../../../data/a.txt";
            if (args.Length > 0)
                FileName = args[0];
            try {
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

                    FillParentVisitor generateParrent = new FillParentVisitor();
                    r.Visit(generateParrent);

                    var opt_sim_diff = new OptSimilarDifference();
                    r.Visit(opt_sim_diff);
                    var opt_sim_assignments = new OptSimilarAssignment();
                    r.Visit(opt_sim_assignments);
                    
                    Console.WriteLine("\nGenerate Three address code");
                    ThreeAddressCodeVisitor treeCode = new ThreeAddressCodeVisitor();
                    r.Visit(treeCode);
                    Console.WriteLine(treeCode.ToString());


                    AutoThreeCodeOptimiser app = new AutoThreeCodeOptimiser();
                    app.Add(new EvalConstExpr());
                    app.Add(new DistributionOfConstants());
                    app.Add(new ApplyAlgebraicIdentities());
                    //ToDo Add new threeCodeOptimisations

                    var blocks = app.Apply(treeCode);
                    Console.WriteLine(ThreeAddressCodeVisitor.ToString(blocks));


                    int i = 1;
                    foreach (var block in blocks)
                    {
                        Console.WriteLine("Block " + i.ToString());
                        foreach (var line in block)
                            Console.WriteLine(line);
                        i += 1;
                    }

                    var cfg = new ControlFlowGraph(treeCode);
                    cfg.GenerateCFG();
                    Console.WriteLine("\nControl flow graph:");
                    Console.WriteLine(cfg.GetAsGraph());
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

            // Console.ReadLine();
        }

    }
}
