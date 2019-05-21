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
        public static void Main(string[] args) {

            string FileName = @"../../../data/DeadOrAliveOptimization.txt";
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


                    //Console.WriteLine(r.ToString());

                    /*Opt2Visitor opt2 = new Opt2Visitor();
					r.Visit(opt2);

                    PrettyPrintVisitor ppvis = new PrettyPrintVisitor();
                    r.Visit(ppvis);
                    Console.WriteLine(ppvis.Text);

                    Console.WriteLine("\nAssignCountVisitor");
                    AssignCountVisitor vis1 = new AssignCountVisitor();
                    r.Visit(vis1);
                    Console.WriteLine(vis1.Count);

                    Console.WriteLine("\nStatementCountVisitor");
                    StatementCountVisitor vis2 = new StatementCountVisitor();
                    r.Visit(vis2);
                    Console.WriteLine(vis2.Count);

                    Console.WriteLine("\nMaxCountExprOpsVisitor");
					MaxCountExprOpsVisitor vis3 = new MaxCountExprOpsVisitor();
					r.Visit(vis3);
					Console.WriteLine(vis3.Max);

					Console.WriteLine("\nNestedCyclesVisitor");
					NestedCyclesVisitor vis4 = new NestedCyclesVisitor();
					r.Visit(vis4);
					Console.WriteLine(vis4.HasNestedCycles);

					Console.WriteLine("\nCycleNestedToIfVisitor");
					CycleNestedToIfVisitor vis5 = new CycleNestedToIfVisitor();
					r.Visit(vis5);
					Console.WriteLine(vis5.HasCycleNestedToIf);

                    Console.WriteLine("\nIfNestedToCycleVisitor");
                    IfNestedToCycleVisitor vis6 = new IfNestedToCycleVisitor();
                    r.Visit(vis6);
                    Console.WriteLine(vis6.HasIfNestedToCycle);

                    Console.WriteLine("\nMaxDepthOfNestedCyclesVisitor");
                    MaxDepthOfNestedCyclesVisitor vis7 = new MaxDepthOfNestedCyclesVisitor();
                    r.Visit(vis7);
                    Console.WriteLine(vis7.Max);*/

                    Console.WriteLine("\nGenerate Three address code");

                    ThreeAddressCodeVisitor treeCode = new ThreeAddressCodeVisitor();
                    r.Visit(treeCode);
                    var blocks = new Block(treeCode).GenerateBlocks();

                    // добавление фиктивных блоков входа и выхода программы
                    var entryPoint = new LinkedList<ThreeCode>();
                    entryPoint.AddLast(new ThreeCode("entry", "", ThreeOperator.None, null, null));
                    var exitPoint = new LinkedList<ThreeCode>();
                    exitPoint.AddLast(new ThreeCode("exit", "", ThreeOperator.None, null, null));
                    blocks.Insert(0, entryPoint);
                    blocks.Add(exitPoint);

                    // построение CFG
                    CFG controlFlowGraph = new CFG(blocks);
                    Console.WriteLine(treeCode.ToString());
                    // выполнение оптимизации для программы, не разбитой на блоки
                    //DeadOrAliveOptimization.DeleteDeadVariables(treeCode.GetCode());
                    // вычисление множеств Def и Use для всего графа потоков данных
                    var DefUse = new DefUseBlocks(controlFlowGraph);

                    var InOut = new InOutActiveVariables(DefUse, controlFlowGraph);

                    ControlFlowOptimisations.DeadOrAliveOnGraph(InOut, controlFlowGraph);
                    Console.WriteLine("\nafter DeleteDeadVariables for graph\n");
                    foreach (var block in controlFlowGraph.blocks)
                        foreach (var line in block)
                            Console.WriteLine(line);
                    Console.Write("");
                    //DeadOrAliveOptimization.



                    //SimpleLang.Compiler.ILCodeGenerator gen = new SimpleLang.Compiler.ILCodeGenerator();
                    //gen.Generate(treeCode.GetCode());
                    //var lst = gen.GetGenerator().commands;
                    //foreach(string cmd in lst)
                    //{
                    //    Console.WriteLine(cmd);
                    //}
                    //Console.WriteLine("\nExecute:");
                    //gen.Execute();

                    /*AutoThreeCodeOptimiser app = new AutoThreeCodeOptimiser();
                    app.Add(new DistributionOfConstants());
                    app.Add(new EvalConstExpr());
                    app.Add(new ApplyAlgebraicIdentities());

                    var blocks = app.Apply(treeCode);
                    Console.WriteLine(ThreeAddressCodeVisitor.ToString(blocks));

					CFG cfg = new CFG(blocks);
					TransferFunction tf = new TransferFunction(cfg);
					Console.WriteLine("\nGen 1");
					foreach (var d in tf.Gen(blocks[0]))
						Console.WriteLine(d);
					Console.WriteLine("\nGen");
					foreach (var d in tf.Gen(blocks[3]))
						Console.WriteLine(d);
					Console.WriteLine("\nKill");
					foreach (var d in tf.Kill(blocks[3]))
						Console.WriteLine(d);

					Console.WriteLine("\nTransfer function");
					var f = tf.BlockTransferFunction(blocks[3]);
					foreach (var d in f(tf.Gen(blocks[0])))
						Console.WriteLine(d);

					var code = treeCode.GetCode();
                    app.Apply(code);*/



                    /*Opt11Visitor opt11vis = new Opt11Visitor();
                    ppvis.Text = "";
                    r.Visit(opt11vis);
                    r.Visit(ppvis);
                    Console.WriteLine(ppvis.Text);*/


                    /*var avis = new AssignCountVisitor();
                    parser.root.Visit(avis);
                    Console.WriteLine("Количество присваиваний = {0}", avis.Count);
                    Console.WriteLine("-------------------------------");

                    var pp = new PrettyPrintVisitor();
                    parser.root.Visit(pp);
                    Console.WriteLine(pp.Text);*/
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

           // ========
           // My part, don't touch !!!


           CFG controlFlowGraph = new CFG(blocks);
           Console.WriteLine(treeCode.ToString());
           // выполнение оптимизации для программы, не разбитой на блоки
           //DeadOrAliveOptimization.DeleteDeadVariables(treeCode.GetCode());
           // вычисление множеств Def и Use для всего графа потоков данных
           var DefUse = new DefUseBlocks(controlFlowGraph);

           var InOut = new InOutActiveVariables(DefUse, controlFlowGraph);

           ControlFlowOptimisations.DeadOrAliveOnGraph(InOut, controlFlowGraph);
           Console.WriteLine("\nafter DeleteDeadVariables for graph\n");
           foreach (var block in controlFlowGraph.blocks)
               foreach (var line in block)
                   Console.WriteLine(line);
           Console.Write("");

           // ========
        }

    }
}
