# Оптимизация Распространение констант !heading

### Команда Roslyn

#### Постановка задачи

Задача состояла в реализации оптимизации по распространению констант на основе информации, полученной в результате применения итерационного алгоритма.

#### Зависимости задач в графе задач
Данная задача зависит от итерационного алгоритма в задаче о распространении констант / обобщённого итерационного алгоритма.

От задачи зависит:
* Задача не порождает новых зависимостей

#### Теория

*Постановка задачи.* Заменить в программе переменные, имеющие константное значение, на константу.
Пример, когда можно провести данную оптимизацию.
```
{
	if (cond)
	{
		x = 1;
	}
	else
	{
		x = 1;
	}
	a = x;
}
```
Пример, когда провести данную оптимизацию нельзя.
```
{
	if (cond)
	{
		x = 1;
	}
	else
	{
		x = 2;
	}
	a = x;
}
```

#### Особенности реализации

```
using ConstPropBlockInfo = BlockInfo<KeyValuePair<string, ConstPropSemilatticeEl>>;
using ConstPropKeyValue = KeyValuePair<string, ConstPropSemilatticeEl>;

public partial class ConstantPropagationOptimizer : ThreeCodeOptimiser
{
	private CFG controlFlowGraph;
	private List<HashSet<ConstPropKeyValue>> Ins;

	public void IterativeAlgorithm(List<LinkedList<ThreeCode>> blocks)
	{
		// построение CFG по блокам
		controlFlowGraph = new CFG(blocks.ToList());
		// создание информации о блоках
		var blocksInfo = new List<ConstPropBlockInfo>();
		var m = new Dictionary<string, ConstPropSemilatticeEl>();
		for (int i = 0; i < blocks.Count; i++)
		{
			foreach (var c in blocks[i].Where(com =>
				com.operation != ThreeOperator.Goto && com.operation != ThreeOperator.IfGoto))
			{
				string[] vars = new string[]
					{ c.result
					, (c.arg1 as ThreeAddressStringValue)?.Value
					, (c.arg2 as ThreeAddressStringValue)?.Value };

				foreach (var v in vars)
					if (v != null && v != "" && !m.ContainsKey(v))
						m[v] = new ConstPropSemilatticeEl(ValConstType.Undef);
			}
		}
		for (int i = 0; i < blocks.Count; i++)
			blocksInfo.Add(new ConstPropBlockInfo(blocks[i]));

		// оператор сбора в задаче о распространении констант
		Func<List<ConstPropBlockInfo>, CFG, int, ConstPropBlockInfo> meetOperator =
			(blocksInfos, graph, index) =>
			{
				var inputIndexes = graph.cfg.GetInputNodes(index);
				var resInfo = new ConstPropBlockInfo(blocksInfos[index]);
				foreach (var i in inputIndexes)
				{
					var resIn = resInfo.IN.ToDictionary(e => e.Key);
					foreach (var Out in blocksInfos[i].OUT)
						if (resIn[Out.Key].Value.Constantness == ValConstType.Undef)
							resIn[Out.Key] = new ConstPropKeyValue(Out.Key, Out.Value);
						else if (resIn[Out.Key].Value.Constantness == ValConstType.NAC
									|| Out.Value.Constantness == ValConstType.NAC
									|| (resIn[Out.Key].Value.Constantness == ValConstType.Const
											&& Out.Value.Constantness == ValConstType.Const
											&& resIn[Out.Key].Value.Value != Out.Value.Value))
							resIn[Out.Key] = new ConstPropKeyValue(Out.Key,
								new ConstPropSemilatticeEl(ValConstType.NAC));

					resInfo.IN = new HashSet<ConstPropKeyValue>(resIn.Values);
				}
				return resInfo;
			};

		var transferFunction = TransferFunction();

		// создание объекта итерационного алгоритма
		var iterativeAlgorithm = new IterativeAlgorithm<ConstPropKeyValue>(blocksInfo,
			controlFlowGraph, meetOperator, true, new HashSet<ConstPropKeyValue>(m),
			new HashSet<ConstPropKeyValue>(m), transferFunction);

		// выполнение алгоритма
		iterativeAlgorithm.Perform();
		Ins = iterativeAlgorithm.GetINs();
	}

	...
	
	public CFG ApplyOptimization(List<LinkedList<ThreeCode>> blocks)
	{
		IterativeAlgorithm(blocks);
		var bs = controlFlowGraph.blocks;

		for (int i = 0; i < bs.Count; ++i)
		{
			var m = Ins[i].ToDictionary(e => e.Key);
			for (var it = bs[i].First; true; it = it.Next)
			{
				var command = it.Value;
				if (command.arg1 is ThreeAddressStringValue v1 && m.ContainsKey(v1.Value)
						&& m[v1.Value].Value.Constantness == ValConstType.Const)
					command.arg1 = new ThreeAddressIntValue(m[v1.Value].Value.Value);

				if (command.arg2 is ThreeAddressStringValue v2 && m.ContainsKey(v2.Value)
						&& m[v2.Value].Value.Constantness == ValConstType.Const)
					command.arg2 = new ThreeAddressIntValue(m[v2.Value].Value.Value);
				m.Remove(command.result);

				if (it == bs[i].Last)
					break;
			}
		}
		return controlFlowGraph;
	}
```

Класс _ConstantPropagationOptimizer_ предоставляет функции _IterativeAlgorithm_ и _ApplyOptimization_. Функция _IterativeAlgorithm_ принимает на вход базовые блоки исходной программы и запускает для них (обобщённый) итерационный алгоритм. Данная функция используется в функции _ApplyOptimization_, которая запускает оптимизацию на основе информации, полученной в результате работы алгоритма. В функции _IterativeAlgorithm_ определяется оператор сбора для задачи распространения констант и используется функция _TransferFunction_, определённая в классе _ConstantPropagationOptimizer_, предоставляющая передаточную функцию для данной задачи.

Для использования данной оптимизации необходимо выполнить следующий код:
```csharp
// Построение базовых блоков по трёхадресному коду исходной программы
var blocks = new Block(treeCode).GenerateBlocks();
// Применение оптимизации (итеративный алгоритм запускается внутри метода ApplyOptimization)
var constPropOptimizer = new ConstantPropagationOptimizer();
CFG cfg = constPropOptimizer.ApplyOptimization(blocks);
```
Функция _ApplyOptimization_ строит по базовым блокам исходной программы граф потока управления и запускает для него обобщённый итерационный алгоритм, используя передаточную функцию для задачи распространения констант. На выходе этой функции — новый граф потока управления.

#### Тесты

Пример исходной программы:
```
{
    int a,b,c;
    b = 5;
    a = 1;
    c = 2 * b;
	if (true)
	{
		a = c * b - a + 3;
		println(a);
	}
}
```
После применения оптимизации программа становится эквивалентной следующей:
```
{
    int a,b,c;
    b = 5;
    a = 1;
    c = 2 * b;
	if (true)
	{
		a = 10 * 5 - 1 + 3;
		println(a);
	}
}
```
Результат в трёхадресном коде:
```
           b = 5
           a = 1
           c = 2 * b
           if True goto label_0

           goto label_1

label_0:
           temp_2 = 10 * 5
           temp_1 = temp_2 - 1
           temp_0 = temp_1 + 3
           a = temp_0
           println a

label_1:
```

[Вверх](#содержание)