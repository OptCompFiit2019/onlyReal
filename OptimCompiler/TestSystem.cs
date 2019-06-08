using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using SimpleLang.ThreeCodeOptimisations;
using SimpleLang.Visitors;
using SimpleLang.AstOptimisations;
using SimpleScanner;
using SimpleParser;
using CFG = SimpleLang.ControlFlowGraph.ControlFlowGraph;
using SimpleLang.Block;
using System.Text.RegularExpressions;
using SimpleLang.ControlFlowGraph;
using SimpleLang.GenericIterativeAlgorithm;
using GenericTransferFunction;
using ProgramTree;
using SimpleLang.Optimisations;

namespace SimpleLang
{
    class TestSystem
    {
        private void MakeIterativeAlgorithmOptimization(ref CFG controlFlowGraph)
        {
            // создание информации о блоках
            var blocksInfo = new List<BlockInfo>();

            // вычисление множеств Def и Use для всего графа потоков данных
            var DefUse = new DefUseBlocks(controlFlowGraph);
            // создание информации о блоках

            for (int l = 0; l < DefUse.DefBs.Count; l++)
                blocksInfo.Add(new BlockInfo(DefUse.DefBs[l], DefUse.UseBs[l]));

            // оператор сбора для анализа активных переменных
            Func<List<BlockInfo>, CFG, int, BlockInfo> meetOperator = (blocksInfos, graph, index) =>
            {
                var successorIndexes = graph.cfg.GetOutputNodes(index);
                var resInfo = new BlockInfo(blocksInfos[index]);
                foreach (var l in successorIndexes)
                    resInfo.OUT.UnionWith(blocksInfos[l].IN);
                return resInfo;
            };

            // делегат передаточной функции для анализа активных переменных
            Func<BlockInfo, BlockInfo> tFunc = (blockInfo) =>
            {
                blockInfo.IN = new HashSet<string>();
                blockInfo.IN.UnionWith(blockInfo.OUT);
                blockInfo.IN.ExceptWith(blockInfo.HelpFirst);
                blockInfo.IN.UnionWith(blockInfo.HelpSecond);
                return blockInfo;
            };

            var transferFunction = new TransferFunction<BlockInfo>(tFunc);

            // создание объекта итерационного алгоритма
            var iterativeAlgorithm = new IterativeAlgorithm(blocksInfo, controlFlowGraph, meetOperator,
            false, new HashSet<string>(), new HashSet<string>(), transferFunction);

            // выполнение алгоритма - вычисление IN и OUT
            iterativeAlgorithm.Perform();

            controlFlowGraph = ControlFlowOptimisations.DeadOrAliveOnGraph(iterativeAlgorithm.GetOUTs(), controlFlowGraph); // выполнение оптимизации
        }

        private void MakeTransferFunctionOptimization(ref CFG controlFlowGraph)
        {
            // создание информации о блоках
            var blocksInfo = new List<BlockInfo>();

            // вычисление множеств Def и Use для всего графа потоков данных
            var DefUse = new DefUseBlocks(controlFlowGraph);
            // создание информации о блоках

            for (int i = 0; i < DefUse.DefBs.Count; i++)
                blocksInfo.Add(new BlockInfo(DefUse.DefBs[i], DefUse.UseBs[i]));

            // оператор сбора для анализа активных переменных
            Func<List<BlockInfo>, CFG, int, BlockInfo> meetOperator = (blocksInfos, graph, index) =>
            {
                var successorIndexes = graph.cfg.GetOutputNodes(index);
                var resInfo = new BlockInfo(blocksInfos[index]);
                foreach (var i in successorIndexes)
                    resInfo.OUT.UnionWith(blocksInfos[i].IN);
                return resInfo;
            };

            // делегат передаточной функции для анализа активных переменных
            Func<BlockInfo, BlockInfo> tFunc = (blockInfo) =>
            {
                blockInfo.IN = new HashSet<string>();
                blockInfo.IN.UnionWith(blockInfo.OUT);
                blockInfo.IN.ExceptWith(blockInfo.HelpFirst);
                blockInfo.IN.UnionWith(blockInfo.HelpSecond);
                return blockInfo;
            };

            var transferFunction = new TransferFunction<BlockInfo>(tFunc);

            // создание объекта итерационного алгоритма
            var iterativeAlgorithm = new IterativeAlgorithm(blocksInfo, controlFlowGraph, meetOperator,
            false, new HashSet<string>(), new HashSet<string>(), transferFunction);

            // выполнение алгоритма - вычисление IN и OUT
            iterativeAlgorithm.Perform();

            controlFlowGraph = ControlFlowOptimisations.DeadOrAliveOnGraph(iterativeAlgorithm.GetOUTs(), controlFlowGraph); // выполнение оптимизации
        }

        private ThreeAddressCodeVisitor GetThreeAddressCodeVisitor(string path, string fileName)
        {
            string text = File.ReadAllText(path + fileName);
            Scanner scanner = new Scanner();
            scanner.SetSource(text, 0);
            SimpleParser.SymbolTable.vars = new Dictionary<string, type>();
            Parser parser = new Parser(scanner);

            var b = parser.Parse();
            if (!b)
            {
                Console.WriteLine("Ошибка при парсинге программы из файла: {0}", fileName);
                return null;
            }

            var r = parser.root;

            FillParentVisitor generateParrent = new FillParentVisitor();
            r.Visit(generateParrent);

            ThreeAddressCodeVisitor treeCode = new ThreeAddressCodeVisitor();
            r.Visit(treeCode);
            return treeCode;
        }

        private Node GetRootOfAST(string path)
        {
            string text = File.ReadAllText(path);
            Scanner scanner = new Scanner();
            scanner.SetSource(text, 0);
            SimpleParser.SymbolTable.vars = new Dictionary<string, type>();
            Parser parser = new Parser(scanner);

            var b = parser.Parse();
            if (!b)
            {
                Console.WriteLine("Ошибка при парсинге программы из файла: {0}", path);
            }

            return parser.root;
        }

