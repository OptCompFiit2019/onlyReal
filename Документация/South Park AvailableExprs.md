# Доступные выражения-множества e_genB, e_killB. Передаточная функция базового блока В.

(Анализ потоков данных: Доступные выражения-множества e_genB, e_killB. Передаточная функция базового блока В)

### South Park

#### Постановка задачи

Задача состояла в реализации вычисления доступных выражений-множеств e_genB и e_killB, а также необходимо было реализовать вычисление передаточной функции базового блока.

#### Зависимости задач в графе задач
Данная задача зависит от задачи генерации базовых блоков.

От задачи зависит:
* Реализация итерационного алгоритма для доступных выражений 

#### Теория

Выражение ```x+y``` доступно в точке p если любой путь от входа к p вычисляет ```x+y``` и после последнего вычисления до достижения p нет присваиваний ```x``` и ```y```. 
Блок уничтожает выражение ```x+y``` если он присваивает ```x``` или ```y``` и потом не перевычисляет ```x+y```. E_killB − множество всех выражений, уничтожаемых блоком B.
Блок генерирует выражение ```x+y``` если он вычисляет ```x+y``` и потом не переопределяет ```x``` и ```y```. E_genB − множество всех выражений, генерируемых блоком B. 
Передаточная функция вычислятся по формуле ```fB(X) = e_genB ∪ (X − e_killB)```. 

#### Особенности реализации

```csharp
using ExprSet = HashSet<(String, String, String)>;
using KillerSet = HashSet<String>;
    
    public class AvaliableExprs
    {
        public static bool IsDefinition(ThreeOperator opType)
        {
            return opType != ThreeOperator.Logic_not && opType != ThreeOperator.Goto && opType != ThreeOperator.IfGoto;
        }
        public ExprSet GetGenExprSet(LinkedList<ThreeCode> bblock)
        {
            
            var ret = new ExprSet();
            foreach (var line in bblock)
            {
                if (line.operation == ThreeOperator.Goto || line.operation == ThreeOperator.Println) continue;

                ret.RemoveWhere(x => x.Item1 == line.result || x.Item3 == line.result);

                if (line.operation == ThreeOperator.Plus || line.operation == ThreeOperator.Mult || line.operation == ThreeOperator.Minus || line.operation == ThreeOperator.Logic_or || line.operation == ThreeOperator.Logic_not || line.operation == ThreeOperator.Logic_neq || line.operation == ThreeOperator.Logic_less || line.operation == ThreeOperator.Logic_leq || line.operation == ThreeOperator.Logic_greater || line.operation == ThreeOperator.Logic_geq || line.operation == ThreeOperator.Logic_equal || line.operation == ThreeOperator.Logic_and || line.operation == ThreeOperator.Div)
                {
                    ret.Add((line.arg1.ToString(), line.operation.ToString(), line.arg2.ToString()));
                }
            }
            return ret;
        }

        public KillerSet GetKillerSet(LinkedList<ThreeCode> bblock)
        {
            return new KillerSet(bblock
                                 .Where(l => IsDefinition(l.operation))
                                 .Select(l => l.result));
        }

        public (List<ExprSet>, List<KillerSet>) GetGenAndKillerSets(List<LinkedList<ThreeCode>> bblocks)
        {
            return (bblocks.Select(b => GetGenExprSet(b)).ToList(),
                    bblocks.Select(b => GetKillerSet(b)).ToList());

        }

        public ExprSet TransferByGenAndKiller(ExprSet X, ExprSet gen, KillerSet kill)
        {
            if (X == null) return gen;
            return new ExprSet(X.Where(e => !kill.Contains(e.Item1) && !kill.Contains(e.Item3))
                               .Union(gen));
        }
    }
```
Функция _GetGenExprSet_ вычисляет ExprSet, состоящий из множества всех выражений, генерируемых блоком. Если выражение, поступившее на вход является арифметическим или сравнительным, то оно добавляется в ExprSet. Функция _GetKillerSet_ вычисляет _KillerSet_, состоящий из множества всех выражений, уничтожаемых блоком. Если выражение, поступившее на вход является определением, то оно добавляется в _KillerSet_. Функция _GetGenAndKillerSets_ создана для удобства, она позволяет сразу сгенерировать оба множества. Функция _TransferByGenAndKiller_ вычисляется по формуле, где X-все выражения блока, gen-e_gen, kill-e_kill.

#### Тесты
Пример исходной программы:
```
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
```
Пример формирование списке e_genB, e_killB и передаточной функции:
```
E_genB
Block 1:
Block 2:
Block 3:
Block 4:
Block 5: (i, Logic_less, 3)
Block 6:
Block 7: (c, Plus, 1)(i, Plus, 1)
Block 8:

E_killB
Block 1: c
Block 2:
Block 3: b
Block 4: i
Block 5: temp_0
Block 6:
Block 7: c i
Block 8: c

Transfer function
Block 1:
Block 2:
Block 3:
Block 4:
Block 5: (i, Logic_less, 3)
Block 6:
Block 7: (c, Plus, 1)(i, Plus, 1)
Block 8:
```



