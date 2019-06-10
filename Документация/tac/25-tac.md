# Оптимизация Доступные выражения !heading

### Команда Roslyn

#### Постановка задачи

Задача состояла в реализации оптимизации на основе анализа доступных выражений, использующего итерационный алгоритм.

#### Зависимости задач в графе задач
Данная задача зависит от итерационного алгоритма для доступных выражений / обобщённого итерационного алгоритма.

От задачи зависит:
* Задача не порождает новых зависимостей

#### Теория

*Постановка задачи.* Если в программе имеются общие подвыражения, вынести их во временную переменную.
Пример:
```
{
	int t1, t2, t3, i, a;
	t1 = 4 * i;
	if (true)
	{
		i = a;
		t3 = 4 * i;
	}
	t2 = 4 * i;
}
```
После применения оптимизации программа должна стать эквивалентной следующей:
```
{
	int t1, t2, t3, i, a;
	#t0 = 4 * i;
	t1 = #t0;
	if (true)
	{
		i = a;
		#t0 = 4 * i;
		t3 = #t0;
	}
	t2 = #t0;
}
```
*Определение.* Выражение `x + y` доступно в точке p если любой путь от входа к p вычисляет `x+y` и после последнего вычисления до достижения p нет присваиваний `x` и `y`. 

#### Особенности реализации

```
using Expr = Tuple<ThreeAddressValueType, ThreeOperator, ThreeAddressValueType>;

class AvailableExprsOptimizer : ThreeCodeOptimiser
{
	private CFG controlFlowGraph;

	private int currentTempVarIndex = 0;

	public List<HashSet<Expr>> Ins { get; private set; }
	public List<HashSet<Expr>> Outs { get; private set; }

	public void IterativeAlgorithm(List<LinkedList<ThreeCode>> blocks)
	{
		var bb = new LinkedList<ThreeCode>();
		bb.AddLast(new ThreeCode("entry", "", ThreeOperator.None, null, null));
		var bs = blocks.ToList();
		// добавление пустого входного блока - необходимо для корректной работы ит. алгоритма
		bs.Insert(0, bb);
		// построение CFG по блокам
		controlFlowGraph = new CFG(bs);
		// создание информации о блоках
		var blocksInfo = new List<BlockInfo<Expr>>();
		for (int i = 0; i < bs.Count; i++)
			blocksInfo.Add(new BlockInfo<Expr>(bs[i]));

		// оператор сбора для доступных выражений
		Func<List<BlockInfo<Expr>>, CFG, int, BlockInfo<Expr>> meetOperator =
			(blocksInfos, graph, index) =>
			{
				var inputIndexes = graph.cfg.GetInputNodes(index);
				var resInfo = new BlockInfo<Expr>(blocksInfos[index]);
				resInfo.IN = resInfo.OUT; // универсальное множество
				foreach (var i in inputIndexes)
					resInfo.IN.IntersectWith(blocksInfos[i].OUT);
				return resInfo;
			};

		var transferFunction = AvaliableExprsAdaptor.TransferFunction();

		var U = new HashSet<Expr>(blocks.Select(b => b.Where(c =>
				AvaliableExprs.IsDefinition(c.operation))
			.Select(c => new Expr(c.arg1, c.operation, c.arg2)))
			.Aggregate((s1, s2) => s1.Union(s2)));

		// создание объекта итерационного алгоритма
		var iterativeAlgorithm = new IterativeAlgorithm<Expr>(blocksInfo,
			controlFlowGraph, meetOperator, true, new HashSet<Expr>(), U, transferFunction);

		// выполнение алгоритма
		iterativeAlgorithm.Perform();
		Ins = iterativeAlgorithm.GetINs();
		Outs = iterativeAlgorithm.GetOUTs();
	}

	public CFG ApplyOptimization(List<LinkedList<ThreeCode>> blocks)
	{
		IterativeAlgorithm(blocks);
		var bs = controlFlowGraph.blocks;

		for (int i = 1; i < bs.Count; ++i)
		{
			for (var it = bs[i].First; true; it = it.Next)
			{
				var command = it.Value;
				var expr = new Expr(command.arg1, command.operation, command.arg2);
				if (Ins[i].Contains(expr))
				{
					string t = GenTempVariable();
					it.Value = new ThreeCode(command.result, new ThreeAddressStringValue(t));
					ApplyOptToAncestors(i, expr, t);
					if (Outs[i].Contains(expr))
						ApplyOptToDescendents(i, expr, t);
				}

				Ins[i].ExceptWith(Ins[i].Where(e => e.Item1.ToString() == command.result
					|| e.Item3?.ToString() == command.result).ToList());
				if (it == bs[i].Last)
					break;
			}
		}
		return controlFlowGraph;
	}
	
	...
}
```

Класс _AvailableExprsOptimizer_ предоставляет функции _IterativeAlgorithm_ и _ApplyOptimization_. Функция _IterativeAlgorithm_ принимает на вход базовые блоки исходной программы и запускает для них (обобщённый) итерационный алгоритм. Данная функция используется в функции _ApplyOptimization_, которая запускает оптимизацию на основе информации, полученной в результате работы алгоритма. В функции _IterativeAlgorithm_ определяется оператор сбора для доступных выражений и используется функция _TransferFunction_, определённая в классе _AvaliableExprsAdaptor_, предоставляющая передаточную функцию для данной задачи.

Для использования данной оптимизации необходимо выполнить следующий код:
```csharp
// Построение базовых блоков по трёхадресному коду исходной программы
var blocks = new Block(treeCode).GenerateBlocks();
// Применение оптимизации (итеративный алгоритм запускается внутри метода ApplyOptimization)
var availableExprsOptimizer = new AvailableExprsOptimizer();
CFG cfg = availableExprsOptimizer.ApplyOptimization(blocks);
```
Функция _ApplyOptimization_ строит по базовым блокам исходной программы граф потока управления и запускает для него обобщённый итерационный алгоритм, используя передаточную функцию для доступных выражений. На выходе этой функции — новый граф потока управления.

#### Тесты

Пример исходной программы:
```
{
	int t1, t2, t3, i, a, x;
	int u1, u2, u3, u4, u5;
	i = 2;
	t1 = 4 * i;
	x = 4 * i;
	u1 = 5 * i;
	if (true)
	{
		u3 = 5 * i;
	}
	t2 = 4 * i;
	u2 = 5 * i;
	u4 = 6 * i;
	u5 = 5 * i;
}
```
После применения оптимизации программа становится эквивалентной следующей:
```
{
	int t1, t2, t3, i, a, x;
	int u1, u2, u3, u4, u5;
	i = 2;
	#t1 = 4 * i;
	#t0 = 5 * i;
	t1 = #t1;
	x = #t1;
	u1 = #t0;
	if (true)
	{
		u3 = #t0;
	}
	t2 = #t1;
	u2 = #t0;
	u4 = 6 * i;
	u5 = #t0;
}
```
Результат в трёхадресном коде:
```
entry:

           i = 2
           #t1 = 4 * i
           #t0 = 5 * i
           t1 = #t1
           x = #t1
           u1 = #t0
           if True goto label_0

           goto label_1

label_0:
           u3 = #t0

           t2 = #t1
           u2 = #t0
           u4 = 6 * i
           u5 = #t0
```

[Вверх](#содержание)