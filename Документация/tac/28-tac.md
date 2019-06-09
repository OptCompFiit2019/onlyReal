# Класс обобщенного итерационного  алгоритма !heading

### Команда Roll

#### Постановка задачи
Написать класс обобщенного итерационного  алгоритма. Обеспечить прямой и обратный ход анализа, задание передаточной функции, оператора сбора. Полученный класс должен позволить выполнять итерационный алгоритм для доступных выражений, активных переменных, достигающих определений и распространения констант.

#### Зависимости задач в графе задач

Задача зависит от:
* Класс передаточной функции

#### Теория
Обобщенный итерационный алгоритм является основой для реализации конкретных итерационных алгоритмов. Для заданного графа потоков управления он производит анализ потоков данных. Основными этапами итерационного алгоритма являются:
1. Инициализация множеств, как анализируемых, так и вспомогательных (IN, OUT, Def, Use, Gen, Kill и т.п.)
2. Основной цикл алгоритма, на каждой итерации которого производится обновление множеств IN и OUT для всего графа. Для информации о каждом из блоков применяется оператор сбора и передаточная функция.
3. Проверка условия остановки. Обычно анализ заканчивается когда множества IN, OUT более не претерпевают изменений.

#### Особенности реализации
Для использования данного класса необходимо:
1. Подключить пространство имен using SimpleLang.GenericTransferFunction;
2. Написать делегат или множество делегатов, реализующих конкретную передаточную функцию.
3. Создать объект передаточной функции, передав в конструктор делегат или список делегатов.
4. Применить передаточную функцию к объекту путем вызова у передаточной функции метода Apply.

Ниже представлен код использования данного класса. Пример показывает анализ активных переменных и удаления мертвых переменных на его основе:
```csharp
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

```

#### Тесты
``` csharp
Исходный код
{
    int a,b,c;
    b = a;
    a = 1;
    a = 2;
    a = 3;
    while ((c > (a - b)))
    {
        c = (c + 1);
        a = b;
        b = 100;
        b = (c + 4);
        a = 30;
    }
    println(a);
}
Блоки трехадресного кода до каскадного удаления мертвых переменных
           b = a
           a = 1
           a = 2
           a = 3
label_0:   temp_2 = a - b
           temp_1 = c > temp_2
           temp_0 = temp_1
           if temp_0 goto label_1
           goto label_2
label_1:   c = c + 1
           a = b
           b = 100
           b = c + 4
           a = 30
           goto label_0
label_2:   println a


После каскадного удаления мертвых переменных для графа
           b = a
           a = 3
label_0:   temp_2 = a - b
           temp_1 = c > temp_2
           temp_0 = temp_1
           if temp_0 goto label_1
           goto label_2
label_1:   c = c + 1
           b = c + 4
           a = 30
           goto label_0
label_2:   println a
```

[Вверх](#содержание)