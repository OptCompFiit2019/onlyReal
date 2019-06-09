# Поиск доминаторов для каждой вершины графа потока управления

Поиск доминаторов для каждой вершины графа потока управления

## Komanda

### Постановка задачи
Задача состояла в поиске доминаторов для каждой вершины графа потока управления

## Зависимости в графе задач

- зависит от зачади построения графа потока управления

## Теория

Пусть d, n − вершины CFG. Будем говорить, что d dom n
(d доминирует над n) если любой путь от входного узла к n проходит
через d.
Среди всех доминаторов узла будем выделять непосредственный
доминатор:

### Особенности реализации

```csharp
using System.Collections.Generic;
using System.Linq;

namespace SimpleLang.Dominators
{
    class DominatorsFinder
    {
        private readonly ControlFlowGraph.ControlFlowGraph cfg;

        private Dictionary<int, List<int>> dominators;
        public Dictionary<int, List<int>> Dominators
        {
            get
            {
                if (dominators == null)
                    dominators = Find();
                return dominators;
            }
        }

        public DominatorsFinder(ControlFlowGraph.ControlFlowGraph cfg)
        {
            this.cfg = cfg;
        }

        public Dictionary<int, List<int>> Find()
        {
            if (dominators != null)
                return dominators;

            dominators = new Dictionary<int, List<int>>
            {
                [0] = new List<int>() { 0 }
            };

            var allBlocks = Enumerable.Range(0, cfg.blocks.Count);
            var initOut = Enumerable.Range(1, cfg.blocks.Count - 1);
            for (int i = 1; i < cfg.blocks.Count; i++)
            {
                dominators[i] = new List<int>(allBlocks);
            }

            var adjMatr = cfg.GetAsAdjacencyMatrix();
            var blocksInputs = new List<List<int>>();
            for (int i = 1; i < cfg.blocks.Count; i++)
            {
                blocksInputs.Add(new List<int>());
                for (int j = 0; j < cfg.blocks.Count; j++)
                {
                    if (i != j && adjMatr[j, i] == 1)
                        blocksInputs.Last().Add(j);
                }
            }

            var f = true;
            while (f)
            {
                f = false;
                for (int i = 1; i < cfg.blocks.Count; i++)
                {
                    var _in = new List<int>(allBlocks);
                    foreach (var bl in blocksInputs[i - 1])
                    {
                        _in = _in.Intersect(dominators[bl]).ToList();
                    }

                    if (!_in.Contains(i))
                        _in.Add(i);
                    if (!f)
                    {
                        f = dominators[i].Count != _in.Count;
                        if (!f)
                            foreach (var item in _in)
                            {
                                f = !dominators[i].Contains(item);
                                if (f)
                                    break;
                            }
                    }
                    dominators[i] = _in;
                }
            }

            return dominators;
        }
    }
}
```
Итерационный алгоритм вычисления доминаторов:
1. Устанавливаем множество доминаторов для входа - множество, состоящее из входа;
2. Для остальных блоков - множество из всех блоков
3. Пока вносятся изменения в множество доминаторов:
4. Для каждого блока(B) != входу
	1. Найти пересечение (⋂) множеств доминаторовов блоков входящих в этот 
	2. Установить множество доминаторов этого блока = {⋂} U {B}


### Тесты

исходный код:

    {
    	real b;
    	int c;
    	c=1;
    	if (true)
    	{
    		b=3.14;
    	} 
    	int i;
    	for (i=0 to 3)
    	{
    		c=c+1;
    	}
            c = 0;
    }

результат:

0: {0}
1: {0 1}
2: {0 2}
3: {0 3}
4: {0 3 4}
5: {0 3 4 5}
6: {0 3 4 6}
7: {0 3 4 5 7}