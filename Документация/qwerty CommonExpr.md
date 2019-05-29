# Оптимизацияптимизация общих подвыражений (простейшая)

(Трёхадресный код: оптимизация общих подвыражений (простейшая))

 ## qwerty

 ### Постановка задачи

Задача состояла в реализации локальной оптимизации общих подвыражений. 

 ### Зависимости задач в графе задач

Данная задача не зависит от других задач в графе задач и не порождает новых зависимостей.

### Теория

Данная оптимизация заключается в том, чтобы при присваивании переменной **Y** выражения **EXPR**,  которое уже встречалось при определении переменной **X** (**X = EXPR**), заменять **Y = EXPR** на **Y = X**.
При этом все переменные, входящие в **EXPR**, не должны переопределяться между командами **X = EXPR** и **Y = EXPR**, иначе подвыражение не подлежит оптимизации. 

### Особенности реализации

```csharp
class CommonExpr
{
	public List<ThreeCode> program;

	public CommonExpr(List<ThreeCode> prog)
	{
		program = MakeProgram(prog);
	}

	public List<ThreeCode> MakeProgram(List<ThreeCode> prog)
	{
		var program = new List<ThreeCode>();
		int i = 0;
		for (i = 0; i < prog.Count - 1; i++)
		{
			if (prog[i + 1].arg1 != null && prog[i].result.Equals(prog[i + 1].arg1.ToString()) && prog[i].result.Contains("temp_") && prog[i + 1].arg2 == null)
			{
				if (prog[i].label != "")
					program.Add(new ThreeCode(prog[i].label, prog[i + 1].result, prog[i].operation, prog[i].arg1, prog[i].arg2));
				else
					program.Add(new ThreeCode(prog[i + 1].result, prog[i].operation, prog[i].arg1, prog[i].arg2));
				i++;
			}
			else
				program.Add(prog[i]);
		}
		if (i == prog.Count - 1)
			program.Add(prog[i]);
		return program;
	}

	public LinkedList<ThreeCode> Optimize()
	{
		var result = new List<ThreeCode>();

		for (int i = 0; i < program.Count - 1; i++)
		{
			if (program[i].arg1 == null && program[i].arg2 == null)
				continue;
			else
			{
				for (int j = i + 1; j < program.Count; j++)
				{
					if (program[j].arg1.ToString() == program[i].arg1.ToString() && program[j].arg2 != null && program[i].arg2 != null && program[j].arg2.ToString() == program[i].arg2.ToString() && program[j].operation == program[i].operation)
					{
						program[j].operation = ThreeOperator.Assign;
						program[j].arg1 = new ThreeAddressStringValue(program[i].result);
						program[j].arg2 = null;
					}
					if (program[j].result == program[i].result || program[j].result == program[i].arg1.ToString() || (program[i].arg2 != null && program[j].result == program[i].arg2.ToString()))
						break;
				}
			}
		}

		for (int i = 0; i < program.Count; i++)
			result.Add(program[i]);
		return new LinkedList<ThreeCode>(result);
	}
}
```
Конструктор класса _CommonExpr_ принимает в качестве аргумента трёхадресный код программы в виде списка команд трёхадресного кода. В конструкторе вызывается метод _MakeProgram_, которая убирает лишние переменные ```temp_``` где это возможно, чтобы улучшить читаемость кода и для удобства применении оптимизации. Например
До вызова _MakeProgram_:
```
temp_1 = b + c
a = temp_1
```
После вызова _MakeProgram_:
```
a = b + c
```
Метод _Optimize_ оптимизирует код согласно заданию: для всех присваиваний типа: ```x = expr``` перебираются все последующие команды трёхадресного кода, пока не встретим новое присваивание ```y = expr```, или пока не закончатся команды в программе, и присваивание ```y = expr``` заменяется на ```y = x```, если переменные, входящие в ```expr```, не переопределялись на пути от ```x``` к ```y```.


### Тесты

Трёхадресный код до применения оптимизации:
```
a = b + c
b = a - d
c = b + c
d = a - d
```

Трёхадресный код после применения оптимизации:
```
a = b + c
b = a - d
c = b + c
d = b
```