        private string DeleteEmptyLines(string text)
        {
            var lines = text.Split('\n').Where(x => x != "\r").ToList();
            string result = lines[0];
            for (int i = 1; i < lines.Count; i++)
                result += "\n" + lines[i];
            return result;
        }
        //Функция для построчного сравнения ожидаемого результата(target) и программы после оптимизации(optimized_program)
        //source - имя файла с тестомб block_num - номер проверяемого базового блока
        private void CheckResults(List<ThreeCode> optimized_program, List<ThreeCode> target, ref bool flag, ref bool testFault, string source, int block_num = 1)
        {
            if (optimized_program.Count != target.Count)
            {
                Console.WriteLine("Тест {0} не пройден: кол-во команд в ББл #{1} не совпадает с ожидаемым", source, block_num);
                Console.WriteLine("Получилось:");
                foreach (var e in optimized_program)
                {
                    Console.WriteLine(e);
                }
                Console.WriteLine("-----");
                Console.WriteLine("Ожидалось:");
                foreach (var e in target)
                {
                    Console.WriteLine(e);
                }
                flag = false;
                return;
            }
            else
            {
                for (int k = 0; k < optimized_program.Count; k++)
                    if (!optimized_program.ElementAt(k).operation.ToString().Contains("Logic") &&
                        optimized_program.ElementAt(k).operation.ToString() != "IfGoto" &&
                        optimized_program.ElementAt(k).ToString() != target.ElementAt(k).ToString())
                    {
                        Console.WriteLine("Тест {0} не пройден: ошибка в строке #{1} в ББл #{2}", source, k + 1, block_num);
                        Console.WriteLine("Получилось:");
                        foreach (var e in optimized_program)
                        {
                            Console.WriteLine(e);
                        }
                        Console.WriteLine("-----");
                        Console.WriteLine("Ожидалось:");
                        foreach (var e in target)
                        {
                            Console.WriteLine(e);
                        }

                        flag = false;
                        testFault = true;
                        break;
                    }
                if (testFault)
                    return;
            }
        }

        private List<ThreeCode> ParseThreeCode(string path)  //TODO: добавить в парсер поиск кода для команды println( operatop=Printl, arg1 = val, остальное null)
        {
            var text = File.ReadAllLines(path);
            List<ThreeCode> program = new List<ThreeCode>();
            var label_rx = new Regex(@"\s*(label_\d{1}):.*");
            var if_rx = new Regex(@"\s*if (\S+) goto (label_\d+).*");
            var goto_rx = new Regex(@"\s*goto\s+(label_\d)");
            var binop_rx = new Regex(@"\s*(\w[\w\d]*)\s*=\s*(\w[\w\d.,]*)\s*([+\-*\/<>=!]{1,2})\s*(\w[\w\d.,]*)]?");
            var assign_rx = new Regex(@"\s*(\w[\w\d]*)\s*=\s*([\w\d.,]+)\s*");
            var println_rx = new Regex(@"\s*println\s+([\w\d.]+).*");

            foreach (var line in text)
            {
                string label, result, arg1, arg2;
                ThreeOperator operation;

                Match match = label_rx.Match(line);
                label = match.Groups[1].Value;

                if (line.Contains("println"))
                {
                    match = println_rx.Match(line);
                    arg1 = match.Groups[1].Value;
                    arg2 = null;
                    operation = ThreeOperator.Println;
                    result = "";
                }
                else if (line.Contains("if"))
                {
                    match = if_rx.Match(line);
                    arg1 = match.Groups[1].Value;
                    arg2 = match.Groups[2].Value;
                    operation = ThreeOperator.IfGoto;
                    result = "";
                }

                else if (line.Contains("goto"))
                {
                    match = goto_rx.Match(line);
                    arg1 = match.Groups[1].Value;
                    operation = ThreeOperator.Goto;
                    arg2 = null;
                    result = "";
                }
                else
                {
                    match = binop_rx.Match(line);
                    if (match.Success)
                    {
                        result = match.Groups[1].Value;
                        arg1 = match.Groups[2].Value;
                        arg2 = match.Groups[4].Value;
                        switch (match.Groups[3].Value)
                        {
                            case "+":
                                operation = ThreeOperator.Plus;
                                break;
                            case "-":
                                operation = ThreeOperator.Minus;
                                break;
                            case "*":
                                operation = ThreeOperator.Mult;
                                break;
                            case "/":
                                operation = ThreeOperator.Div;
                                break;
                            case "<":
                                operation = ThreeOperator.Logic_less;
                                break;
                            case "<=":
                                operation = ThreeOperator.Logic_leq;
                                break;
                            case ">":
                                operation = ThreeOperator.Logic_greater;
                                break;
                            case ">=":
                                operation = ThreeOperator.Logic_geq;
                                break;
                            case "==":
                                operation = ThreeOperator.Logic_equal;
                                break;
                            case "!=":
                                operation = ThreeOperator.Logic_neq;
                                break;
                            default:
                                operation = ThreeOperator.Assign;
                                break;
                        }
                    }
                    else
                    {
                        match = assign_rx.Match(line);
                        if (match.Success)
                        {
                            result = match.Groups[1].Value;
                            arg1 = match.Groups[2].Value;
                            arg2 = null;
                            operation = ThreeOperator.Assign;
                        }
                        else
                        {
                            result = null;
                            arg1 = null;
                            arg2 = null;
                            operation = ThreeOperator.None;
                        }
                    }
                }
                //Стандратный метод ToString() для типа double печатает вещественные числа с запятой, поэтому в нашем трёхадресном коде меняем точки на запятые
                if (arg1 != null)
                    arg1 = arg1.Replace('.', ',');
                if (arg2 != null)
                    arg2 = arg2.Replace('.', ',');
                program.Add(new ThreeCode(label, result, operation, new ThreeAddressStringValue(arg1), new ThreeAddressStringValue(arg2)));
            }
            return program;
        }

