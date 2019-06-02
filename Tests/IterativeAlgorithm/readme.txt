IterativeAlgorithm
Позволяет реализовать множество итерационных алгоритмов, описывая их общую структуру.
Ниже пример оптимизации (удаление мертвых переменных на основе ИтА для активных переменных) с помощью классов передаточной функции и обобщенного ИтА.

using SimpleLang.GenericIterativeAlgorithm;
using GenericTransferFunction;


CFG controlFlowGraph = new CFG(blocks);

using SimpleLang.GenericIterativeAlgorithm;
using GenericTransferFunction;


CFG controlFlowGraph = new CFG(blocks);

                    // создание информации о блоках
                    var blocksInfo = new List<BlockInfo<string>>();

                    // вычисление множеств Def и Use для всего графа потоков данных
                    var DefUse = new DefUseBlocks(controlFlowGraph);
                    // создание информации о блоках

                    for (int i = 0; i < DefUse.DefBs.Count; i++)
                        blocksInfo.Add(new BlockInfo<string>(DefUse.DefBs[i], DefUse.UseBs[i]));

                    // оператор сбора для анализа активных переменных
                    Func<List<BlockInfo<string>>, CFG, int, BlockInfo<string>> meetOperator = (blocksInfos, graph, index) =>
                    {
                        var successorIndexes = graph.cfg.GetOutputNodes(index);
                        var resInfo = new BlockInfo<string>(blocksInfos[index]);
                        foreach (var i in successorIndexes)
                            resInfo.OUT.UnionWith(blocksInfos[i].IN);
                        return resInfo;
                    };

                    // делегат передаточной функции для анализа активных переменных
                    Func<BlockInfo<string>, BlockInfo<string>> tFunc = (blockInfo) =>
                    {
                        blockInfo.IN = new HashSet<string>();
                        blockInfo.IN.UnionWith(blockInfo.OUT);
                        blockInfo.IN.ExceptWith(blockInfo.HelpFirst);
                        blockInfo.IN.UnionWith(blockInfo.HelpSecond);
                        return blockInfo;
                    };

                    var transferFunction = new TransferFunction<BlockInfo<string>>(tFunc);

                    // создание объекта итерационного алгоритма
                    var iterativeAlgorithm = new IterativeAlgorithm<string>(blocksInfo, controlFlowGraph, meetOperator,
                    false, new HashSet<string>(), new HashSet<string>(), transferFunction);

                    // выполнение алгоритма - вычисление IN и OUT
                    iterativeAlgorithm.Perform();

                    controlFlowGraph = ControlFlowOptimisations.DeadOrAliveOnGraph(iterativeAlgorithm.GetOUTs(), controlFlowGraph); // выполнение оптимизации
