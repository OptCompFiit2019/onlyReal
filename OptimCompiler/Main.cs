using System;
using System.IO;
using System.Collections.Generic;
using SimpleScanner;
using SimpleParser;
using SimpleLang.Visitors;
using SimpleLang.ThreeCodeOptimisations;
using CFG = SimpleLang.ControlFlowGraph.ControlFlowGraph;
using SimpleLang.Block;
using SimpleLang.GenericIterativeAlgorithm;
using GenericTransferFunction;

namespace SimpleCompiler
{
    public class SimpleCompilerMain
    {
        public static void Main(string[] args)
        {

            string FileName = @"../../../data/DeadOrAliveOptimization.txt";
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

                    FillParentVisitor generateParrent = new FillParentVisitor();
                    r.Visit(generateParrent);

                    Console.WriteLine("\nGenerate Three address code");

                    ThreeAddressCodeVisitor threeCode = new ThreeAddressCodeVisitor();
                    r.Visit(threeCode);

                    var pepeVisitor = new PrettyPrintVisitor();
                    r.Visit(pepeVisitor);
                    //var r = parser.root;    // корень AST
                    Console.Write("Before:\n" + pepeVisitor.Text);
                    r.Visit(new FillParentVisitor());   // установка ссылок на родителей на AST
                    pepeVisitor.Text = "";
                    r.Visit(new OptMulDivOneVisitor());	// выполнение текущей оптимизации
                    r.Visit(pepeVisitor);
                    Console.Write("After:\n" + pepeVisitor.Text);

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


                    Console.WriteLine("Блоки трехадресного кода до каскадного удаления мертвых переменных\n" + controlFlowGraph);

                    // вычисление множеств Def и Use для всего графа потоков данных                    
                    var DefUse = new DefUseBlocks(controlFlowGraph);

                    // создание информации о блоках
                    var blocksInfo = new List<BlockInfo>();
                    for(int i = 0; i < DefUse.DefBs.Count; i++)
                        blocksInfo.Add(new BlockInfo(DefUse.DefBs[i], DefUse.UseBs[i]));

                    // оператор сбора для анализа активных переменных
                    Func<List<BlockInfo>, CFG, int, BlockInfo> meetOperator = (blocksInfos, graph, index) =>
                    {
                        var successorIndexes = graph.cfg.GetOutputNodes(index);
                        var resInfo = new BlockInfo(blocksInfos[index]);
                        foreach(var i in successorIndexes)
                            resInfo.OUT.UnionWith(blocksInfos[i].IN);
                        return resInfo;
                    };

                    // делегат передаточной функции для анализа активных переменных
                    Func<BlockInfo, BlockInfo> tFunc1 = (blockInfo) =>
                    {
                        blockInfo.IN = new HashSet<string>();
                        blockInfo.IN.UnionWith(blockInfo.OUT);
                        return blockInfo;
                    };

                    Func<BlockInfo, BlockInfo> tFunc2 = (blockInfo) =>
                    {
                        blockInfo.IN.ExceptWith(blockInfo.HelpFirst);
                        blockInfo.IN.UnionWith(blockInfo.HelpSecond);
                        return blockInfo;
                    };

                    var transferFunction1 = new TransferFunction<BlockInfo>(tFunc1);
                    var transferFunction2 = new TransferFunction<BlockInfo>(tFunc2);

                    // создание объекта итерационного алгоритма
                    var iterativeAlgorithm = new IterativeAlgorithm(blocksInfo, controlFlowGraph, meetOperator,
                        false, new HashSet<string>(), new HashSet<string>(), transferFunction1 * transferFunction2);

                    // выполнение алгоритма
                    iterativeAlgorithm.Perform();

                    //var Out = new List<HashSet<string>>();
                    //foreach(var OUT in iterativeAlgorithm.)

                    // вычисление множеств IN, OUT на основе DefUse
                    var Out = new InOutActiveVariables(DefUse, controlFlowGraph).OutBlocks;

                    controlFlowGraph = ControlFlowOptimisations.DeadOrAliveOnGraph(iterativeAlgorithm.GetOUTs(), controlFlowGraph);

                    Console.WriteLine("\nПосле каскадного удаления мертвых переменных для графа\n" + controlFlowGraph);

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
