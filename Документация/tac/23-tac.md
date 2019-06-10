# Класс передаточной функции !heading

### Команда Roll

#### Постановка задачи
Написать класс передаточной функции. Функция может задаваться формулой, алгоритмом. Реализовать суперпозицию функций.

#### Зависимости задач в графе задач

От задачи зависит:
* Обобщенный итерационный алгоритм

#### Теория
Передаточная функция преобразует множество IN на входе в блок во множество на выходе из блока OUT.

#### Особенности реализации
Для использования данной оптимизации необходимо:
1. Подключить пространство имен using SimpleLang.GenericTransferFunction;
2. Написать делегат или множество делегатов, реализующих конкретную передаточную функцию.
3. Создать объект передаточной функции, передав в конструктор делегат или список делегатов.
4. Применить передаточную функцию к объекту путем вызова у передаточной функции метода Apply.

Ниже представлен код использования данной функции:
```csharp
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

```
В строках 46-62 создаются делегаты и объекты передаточных функций. Для наглядности они представлены в виде двух функций, суперпозиция которых передается в конструктор итерационного алгоритма. Суперпозиция передаточных функций синтаксически оформлена через оператор \*. При этом результатом суперпозиции будет новый объект передаточной функции, в списке делегатов которого содержатся все делегаты исходных функций.
Еще примеры использования данного класса:
```csharp
using GenericTransferFunction;
using SimpleLang.Visitors;

// сначала описать передаточную функцию как делегат
// например эквивалентная передаточная функция для строк
Func<string, string> EquivalentFunction = (In) => In;

//создать объект передаточной функции одним из методов:

var ts = new TransferFunction<string>(EquivalentFunction);

// применение передаточной функции к аргументу выполняется через метод Apply
string s = "d";
string res = ts.Apply(s);

// суперпозиция передаточных функций - новая передаточная функция
var tsSuperpostion = (ts * ts * ts);

// получение результата применения суперпозиции без создания нового объекта функции
var resSuperpostion = (ts * ts * ts).Apply(s);

// чтобы изначально создать объект передаточной функции, которая является суперпозицией, нужно вызвать конструктор, принимающий List<Func<T, T>> algorithms
var listFs = new List<Func<string, string>>() 
{
	EquivalentFunction, EquivalentFunction, EquivalentFunction
};
var tfAlgorithms = new TransferFunction<string>(listFs);
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