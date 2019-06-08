### Определение того, является ли CFG приводимым
Определение приводимости Control Flow Graph

### Команда GreatBean

#### Постановка задачи
Требуется определить, является в CFG приводимым.

#### Зависимости задач в графе задач
Задача зависит от:
* Вычисление доминаторов
* Нахождение обратных ребер CFG

#### Теория
Задача является сугубо теоритической.

###### Приводимость графа потока управления
Определение. Граф потока управления называется приводимым если все его отступающие рёбра являются обратными.


#### Особенности реализации
Для определения того, что Сontrol Flow Graph является приводимым сперва необходимо найти список обратных ребер. После нахождения необходимо сравнить количество обратных и отступающих ребер.

Для решения этой задачи был реализован класс DetectReversibleEdges.
```csharp
class DetectReversibleEdges
{
    Dictionary<int, List<int>> dominators;
    List<Edge> edges;
    // словарь, где каждому ребру соответствует значение bool, если ребро обратимо-true, иначе false
    Dictionary<Edge, bool> isReversible = new Dictionary<Edge, bool>();

    //Конструктор
    public DetectReversibleEdges(ControlFlowGraph.ControlFlowGraph cfg)
    {
        var dom = new DominatorsFinder(cfg);
        dominators = dom.Find();

        var spt = new SpanTree(cfg);
        edges = spt.buildSpanTree();

        foreach (var e in edges)
            isReversible[e] = dominators.Keys.Contains(e.v1.num)
                && dominators[e.v1.num].Contains(e.v2.num);
    }

    // возвращает заполненный словарь isReversible
    public Dictionary<Edge, bool> isRevers()
    {
        return isReversible;
    }

    // Выводит словарь isReversible в консоль
    public void PrintIsReverseDic()
    {
        var dic = isRevers();
        foreach (var x in dic)
            Console.WriteLine("Edge {0} -> {1} is {2}", x.Key.v1.num.ToString(),  
                x.Key.v2.num.ToString(), x.Value? "reverse" : "not reverse");
    }

    //Функция определяет, является ли граф приводимым.
    //Если количество отступающих рёбер равно количеству обратимых, то граф приводим
    public bool isReducible()  
    {
        int countBack = 0;
        int countReversible = 0;
        foreach(var e in edges)
            if (e.type == EdgeType.Back)
                countBack++;

        foreach (var i in isReversible)
            if (i.Value)
                countReversible++;

        return countBack == countReversible;
    }

    // Выводит в консоль факт приводимости CFG
    public void PrintisReducible()
    {
        Console.WriteLine("CFG is {0}", isReducible() ? "reducibile" : "not reducibile");
    }
}
```

#### Тесты
Из исходной программы вида
```csharp
{
  int a,b;
  a = 3;
  b = a;
  while (a > 0)
  {
    a = a - 1;
  }
  while (a > 0)
  {
    a = a - 1;
  }
}
```

Мы получаем отчет в виде

CFG is reducibile
