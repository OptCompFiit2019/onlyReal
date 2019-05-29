# Вычисление множеств DEFb и USEb для активных переменных

(Граф потока управления (CFG): Вычисление множеств DEFb и USEb для активных переменных)

 ## qwerty

 ### Постановка задачи

Задача состояла в вычислении множеств DEFb и USEb для активных переменных. 

 ### Зависимости задач в графе задач

Данная задача зависит от CFG. 
От нее зависит задача оптимизация на основе ИтА для активных переменных - удаление мертвого кода.

### Теория

Множества вычисляются локально в каждом базовом блоке ```B``` в графе потока управления.

Переменная x активна в точке ```p``` если значение ```x``` из точки ```p``` может использоваться вдоль некоторого пути, начинающегося в ```p```.

```DEFb``` - множество переменных, определенных в ```B```.
```USEb``` - множество переменных, значения которых могут использоваться в ```B``` до любого их определения. 

### Особенности реализации

```csharp
using CFG = SimpleLang.ControlFlowGraph.ControlFlowGraph;
...
class DefUseBlocks
{
	public List<HashSet<string>> DefBs { get; }
	public List<HashSet<string>> UseBs { get; }
	readonly CFG Graph;

	public DefUseBlocks(CFG graph)
	{
		DefBs = new List<HashSet<string>>();
		UseBs = new List<HashSet<string>>();
		Graph = graph;
		MakeSets();
	}

	private void MakeSets()
	{
		foreach (var block in Graph.blocks)
		{
			var DefB = new HashSet<string>();
			var UseB = new HashSet<string>();
			foreach (var cmd in block)
			{
				if (cmd.arg1 != null && cmd.arg1.ToString() != "" && cmd.arg1 is ThreeAddressStringValue 
					&& !cmd.arg1.ToString().StartsWith("temp_") && !cmd.arg1.ToString().StartsWith("label") 
					&& !DefB.Contains(cmd.arg1.ToString()))
					UseB.Add(cmd.arg1.ToString());
				if (cmd.arg2 != null && cmd.arg2.ToString() != "" &&  cmd.arg2 is ThreeAddressStringValue 
					&& !cmd.arg2.ToString().StartsWith("temp_") && !cmd.arg2.ToString().StartsWith("label") 
					&& !DefB.Contains(cmd.arg2.ToString()))
					UseB.Add(cmd.arg2.ToString());
				if (cmd.result != null && cmd.result.ToString() != "" && !cmd.result.ToString().StartsWith("temp_") )
					DefB.Add(cmd.result);
			}
			DefBs.Add(DefB);
			UseBs.Add(UseB);
		}
	}
}
```
Для каждого базового блока в CFG осуществляется проход по каждой команде и проверяется сначала возможность добавить переменную в множество ```USEb```, затем во множество ```DEFb```.


### Тесты

Исходный код:
```
i = k + 1
j = l + 1
k = i
l = j
```

Получившиеся множества:
```
DEFb: i j k l
USEb: k l
```