# Протяжка копий

(Трёхадресный код: протяжка копий)

 ## qwerty

 ### Постановка задачи

Задача состояла в реализации локальной оптимизации «протяжка копий».

 ### Зависимости задач в графе задач

Данная задача не зависит от других задач в графе задач и не порождает новых зависимостей.

### Теория

Данная оптимизация заключается в том, чтобы при присваивании переменной **_X_** значения другой переменной **_Y_**,  которая уже определёна (**Y = t**), заменять переменную **_Y_**  на _**t**._

### Особенности реализации

```csharp
class PullCopies
{
	public List<ThreeCode> program;

	public PullCopies(List<ThreeCode> prog)
	{
		program = prog;
	}

	public LinkedList<ThreeCode> Optimize()
	{
		for (int i = 0; i < program.Count - 1; i++)
		{
			string def;
			ThreeAddressValueType newArg;
			if (program[i].arg2 == null && !program[i].arg1.ToString().Contains("temp_"))
			{
				def = program[i].result;
				newArg = program[i].arg1;
			}
			else
				continue;
			bool same_def = false;
			for (int j = i + 1; j < program.Count; j++)
			{
				//Проверка на то, что достигли переопределения переменной def,
				//но чтобы была возможность протащить в это определение копию, цикл прервём в конце тела цикла.
				//Для этого создан флаг same_def
				if (program[j].result == def)
					same_def = true;

				if (program[j].arg1 != null && program[j].arg1.ToString() == def)
					program[j].arg1 = newArg;

				if (program[j].arg2 != null && program[j].arg2.ToString() == def)
					program[j].arg2 = newArg;

				if (same_def)
					break;
			}
		}
		return new LinkedList<ThreeCode>(program);
	}
}
```
Конструктор класса _PullCopies_ принимает в качестве аргумента трёхадресный код программы в виде списка команд трёхадресного кода. 
Метод _Optimize_ оптимизирует код согласно заданию: для всех присваиваний типа: ``` a = x ``` перебираются все последующие команды трёхадресного кода, пока не встретим новое присваивание переменной ```a```, или пока не закончатся команды в программе, и переменная ```a``` заменяется своим значением ```x``` для тех команд, где ```a``` является одним из аргументов команды.
Временные переменные для трехадресного кода ```temp_``` протягиваться не могут. 


### Тесты

Программа до применения оптимизации:
```
a = b
c = b
d = c + a
a = c - a
c = a
```

Программа после применения оптимизации:
```
a = b
c = b
d = b + b
a = b - b
c = a
```