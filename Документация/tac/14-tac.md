# Достигающие определения множества genB killB Передаточная функция базового блока В !heading

### Команда Roslyn

#### Постановка задачи

Задача состояла в реализации вычисления множеств genB и killB для анализа достигающих определений, а также в разработке структуры для хранения передаточной функции.

#### Зависимости задач в графе задач
Данная задача зависит от задачи генерации базовых блоков.

От задачи зависит:
* Реализация итерационного алгоритма для достигающих определений

#### Теория

genB — множество определений, генерируемых и не переопределённых базовым блоком B.
killB — множество остальных определений переменных, определяемых в определениях genB, в других базовых блоках.
Передаточная функция вычислятся по формуле `fB(X) = genB ∪ (X − killB)`. 

#### Особенности реализации

```csharp
using BasicBlock = LinkedList<ThreeCode>;
// Адаптор для совместимости с обобщённым итерационным алгоритмом
public class ReachingDefsAdaptor
{
	private ReachingDefsTransferFunction tf;
	public ReachingDefsAdaptor(CFG cfg)
		=> tf = new ReachingDefsTransferFunction(cfg);
	public TransferFunction<BlockInfo<ThreeCode>> TransferFunction()
		=> new TransferFunction<BlockInfo<ThreeCode>>(bi =>
		{
			var Out = new BlockInfo<ThreeCode>(bi);
			Out.OUT = tf.BlockTransferFunction(bi.Commands)
				(new HashSet<ThreeCode>(bi.IN));
			return Out;
		});
}
public class ReachingDefsTransferFunction
{
	private CFG cfg;
	public ReachingDefsTransferFunction(CFG cfg) => this.cfg = cfg;
	private IEnumerable<ThreeCode> Definitions(BasicBlock bb)
		=> bb.Where(tc => tc.operation != ThreeOperator.Goto
			&& tc.operation != ThreeOperator.IfGoto
			&& tc.operation != ThreeOperator.None);
	private List<HashSet<ThreeCode>> InstructionGens(BasicBlock bb)
		=> Definitions(bb).Select(tc =>
			new HashSet<ThreeCode>(new ThreeCode[] { tc })
		).ToList();
	private List<HashSet<ThreeCode>> InstructionKills(BasicBlock bb)
		=> Definitions(bb).Select(tc =>
			new HashSet<ThreeCode>(cfg.blocks.Where(b => b != bb)
				.Select(b => Definitions(b))
				.SelectMany(e => e).Where(tc_cur => tc_cur.result == tc.result))
		).ToList();
	public HashSet<ThreeCode> Kill(BasicBlock bb)
	{
		var kills = InstructionKills(bb);
		if (kills.Count == 0)
			return new HashSet<ThreeCode>();
		else
			return kills.Aggregate((s1, s2) =>
				new HashSet<ThreeCode>(s1.Union(s2))
		);
	}
	public HashSet<ThreeCode> Gen(BasicBlock bb)
	{
		var gens = InstructionGens(new LinkedList<ThreeCode>
			(bb.GroupBy(tc => tc.result, (tc, e) => e.Last())));
		var kills = InstructionKills(bb);
		int n = gens.Count;
		if (n <= 0)
			return new HashSet<ThreeCode>();
		HashSet<ThreeCode> gen = gens[n - 1];
		for (int i = n - 2; i >= 0; --i)
		{
			var gen_cur = gens[i];
			for (int j = i + 1; j < n; ++j)
				gen_cur.ExceptWith(kills[j]);
			gen.UnionWith(gen_cur);
		}
		return gen;
	}
	private IEnumerable<InstructionTransferFunction> InstructionTransferFunctions(BasicBlock bb)
	{
		var tf = InstructionGens(bb).Zip(InstructionKills(bb), (g, k) =>
			new InstructionTransferFunction(g, k));
		if (tf.Count() == 0)
			tf = new InstructionTransferFunction[] { new InstructionTransferFunction() };
		return tf;
	}
	public Func<HashSet<ThreeCode>, HashSet<ThreeCode>> BlockTransferFunction(BasicBlock bb)
		=> InstructionTransferFunctions(bb).Aggregate((f, g) => f * g).Func;
}
public class InstructionTransferFunction
{
	public Func<HashSet<ThreeCode>, HashSet<ThreeCode>> Func;
	public InstructionTransferFunction()
		=> Func = defs => defs;
	public InstructionTransferFunction(HashSet<ThreeCode> gen, HashSet<ThreeCode> kill)
		=> Func = defs => new HashSet<ThreeCode>(gen.Union(defs.Except(kill)));
	public InstructionTransferFunction(Func<HashSet<ThreeCode>, HashSet<ThreeCode>> Func)
		=> this.Func = Func;
	public static InstructionTransferFunction operator *
			(InstructionTransferFunction f, InstructionTransferFunction g)
		=> new InstructionTransferFunction(defs => f.Func(g.Func(defs)));
}
```
Функция _Gen_ вычисляет множество определений (команд трёхадресного кода), генерируемых блоком. Функция _Kill_ вычисляет множество всех определений, уничтожаемых блоком. Функция _BlockTransferFunction_ возвращает делегат передаточной функции, вычисленной по формуле. Класс _ReachingDefsAdaptor_ предоставляет функцию _TransferFunction_ в формате, совместимом с обобщённым итерационным алгоритмом.

#### Тесты
Пример исходной программы:
```
{
		int i, j, m, n, a, u1, u2, u3, t, x;
/*d1*/	i = m - 1;
/*d3*/	j = n;
/*d4*/	a = u1;
		while (true)
		{
/*d5*/		i = i + 1;
/*d6*/		j = j - 1;
			if (false)
			{
/*d7*/			a = u2;
			}
/*d8*/		i = u3;
		}
}
```
Пример формирования множеств genB и killB:
```
GenB
Block 1: { d1, d2, d3 }
Block 2: { d4, d5 }
Block 3: { d6 }
Block 4: { d7 }
KillB
Block 1: { d4, d5, d6, d7 }
Block 2: { d1, d2, d7 }
Block 3: { d3 }
Block 4: { d1, d4 }
```

[Вверх](#содержание)