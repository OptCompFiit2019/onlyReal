# Передаточная функция в задаче о распространении констант !heading

### Команда Roslyn

#### Постановка задачи

Задача состояла в разработке структуры для хранения передаточной функции для задачи о распространении констант.

#### Зависимости задач в графе задач
Данная задача зависит от задачи генерации базовых блоков.

От задачи зависит:
* Реализация итерационного алгоритма для распространения констант

#### Теория

fS — передаточная функция одной команды S.
1. Если S — не присваивание, то fS — тождественная: `fS(m) = m`
2. Если S: x = ..., то ∀v ≠ x m′(v) = m(v), а m′(x) определяется так:
a) если x := c, то m′ x = c
b) если x = y + z, то
m′(x) = m(y) + m(z), если m(y) − const и m(z) − const, иначе
m′(x) = NAC, если m(y) = NAC или m(z) = NAC, иначе
m′(x) = UNDEF
c) если x = g(...), то m′(x) = NAC (консервативно)

#### Особенности реализации

```csharp
using ConstPropBlockInfo = BlockInfo<KeyValuePair<string, ConstPropSemilatticeEl>>;
using ConstPropKeyValue = KeyValuePair<string, ConstPropSemilatticeEl>;
public partial class ConstantPropagationOptimizer
{
	public static TransferFunction<ConstPropBlockInfo> TransferFunction()
		=> new TransferFunction<ConstPropBlockInfo>(bi =>
		{
			var m = bi.IN.ToDictionary(e => e.Key);
			foreach (var command in bi.Commands)
			{
				if (command.arg1 is ThreeAddressLogicValue
						|| command.arg1 is ThreeAddressDoubleValue
						|| command.arg2 is ThreeAddressLogicValue
						|| command.arg2 is ThreeAddressDoubleValue)
					continue;
				if (command.operation == ThreeOperator.Assign)
					m[command.result] = new ConstPropKeyValue(command.result,
						GetSemilatticeEl(command.arg1, m));
				else if (command.operation == ThreeOperator.Plus
						|| command.operation == ThreeOperator.Minus
						|| command.operation == ThreeOperator.Mult
						|| command.operation == ThreeOperator.Div)
				{
					var el1 = GetSemilatticeEl(command.arg1, m);
					var el2 = GetSemilatticeEl(command.arg2, m);
					if (el1.Constantness == ValConstType.Const
							&& el2.Constantness == ValConstType.Const)
						m[command.result] = new ConstPropKeyValue(command.result,
							new ConstPropSemilatticeEl(ValConstType.Const,
								EvalConst(el1.Value, el2.Value, command.operation)));
					else if (el1.Constantness == ValConstType.NAC
							|| el2.Constantness == ValConstType.NAC)
						m[command.result] = new ConstPropKeyValue(command.result,
							new ConstPropSemilatticeEl(ValConstType.NAC));
					else
						m[command.result] = new ConstPropKeyValue(command.result,
							new ConstPropSemilatticeEl(ValConstType.Undef));
				}
			}
			var Out = new ConstPropBlockInfo(bi);
			Out.OUT = new HashSet<ConstPropKeyValue>(m.Values);
			return Out;
		});
	private static int EvalConst(int c1, int c2, ThreeOperator op)
	{
		switch (op)
		{
			case ThreeOperator.Plus:  return c1 + c2;
			case ThreeOperator.Minus: return c1 - c2;
			case ThreeOperator.Mult:  return c1 * c2;
			case ThreeOperator.Div:   return c1 / c2;
			default: throw new Exception("Logic error");
		}
	}
	private static ConstPropSemilatticeEl GetSemilatticeEl
		(ThreeAddressValueType val,
		 Dictionary<string, ConstPropKeyValue> m)
	{
		ConstPropSemilatticeEl semilatticeEl = null;
		if (val is ThreeAddressStringValue v)
			semilatticeEl = m[v.Value].Value;
		else if (val is ThreeAddressIntValue c)
			semilatticeEl = new ConstPropSemilatticeEl(ValConstType.Const, c.Value);
		return semilatticeEl;
	}
}
```
Класс _ConstantPropagationOptimizer_ предоставляет функцию _TransferFunction_ в формате, совместимом с обобщённым итерационным алгоритмом. _ConstPropSemilatticeEl_ — это тип элементов полурешётки. Значения этого типа имеют два поля: _Constantness_ и _Value_. Поле _Constantness_ допускает значения _Const_, _NAC_ и _Undef_. Поле _Value_ используется, если _Constantness_ имеет значение _Const_, и хранит значение константы типа _int_.

#### Тесты
Пример исходной программы:
```
{
	int t1, t2, i, x, u1;
	i = 2;
	t1 = 4 * i;
	x = 3 + 17;
	u1 = 5 * t1;
	t2 = i;
}
```
Пример работы передаточной функции:
```
Before
i: (Undef, 0)
t1: (Undef, 0)
x: (Undef, 0)
u1: (Undef, 0)
t2: (Undef, 0)
After
i: (Const, 2)
t1: (Const, 8)
x: (Const, 20)
u1: (Const, 40)
t2: (Const, 2)
```

[Вверх](#содержание)