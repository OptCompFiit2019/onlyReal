### Название задачи

Итерационный алгоритм для активных переменных.


### Название команды

BOOM


### Постановка задачи

Задача состояла в вычислении множеств IN и OUT для каждого из блоков графа потоков управления.


### Зависимости задач в графе задач

Данная задача зависит от задачи получения множеств DefB и UseB. Также от задачи Хранение IN[B] и OUT[B] для ряда задач.


### Теория

Данная задача состоит в том, чтобы получить множества In[B] и OUT[B] для каждого из блоков графа потоков управления. Задачи относится к категории анализа потока данных и не является оптимизацией.


### Особенности реализации

```
public class InOutActiveVariables
{
    public List<HashSet<string>> InBlocks { get; } // множества IN для блоков
    public List<HashSet<string>> OutBlocks { get; } // множества OUT для блоков

    /// <summary>
    /// Итерационный алгоритм для активных переменных. Находит для графа <paramref name="graph"/> множества
    /// IN (<paramref name="InBlocks"/>) и OUT (<paramref name="OutBlocks"/>) на основе ранее вычисленных
    /// Def и Use (<paramref name="defUseBlocks"/>). IN и OUT сохраняются в объекте IterAlgoActiveVariables
    /// Граф должен содержать фиктивный узел ВЫХОД.
    /// </summary>
    /// <param name="defUseBlocks"></param>
    /// <param name="graph"></param>
    public InOutActiveVariables(DefUseBlocks defUseBlocks, ControlFlowGraph.ControlFlowGraph graph)
    {
        InBlocks = new List<HashSet<string>>();
        OutBlocks = new List<HashSet<string>>();
        for(int i = 0; i < defUseBlocks.DefBs.Count; i++)
        {
            InBlocks.Add(new HashSet<string>());
            OutBlocks.Add(new HashSet<string>());
        }
        bool isInChanged = true;

        while(isInChanged)
        {
            isInChanged = false;
            for(int i = 0; i < defUseBlocks.DefBs.Count - 1; i++)
            {
                var previousIn = new string[InBlocks[i].Count];
                InBlocks[i].CopyTo(previousIn);
                OutBlocks[i] = MeetOperator(graph, i);
                var Except = new HashSet<string>(OutBlocks[i]);
                Except.ExceptWith(defUseBlocks.DefBs[i]);
                InBlocks[i].UnionWith(Except);
                InBlocks[i].UnionWith(defUseBlocks.UseBs[i]);
                isInChanged = isInChanged || !InBlocks[i].SetEquals(previousIn);
            }
        }
    }

    /// <summary>
    /// Оператор сбора для задачи анализа активных переменных
    /// </summary>
    /// <param name="graph">Граф потоков управления</param>
    /// <param name="index">Индекс анализируемого блока B</param>
    /// <returns></returns>
    private HashSet<string> MeetOperator(ControlFlowGraph.ControlFlowGraph graph, int index)
    {
        var successors = graph.GetAsGraph().GetOutputNodes(index);
        var OutBlock = new HashSet<string>();
        foreach (var i in successors)
            OutBlock.UnionWith(InBlocks[i]);
        return OutBlock;
    }
}
```


### Тесты

Программа до применения алгоритма:
```
{
    int a,b,c,d;
    a = 5;
    b = 3;
    if (true){
	       c = 2;
    }
    int i;
    for (i=1 to 10)
    {
        c = c + 1;
    }
    d = 7;
}
```

После применения алгоритма будут получены множества IN[B] и OUT[B]:
```
Block1
InB: c
OutB: c
Block2
InB: c
OutB: c
Block3
InB:
OutB: c
Block4
InB: c
OutB: i c
Block5
InB: i c
OutB: c i
Block6
InB:
OutB:
Block7
InB: c i
OutB: i c
Block8
InB:
OutB:
```