        private void LaunchASTOptTest(string pathToFolder, string source, string expectation, Visitor v)
        {
            var source_root = GetRootOfAST(pathToFolder + source);

            source_root.Visit(new FillParentVisitor());
            source_root.Visit(v);

            var source_print = new PrettyPrintVisitor();
            source_root.Visit(source_print);

            var expectation_root = GetRootOfAST(pathToFolder + expectation);
            var expectation_print = new PrettyPrintVisitor();
            expectation_root.Visit(expectation_print);

            string opt_program = DeleteEmptyLines(source_print.Text);
            string target = DeleteEmptyLines(expectation_print.Text);

            if (opt_program == target)
                Console.WriteLine("Тест {0} успешно пройден!", source);
            else
                Console.WriteLine("Тест {0} не пройден!\nПолучено:\n{1}\n\nОжидалось:\n{2}\n\n", source, opt_program, target);
        }
        public void LaunchTest(string testName)
        {
            string pathToFolder = string.Format(@"../../../Tests/{0}/", testName);
            Console.WriteLine("\nТестирование {0}\n", testName);
            int numberOfTests = (new DirectoryInfo(pathToFolder).GetFiles().Length - 1) / 2;
            for (int i = 1; i <= numberOfTests; i++)
            {
                string source = string.Format("source{0}.txt", i);
                string expectation = string.Format("expectation{0}.txt", i);

                switch (testName)
                {
                    case "DAG":  //qwerty
                        var sourceCode = GetThreeAddressCodeVisitor(pathToFolder, source);
                        var sourceBlocks = new Block.Block(sourceCode).GenerateBlocks();
                        var expectationCode = GetThreeAddressCodeVisitor(pathToFolder, expectation);
                        var expectationBlocks = new Block.Block(expectationCode).GenerateBlocks();
                        bool flag = true;
                        for (int j = 0; j < sourceBlocks.Count; j++)
                        {
                            bool _apply = false;
                            bool testFault = false;
                            var optimizer = new DAG(sourceBlocks[j].ToList());
                            var opt_program = optimizer.Optimize(ref _apply);
                            var target = optimizer.MakeProgram(expectationBlocks[j].ToList());
                            CheckResults(opt_program.ToList(), target, ref flag, ref testFault, source, j);
                        }
                        if (flag)
                            Console.WriteLine("Тест {0} успешно пройден!", source);
                        break;

                    case "PullCopies":  //qwerty
                        sourceCode = GetThreeAddressCodeVisitor(pathToFolder, source);
                        sourceBlocks = new Block.Block(sourceCode).GenerateBlocks();
                        expectationCode = GetThreeAddressCodeVisitor(pathToFolder, expectation);
                        expectationBlocks = new Block.Block(expectationCode).GenerateBlocks();
                        flag = true;
                        for (int j = 0; j < sourceBlocks.Count; j++)
                        {
                            bool _apply = false;
                            bool testFault = false;
                            var optimizer = new PullCopies(sourceBlocks[j].ToList());

                            var opt_program = optimizer.Optimize(ref _apply);
                            var target = expectationBlocks[j].ToList();
                            CheckResults(opt_program.ToList(), target, ref flag, ref testFault, source, j);
                        }
                        if (flag)
                            Console.WriteLine("Тест {0} успешно пройден!", source);
                        break;

                    case "CommonExpr":  //qwerty
                        sourceCode = GetThreeAddressCodeVisitor(pathToFolder, source);
                        sourceBlocks = new Block.Block(sourceCode).GenerateBlocks();
                        expectationCode = GetThreeAddressCodeVisitor(pathToFolder, expectation);
                        expectationBlocks = new Block.Block(expectationCode).GenerateBlocks();
                        flag = true;
                        for (int j = 0; j < sourceBlocks.Count; j++)
                        {
                            bool _apply = false;
                            bool testFault = false;
                            var optimizer = new CommonExpr(sourceBlocks[j].ToList());

                            var opt_program = optimizer.Optimize(ref _apply);
                            var target = optimizer.MakeProgram(expectationBlocks[j].ToList());
                            CheckResults(opt_program.ToList(), target, ref flag, ref testFault, source, j);
                        }
                        if (flag)
                            Console.WriteLine("Тест {0} успешно пройден!", source);
                        break;

                    case "DefBUseB":  //qwerty
                        sourceCode = GetThreeAddressCodeVisitor(pathToFolder, source);
                        sourceBlocks = new Block.Block(sourceCode).GenerateBlocks();
                        flag = true;
                        CFG controlFlowGraph = new CFG(sourceBlocks);
                        var DefUse = new DefUseBlocks(controlFlowGraph);
                        var expectationSets = File.ReadAllLines(pathToFolder + expectation);

                        if (expectationSets.Length != 3 * sourceBlocks.Count)
                        {

                            Console.WriteLine("Тест {0} не пройден: кол-во ББл не совпадает с ожидаемым", source);
                            flag = false;
                            break;
                        }
                        else
                        {
                            for (int k = 0; k < expectationSets.Length; k += 3)
                            {
                                bool testFault = false;
                                var b = int.Parse(expectationSets[k].Substring(5)) - 1;
                                var def = expectationSets[k + 1].Split(' ');
                                var use = expectationSets[k + 2].Split(' ');

                                if (def.Length - 1 != DefUse.DefBs[b].Count)
                                {
                                    Console.WriteLine("Тест {0} не пройден: кол-во переменных мн-ва DefB в ББл #{1} не совпадает с ожидаемым", source, b + 1);
                                    flag = false;
                                    break;
                                }

                                if (use.Length - 1 != DefUse.UseBs[b].Count)
                                {
                                    Console.WriteLine("Тест {0} не пройден: кол-во переменных мн-ва UseB в ББл #{1} не совпадает с ожидаемым", source, b + 1);
                                    flag = false;
                                    break;
                                }

                                for (int m = 1; m < def.Length; m++)
                                {
                                    if (def[m] != DefUse.DefBs[b].ElementAt(m - 1))
                                    {
                                        Console.WriteLine("Тест {0} не пройден: ошибка во множестве DefB в ББл #{1}", source, b + 1);
                                        flag = false;
                                        testFault = true;
                                        break;
                                    }
                                }
                                if (testFault)
                                    break;
                                for (int m = 1; m < use.Length; m++)
                                {
                                    if (use[m] != DefUse.UseBs[b].ElementAt(m - 1))
                                    {
                                        Console.WriteLine("Тест {0} не пройден: ошибка во множестве UseB в ББл #{1}", source, b + 1);
                                        flag = false;
                                        testFault = true;
                                        break;
                                    }
                                }
                                if (testFault)
                                    break;
                            }
                        }
                        if (flag)
                            Console.WriteLine("Тест {0} успешно пройден!", source);
                        break;

                    case "JumpThroughJump":  //SouthPark
                        {
                            sourceCode = GetThreeAddressCodeVisitor(pathToFolder, source);
                            sourceBlocks = new Block.Block(sourceCode).GenerateBlocks();
                            var optimizer = new NonZero_JTJ(sourceCode);
                            optimizer.DeleteJumpThroughJump();
                            flag = true;

                            bool testFault = false;
                            var opt_program = optimizer.code.ToList();
                            var target = ParseThreeCode(pathToFolder + expectation);
                            CheckResults(opt_program.ToList(), target, ref flag, ref testFault, source);
                            if (flag)
                                Console.WriteLine("Тест {0} успешно пройден!", source);
                        }
                        break;

                    case "DeadOrAliveOnGraph":  //Roll
                        sourceCode = GetThreeAddressCodeVisitor(pathToFolder, source);
                        sourceBlocks = new Block.Block(sourceCode).GenerateBlocks();
                        expectationCode = GetThreeAddressCodeVisitor(pathToFolder, expectation);
                        expectationBlocks = new Block.Block(expectationCode).GenerateBlocks();

                        //добавление фиктивных блоков входа и выхода программы 
                        var entryPoint = new LinkedList<ThreeCode>();
                        entryPoint.AddLast(new ThreeCode("entry", "", ThreeOperator.None, null, null));
                        var exitPoint = new LinkedList<ThreeCode>();
                        exitPoint.AddLast(new ThreeCode("exit", "", ThreeOperator.None, null, null));
                        sourceBlocks.Insert(0, entryPoint);
                        sourceBlocks.Add(exitPoint);
                        expectationBlocks.Insert(0, entryPoint);
                        expectationBlocks.Add(exitPoint);

                        controlFlowGraph = new CFG(sourceBlocks); // построение графа
                        DefUse = new DefUseBlocks(controlFlowGraph); // нахождение Def,Use
                        InOutActiveVariables inOutActive = new InOutActiveVariables(DefUse, controlFlowGraph); // ИтА для активных переменных
                        controlFlowGraph = ControlFlowOptimisations.DeadOrAliveOnGraph(inOutActive.OutBlocks, controlFlowGraph); // оптимизация

                        flag = true;
                        for (int j = 0; j < controlFlowGraph.blocks.Count; j++)
                        {
                            bool testFault = false;

                            var opt_program = controlFlowGraph.blocks[j].ToList();
                            var target = expectationBlocks[j].ToList();
                            CheckResults(opt_program.ToList(), target, ref flag, ref testFault, source, j);
                            if (!flag)
                                break;
                        }
                        if (flag)
                            Console.WriteLine("Тест {0} успешно пройден!", source);
                        break;

                    case "DeadOrAliveOptimization":  //Roll
                        sourceCode = GetThreeAddressCodeVisitor(pathToFolder, source);
                        sourceBlocks = new Block.Block(sourceCode).GenerateBlocks();
                        expectationCode = GetThreeAddressCodeVisitor(pathToFolder, expectation);
                        expectationBlocks = new Block.Block(expectationCode).GenerateBlocks();

                        //добавление фиктивных блоков входа и выхода программы 
                        entryPoint = new LinkedList<ThreeCode>();
                        entryPoint.AddLast(new ThreeCode("entry", "", ThreeOperator.None, null, null));
                        exitPoint = new LinkedList<ThreeCode>();
                        exitPoint.AddLast(new ThreeCode("exit", "", ThreeOperator.None, null, null));
                        sourceBlocks.Insert(0, entryPoint);
                        sourceBlocks.Add(exitPoint);
                        expectationBlocks.Insert(0, entryPoint);
                        expectationBlocks.Add(exitPoint);

                        controlFlowGraph = new CFG(sourceBlocks); // построение графа
                        controlFlowGraph = DeadOrAliveOptimization.DeleteDeadVariables(controlFlowGraph); // оптимизация

                        flag = true;
                        for (int j = 0; j < controlFlowGraph.blocks.Count; j++)
                        {
                            bool testFault = false;

                            var opt_program = controlFlowGraph.blocks[j].ToList();
                            var target = expectationBlocks[j].ToList();
                            CheckResults(opt_program.ToList(), target, ref flag, ref testFault, source, j);
                            if (!flag)
                                break;
                        }
                        if (flag)
                            Console.WriteLine("Тест {0} успешно пройден!", source);
                        break;

                    case "GraphDepth":  //Roll
                        sourceCode = GetThreeAddressCodeVisitor(pathToFolder, source);
                        sourceBlocks = new Block.Block(sourceCode).GenerateBlocks();
                        string expectationText = File.ReadAllText(pathToFolder + expectation, Encoding.GetEncoding(1251));
                        Regex depth_rx = new Regex(@".*Глубина CFG:\s*(\d+).*");
                        Match match = depth_rx.Match(expectationText);

                        controlFlowGraph = new CFG(sourceBlocks);
                        int depth = GraphDepth.GetGraphDepth(controlFlowGraph);
                        int expectedDepth = int.Parse(match.Groups[1].Value);
                        if (expectedDepth == depth)
                            Console.WriteLine("Тест {0} успешно пройден!", source);
                        else
                            Console.WriteLine("Тест {0} не пройден. Ожидалось {1}; Получено {2}", source, expectedDepth, depth);
                        break;

                    case "IterativeAlgorithm":  //Roll
                        sourceCode = GetThreeAddressCodeVisitor(pathToFolder, source);
                        sourceBlocks = new Block.Block(sourceCode).GenerateBlocks();
                        expectationCode = GetThreeAddressCodeVisitor(pathToFolder, expectation);
                        expectationBlocks = new Block.Block(expectationCode).GenerateBlocks();

                        //добавление фиктивных блоков входа и выхода программы 
                        entryPoint = new LinkedList<ThreeCode>();
                        entryPoint.AddLast(new ThreeCode("entry", "", ThreeOperator.None, null, null));
                        exitPoint = new LinkedList<ThreeCode>();
                        exitPoint.AddLast(new ThreeCode("exit", "", ThreeOperator.None, null, null));
                        sourceBlocks.Insert(0, entryPoint);
                        sourceBlocks.Add(exitPoint);
                        expectationBlocks.Insert(0, entryPoint);
                        expectationBlocks.Add(exitPoint);

                        controlFlowGraph = new CFG(sourceBlocks);
                        MakeIterativeAlgorithmOptimization(ref controlFlowGraph);

                        flag = true;
                        for (int j = 0; j < controlFlowGraph.blocks.Count; j++)
                        {
                            bool testFault = false;

                            var opt_program = controlFlowGraph.blocks[j].ToList();
                            var target = expectationBlocks[j].ToList();
                            CheckResults(opt_program.ToList(), target, ref flag, ref testFault, source, j);
                            if (!flag)
                                break;
                        }
                        if (flag)
                            Console.WriteLine("Тест {0} успешно пройден!", source);
                        break;

                    case "LVNOptimization":  //Roll
                        sourceCode = GetThreeAddressCodeVisitor(pathToFolder, source);
                        sourceBlocks = new Block.Block(sourceCode).GenerateBlocks();
                        expectationCode = GetThreeAddressCodeVisitor(pathToFolder, expectation);
                        expectationBlocks = new Block.Block(expectationCode).GenerateBlocks();

                        controlFlowGraph = new CFG(sourceBlocks); // построение графа
                        controlFlowGraph = LVNOptimization.LVNOptimize(controlFlowGraph); // оптимизация

                        flag = true;
                        for (int j = 0; j < controlFlowGraph.blocks.Count; j++)
                        {
                            bool testFault = false;

                            var opt_program = controlFlowGraph.blocks[j].ToList();
                            var target = expectationBlocks[j].ToList();
                            CheckResults(opt_program.ToList(), target, ref flag, ref testFault, source, j);
                            if (!flag)
                                break;
                        }
                        if (flag)
                            Console.WriteLine("Тест {0} успешно пройден!", source);
                        break;

                    case "TransferFunction":  //Roll
                        sourceCode = GetThreeAddressCodeVisitor(pathToFolder, source);
                        sourceBlocks = new Block.Block(sourceCode).GenerateBlocks();
                        expectationCode = GetThreeAddressCodeVisitor(pathToFolder, expectation);
                        expectationBlocks = new Block.Block(expectationCode).GenerateBlocks();

                        //добавление фиктивных блоков входа и выхода программы 
                        entryPoint = new LinkedList<ThreeCode>();
                        entryPoint.AddLast(new ThreeCode("entry", "", ThreeOperator.None, null, null));
                        exitPoint = new LinkedList<ThreeCode>();
                        exitPoint.AddLast(new ThreeCode("exit", "", ThreeOperator.None, null, null));
                        sourceBlocks.Insert(0, entryPoint);
                        sourceBlocks.Add(exitPoint);
                        expectationBlocks.Insert(0, entryPoint);
                        expectationBlocks.Add(exitPoint);

                        controlFlowGraph = new CFG(sourceBlocks);
                        MakeTransferFunctionOptimization(ref controlFlowGraph);

                        flag = true;
                        for (int j = 0; j < controlFlowGraph.blocks.Count; j++)
                        {
                            bool testFault = false;

                            var opt_program = controlFlowGraph.blocks[j].ToList();
                            var target = expectationBlocks[j].ToList();
                            CheckResults(opt_program.ToList(), target, ref flag, ref testFault, source, j);
                            if (!flag)
                                break;
                        }
                        if (flag)
                            Console.WriteLine("Тест {0} успешно пройден!", source);
                        break;

                    case "ThreeAddressCode":  //Roslyn
                        {
                            sourceCode = GetThreeAddressCodeVisitor(pathToFolder, source);
                            //
                            SimpleParser.SymbolTable.vars = new Dictionary<string, type>();
                            //
                            string tt = System.IO.File.ReadAllText(pathToFolder + source);
                            SimpleScanner.Scanner scan = new SimpleScanner.Scanner();
                            scan.SetSource(tt, 0);
                            SimpleParser.Parser pars = new SimpleParser.Parser(scan);
                            var b = pars.Parse();
                            var r = pars.root;
                            SimpleLang.Visitors.FillParentVisitor parVisitor = new SimpleLang.Visitors.FillParentVisitor();
                            r.Visit(parVisitor);
                            SimpleLang.Visitors.ThreeAddressCodeVisitor threeCodeVisitor = new SimpleLang.Visitors.ThreeAddressCodeVisitor();
                            r.Visit(threeCodeVisitor);


                            flag = true;

                            bool testFault = false;
                            var opt_program = threeCodeVisitor.GetCode().ToList();
                            var target = ParseThreeCode(pathToFolder + expectation);
                            CheckResults(opt_program.ToList(), target, ref flag, ref testFault, source);
                            if (flag)
                                Console.WriteLine("Тест {0} успешно пройден!", source);
                        }
                        break;



                    case "ApplyAlgebraicIdentities":  //Roslyn
                        sourceCode = GetThreeAddressCodeVisitor(pathToFolder, source);
                        sourceBlocks = new Block.Block(sourceCode).GenerateBlocks();
                        expectationCode = GetThreeAddressCodeVisitor(pathToFolder, expectation);
                        expectationBlocks = new Block.Block(expectationCode).GenerateBlocks();

                        flag = true;
                        for (int j = 0; j < sourceBlocks.Count; j++)
                        {
                            bool testFault = false;
                            ApplyAlgebraicIdentities dist = new ApplyAlgebraicIdentities();
                            var sb = sourceBlocks.ElementAt(j);
                            dist.Apply(ref sb);

                            var opt_program = sb.ToList();
                            var target = expectationBlocks.ElementAt(j).ToList();
                            CheckResults(opt_program, target, ref flag, ref testFault, source, j);
                        }
                        if (flag)
                            Console.WriteLine("Тест {0} успешно пройден!", source);
                        break;

                    case "DistributionOfConstant":  //Roslyn
                        sourceCode = GetThreeAddressCodeVisitor(pathToFolder, source);
                        sourceBlocks = new Block.Block(sourceCode).GenerateBlocks();
                        expectationCode = GetThreeAddressCodeVisitor(pathToFolder, expectation);
                        expectationBlocks = new Block.Block(expectationCode).GenerateBlocks();

                        flag = true;
                        for (int j = 0; j < sourceBlocks.Count; j++)
                        {
                            bool testFault = false;
                            DistributionOfConstants dist = new DistributionOfConstants();
                            var sb = sourceBlocks.ElementAt(j);
                            dist.Apply(ref sb);

                            var opt_program = sb.ToList();
                            var target = expectationBlocks.ElementAt(j).ToList();
                            CheckResults(opt_program, target, ref flag, ref testFault, source, j);
                        }
                        if (flag)
                            Console.WriteLine("Тест {0} успешно пройден!", source);
                        break;

                    case "EvalConstExpr":  //Roslyn
                        sourceCode = GetThreeAddressCodeVisitor(pathToFolder, source);
                        sourceBlocks = new Block.Block(sourceCode).GenerateBlocks();
                        expectationCode = GetThreeAddressCodeVisitor(pathToFolder, expectation);
                        expectationBlocks = new Block.Block(expectationCode).GenerateBlocks();

                        flag = true;
                        for (int j = 0; j < sourceBlocks.Count; j++)
                        {
                            bool testFault = false;
                            EvalConstExpr dist = new EvalConstExpr();
                            var sb = sourceBlocks.ElementAt(j);
                            dist.Apply(ref sb);

                            var opt_program = sb.ToList();
                            var target = expectationBlocks.ElementAt(j).ToList();
                            CheckResults(opt_program, target, ref flag, ref testFault, source, j);
                        }
                        if (flag)
                            Console.WriteLine("Тест {0} успешно пройден!", source);
                        break;

                    case "DeepSpanTree": //qwerty + SouthPark
                        flag = true;
                        sourceCode = GetThreeAddressCodeVisitor(pathToFolder, source);
                        CFG controlFlowGraph1 = new CFG(sourceCode);
                        var st = new SpanTree(controlFlowGraph1);
                        var edges = st.buildSpanTree();
                        var expectationEdges = File.ReadAllLines(pathToFolder + expectation);
                        if (expectationEdges.Length != edges.Count)
                        {
                            Console.WriteLine("Тест {0} не пройден: кол-во ребер не совпадает с ожидаемым", source);
                            flag = false;
                            break;
                        }
                        else
                        {
                            for (int k = 0; k < expectationEdges.Length; k++)
                            {
                                bool testFault = false;
                                var e = expectationEdges[k].Split(' ');
                                var v1 = int.Parse(e[0].Substring(3));
                                var v2 = int.Parse(e[1].Substring(3));
                                var t = e[2].Substring(5);

                                if (v1 != edges.ElementAt(k).v1.num || v2 != edges.ElementAt(k).v2.num)
                                {
                                    Console.WriteLine("Тест {0} не пройден: получившееся ребро {1}-{2} не совпадает с ожидаемым {3}-{4}", source, edges.ElementAt(k).v1.num, edges.ElementAt(k).v2.num, v1, v2);
                                    flag = false;
                                    testFault = true;
                                    break;
                                }
                                if (testFault)
                                    break;
                                if (t != edges.ElementAt(k).type.ToString())
                                {
                                    Console.WriteLine("Тест {0} не пройден: тип ребра {1}-{2} {3} не совпадает с ожидаемым {4}", source, v1, v2, edges.ElementAt(k).type, t);
                                    flag = false;
                                    testFault = true;
                                    break;
                                }
                                if (testFault)
                                    break;
                            }
                        }
                        if (flag)
                            Console.WriteLine("Тест {0} успешно пройден!", source);
                        break;

                    case "AvailableExprs":  //SouthPark
                        sourceCode = GetThreeAddressCodeVisitor(pathToFolder, source);
                        sourceBlocks = new Block.Block(sourceCode).GenerateBlocks();
                        expectationText = File.ReadAllText(pathToFolder + expectation);
                        var genB_blocks = new Regex(@"E_genB\n*([\s\S]*)\n+E_killB").Match(expectationText).Groups[1].Value.Split('\n').Where(x => x.Contains("Block")).ToList(); //получили список блоков с описанием genB
                        var killB_blocks = new Regex(@"E_killB\n*([\s\S]*)\n+Transfer function").Match(expectationText).Groups[1].Value.Split('\n').Where(x => x.Contains("Block")).ToList(); //получили список блоков с описанием killB
                        var transfer_func_blocks = new Regex(@"Transfer function\n*([\s\S]*)\n*").Match(expectationText).Groups[1].Value.Split('\n').Where(x => x.Contains("Block")).ToList(); //получили список блоков с описанием Transfer function


                        var tuple_rx = new Regex(@"Block\s*(\d+)\s*:\s*(\([\S ]+\))");
                        var kill_rx = new Regex(@"Block\s*\d+:([\S\s]*)");
                        var parse_tuple_rx = new Regex(@"(\((\S+)\s*,\s*(\S+),\s*(\S+)\))");

                        var GenB = new List<List<Tuple<string, string, string>>>();
                        var KillB = new List<List<string>>();
                        var TransferFunction = new List<List<Tuple<string, string, string>>>();

                        for (int l = 0; l < sourceBlocks.Count; l++)
                        {
                            var gen_block = tuple_rx.Matches(genB_blocks[l]);
                            GenB.Add(new List<Tuple<string, string, string>>());
                            foreach (Match tuple in gen_block)
                            {
                                var tuples_in_block = parse_tuple_rx.Matches(tuple.Value);
                                foreach (Match temp in tuples_in_block)
                                    GenB[l].Add(Tuple.Create(temp.Groups[2].Value, temp.Groups[3].Value, temp.Groups[4].Value));
                            }

                            var kill_block = kill_rx.Match(killB_blocks[l]);
                            KillB.Add(kill_rx.Match(killB_blocks[l]).Groups[1].Value.Split(new char[] { ' ', '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList());

                            var transfer_block = tuple_rx.Matches(transfer_func_blocks[l]);
                            TransferFunction.Add(new List<Tuple<string, string, string>>());
                            foreach (Match tuple in transfer_block)
                            {
                                var tuples_in_block = parse_tuple_rx.Matches(tuple.Value);
                                foreach (Match temp in tuples_in_block)
                                    TransferFunction[l].Add(Tuple.Create(temp.Groups[2].Value, temp.Groups[3].Value, temp.Groups[4].Value));
                            }
                        }

                        var ae = new AvaliableExprs();
                        var rr = ae.GetGenAndKillerSets(sourceBlocks);
                        var gen = rr.Item1;
                        var kill = rr.Item2;
                        flag = true;
                        for (int j = 0; j < sourceBlocks.Count; j++)
                        {
                            if (!flag)
                                break;
                            var ttt = ae.TransferByGenAndKiller(rr.Item1[j], rr.Item1[j], rr.Item2[j]);
                            if (GenB[j].Count != gen[j].Count)
                            {
                                Console.WriteLine("Тест {0} не пройден: неверная мощность множества GenB:\nПолучено {1} Ожидалось {2}", source, gen[j].Count, GenB[j].Count);
                                flag = false;
                            }
                            else
                            {
                                for (int l = 0; l < gen[j].Count; l++)
                                    if (GenB[j][l].Item1 != gen[j].ToList()[l].Item1 || GenB[j][l].Item2 != gen[j].ToList()[l].Item2 || GenB[j][l].Item3 != gen[j].ToList()[l].Item3)
                                    {
                                        Console.WriteLine("Тест {0} не пройден: ошибка в множестве GenB в блоке № {1}", source, l);
                                        flag = false;
                                        break;
                                    }
                            }

                            kill[j].RemoveWhere(s => s == "");
                            if (KillB[j].Count != kill[j].Count)
                            {
                                Console.WriteLine("Тест {0} не пройден: неверная мощность множества KillB:\nПолучено {1} Ожидалось {2}", source, kill[j].Count, KillB[j].Count);
                                flag = false;
                            }
                            else
                            {
                                for (int l = 0; l < kill[j].Count; l++)
                                    if (!KillB[j][l].Equals(kill[j].ToList()[l]))
                                    {
                                        Console.WriteLine("Тест {0} не пройден: ошибка в множестве KillB в блоке № {1}", source, l);
                                        flag = false;
                                        break;
                                    }
                            }

                            if (TransferFunction[j].Count != ttt.Count)
                            {
                                Console.WriteLine("Тест {0} не пройден: неверная мощность множества Transfer Function:\nПолучено {1} Ожидалось {2}", source, ttt.Count, TransferFunction[j].Count);
                                flag = false;
                            }
                            else
                            {
                                for (int l = 0; l < ttt.Count; l++)
                                    if (TransferFunction[j][l].Item1 != ttt.ToList()[l].Item1 || TransferFunction[j][l].Item2 != ttt.ToList()[l].Item2 || TransferFunction[j][l].Item3 != ttt.ToList()[l].Item3)
                                    {
                                        Console.WriteLine("Тест {0} не пройден: ошибка в множестве TransferFunction в блоке № {1}", source, l);
                                        flag = false;
                                        break;
                                    }
                            }
                        }
                        if (flag)
                            Console.WriteLine("Тест {0} успешно пройден!", source);
                        break;

                    case "PullOfCopies (Intel)":  //Intel
                        {
                            sourceCode = GetThreeAddressCodeVisitor(pathToFolder, source);
                            expectationCode = GetThreeAddressCodeVisitor(pathToFolder, expectation);

                            flag = true;
                            var pc = new PullOfCopies(sourceCode);
                            pc.PullCopies();
                            var opt_program = pc.Program.ToList();
                            var target = expectationCode.GetCode().ToList();
                            bool testFault = false;
                            CheckResults(opt_program, target, ref flag, ref testFault, source, 1);
                            if (flag)
                                Console.WriteLine("Тест {0} успешно пройден!", source);
                        }
                        break;

                    case "DeleteOfDeadCode (Intel)":  //Intel
                        {
                            sourceCode = GetThreeAddressCodeVisitor(pathToFolder, source);
                            expectationCode = GetThreeAddressCodeVisitor(pathToFolder, expectation);

                            flag = true;
                            var ddc = new DeleteOfDeadCode(sourceCode);
                            ddc.DeleteDeadCode();
                            var opt_program = ddc.Program.ToList();
                            var target = expectationCode.GetCode().ToList();
                            bool testFault = false;
                            CheckResults(opt_program, target, ref flag, ref testFault, source, 1);
                            if (flag)
                                Console.WriteLine("Тест {0} успешно пройден!", source);
                        }
                        break;

                    case "DefUse":  //komanda
                        sourceCode = GetThreeAddressCodeVisitor(pathToFolder, source);
                        sourceBlocks = new Block.Block(sourceCode).GenerateBlocks();
                        expectationCode = GetThreeAddressCodeVisitor(pathToFolder, expectation);
                        expectationBlocks = new Block.Block(expectationCode).GenerateBlocks();

                        AutoThreeCodeOptimiser app = new AutoThreeCodeOptimiser();
                        app.Add(new DefUseConstOpt());
                        if (sourceBlocks.Count == 1) app.Add(new DefUseDeadCodeOpt());
                        var blocks = app.Apply(sourceCode);

                        flag = true;
                        for (int j = 0; j < sourceBlocks.Count; j++)
                        {
                            bool testFault = false;

                            var opt_program = blocks[j].ToList();
                            var target = expectationBlocks.ElementAt(j).ToList();
                            CheckResults(opt_program, target, ref flag, ref testFault, source, j);
                        }
                        if (flag)
                            Console.WriteLine("Тест {0} успешно пройден!", source);
                        break;

                    case "UnreachableCodeOpt": //Boom
                        {
                            sourceCode = GetThreeAddressCodeVisitor(pathToFolder, source);
                            sourceBlocks = new Block.Block(sourceCode).GenerateBlocks();
                            var optimizer = new UnreachableCodeOpt();
                            flag = true;

                            bool testFault = false;
                            var opt_program = optimizer.DeleteUnreachCode(sourceCode.GetCode());
                            var target = ParseThreeCode(pathToFolder + expectation);
                            CheckResults(opt_program.ToList(), target, ref flag, ref testFault, source);
                            if (flag)
                                Console.WriteLine("Тест {0} успешно пройден!", source);
                        }
                        break;

                    case "AST_Opt8Visitor":  //qwerty
                        {
                            LaunchASTOptTest(pathToFolder, source, expectation, new OptVisitor_8());
                        }
                        break;

                    case "AST_Opt13Visitor":  //qwerty
                        {
                            LaunchASTOptTest(pathToFolder, source, expectation, new OptVisitor_13());
                        }
                        break;

                    case "AST_OptWhileVisitor":  //Roll
                        {
                            LaunchASTOptTest(pathToFolder, source, expectation, new OptWhileVisitor());
                        }
                        break;

                    case "AST_OptMulDivOneVisitor":  //Roll
                        {
                            LaunchASTOptTest(pathToFolder, source, expectation, new OptMulDivOneVisitor());
                        }
                        break;

                    case "AST_LinearizeBlocks":  //SouthPark
                        {
                            LaunchASTOptTest(pathToFolder, source, expectation, new LinearizeBlocks());
                        }
                        break;

                    case "AST_PlusNonZero":  //SouthPark
                        {
                            LaunchASTOptTest(pathToFolder, source, expectation, new PlusNonZero());
                        }
                        break;

                    case "AST_Opt2Visitor":  //Roslyn
                        {
                            LaunchASTOptTest(pathToFolder, source, expectation, new Opt2Visitor());
                        }
                        break;

                    case "AST_Opt11Visitor":  //Roslyn
                        {
                            LaunchASTOptTest(pathToFolder, source, expectation, new Opt11Visitor());
                        }
                        break;

                    case "AST_Opt7Visitor":  //Intel
                        {
                            LaunchASTOptTest(pathToFolder, source, expectation, new Opt7Visitor());
                        }
                        break;

                    case "AST_Opt12Visitor":  //Intel
                        {
                            LaunchASTOptTest(pathToFolder, source, expectation, new Opt12Visitor());
                        }
                        break;

                    case "AST_AssignVisitor":  //Boom
                        {
                            LaunchASTOptTest(pathToFolder, source, expectation, new AssignVisitor());
                        }
                        break;

                    case "AST_DeleteNullVisitor":  //Boom
                        {
                            LaunchASTOptTest(pathToFolder, source, expectation, new DeleteNullVisitor());
                        }
                        break;

                    case "AST_FalseExprMoreAndNonEqualVisitor": //komanda
                        {
                            LaunchASTOptTest(pathToFolder, source, expectation, new FalseExprMoreAndNonEqualVisitor());
                        }
                        break;

                    case "AST_MultiplicationComputeVisitor": //komanda
                        {
                            LaunchASTOptTest(pathToFolder, source, expectation, new MultiplicationComputeVisitor());
                        }
                        break;

                    case "AST_IfFalseVisitor": //komanda
                        {
                            LaunchASTOptTest(pathToFolder, source, expectation, new IfFalseVisitor());
                        }
                        break;

                    case "AST_ElseStVisitor": //GreatBean
                        {
                            LaunchASTOptTest(pathToFolder, source, expectation, new ElseStVisitor());
                        }
                        break;

                    case "AST_LessOptVisitor": //GreatBean
                        {
                            LaunchASTOptTest(pathToFolder, source, expectation, new LessOptVisitor());
                        }
                        break;

                    case "AST_Opt5SimilarDifference": //Nvidia
                        {
                            LaunchASTOptTest(pathToFolder, source, expectation, new OptSimilarDifference());
                        }
                        break;

                    case "AST_Opt10SimilarAssignment": //Nvidia
                        {
                            LaunchASTOptTest(pathToFolder, source, expectation, new OptSimilarAssignment());
                        }
                        break;

                    default:
                        break;


                }
            }
        }
    }
}


