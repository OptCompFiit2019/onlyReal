# Определение того, является ли ребро обратным и являеется ли CFG приводимым !heading

### Команда GreatBean

## Является ли ребро обратным !heading

#### Постановка задачи
Требуется найти в CFG графе ребра, которые являются обратными.

#### Зависимости задач в графе задач
Задача зависит от:
* Вычисление доминаторов

#### Теория
Задача является сугубо теоритической.

###### Обратные ребра (back edjes)
Определение. Обратным в графе CFG называется ребро 𝑎 → 𝑏, у которого 𝑏 доминирует над 𝑎.


#### Особенности реализации
Для определения того, что ребро Сontrol Flow Graph является обратным сперва необходимо найти список доминаторов CFG. Также необходимо построить структуру CFG для ее дальнейшего обхода. После этого необходимо для каждого ребра из CFG проверить, что вершина, в которую приходит ребро доминирует над вершиной, из которой ребро выходит. Эту проверку мы выполняем поиском соответвующих доминаторов в списке доминаторов.

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
```
Edge 1 -> 2 is not reverse
Edge 2 -> 3 is not reverse
Edge 2 -> 4 is not reverse
Edge 3 -> 5 is not reverse
Edge 4 -> 2 is reverse
Edge 5 -> 6 is not reverse
Edge 5 -> 7 is not reverse
Edge 6 -> 8 is not reverse
Edge 7 -> 5 is reverse
```
## Является ли CFG приводимым !heading

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

[Вверх](#содержание)

