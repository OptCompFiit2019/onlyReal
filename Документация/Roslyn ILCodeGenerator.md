# Генерация IL-кода

(Генерация IL-кода по трёхадресному коду программы.)

### Roslyn

#### Постановка задачи

Требуется произвести генерацию IL-инструкций на основе программного кода, представленного в виде трёхадресных выражений.

#### Зависимости задач в графе задач
Данная задача зависит от построения трёхадресного кода.

От задачи зависит:
* Задача не порождает новых зависимостей

#### Теория

* IL-код основан на стековой модели, регистры отсутствуют. Чтобы произвести вычисления. значения вначале кладутся на стек, а потом над верхними значениями совершается операция, при этом они снимаются со стека и на стек кладется результат.
* OpCodes содержит около сотни кодов команд кода IL (Intermediate Language). Основные:
    - OpCodes.Ldc_I4 - загружает в стек целое значение
    - OpCodes.Ldc_R8 - загружает в стек вещественное значение
    - OpCodes.Ldloc - загружает в стек локальную переменную
    - OpCodes.Stloc - извлекает из стека верхнее значение и помещает его в локальную переменную
    - OpCodes.Ldarg - загружает в стек параметр функции
    - OpCodes.Starg - извлекает из стека верхнее значение и помещает его в параметр функции
    - OpCodes.Add - складывает два значения на вершине стека и помещает результат в стек вычислений
    - OpCodes.Sub - вычитает два значения на вершине стека и помещает результат в стек вычислений
    - OpCodes.Mult - умножает два значения на вершине стека и помещает результат в стек вычислений
    - OpCodes.Div - делит два значения на вершине стека и помещает результат в стек вычислений
    - OpCodes.Ret - выполняет возврат из текущего метода
    - OpCodes.Br - обеспечивает безусловный переход
    - OpCodes.Blt - обеспечивает переход если первый операнд меньше второго 

#### Особенности реализации

Ниже представлен пример использования IL-генератора:
```csharp
SimpleLang.Compiler.ILCodeGenerator gen = new SimpleLang.Compiler.ILCodeGenerator();
gen.Generate(treeCode.GetCode());
gen.PrintCommands();
Console.WriteLine("\nExecute:");
gen.Execute();
```
Функция _PrintCommands_ выводит на экран сгенерированные IL-инструкции. Функция _Execute_ запускает выполнение программы.

#### Тесты

Пример исходной программы:
```
{
	bool a, b;
	real c;
	int d;
	a = true;
	b = !(!a);
	c = 1.1;
	if ((a==true) && (b == false))
	{
		c=3.14;
		d = 1;
	}
	else
		d=2;
	println(c);
	println(d);
}
```
Сгенерированные по ней инструкции IL-кода:
```
DeclareLocal a: System.Boolean
DeclareLocal b: System.Boolean
DeclareLocal c: System.Double
DeclareLocal d: System.Int32
DeclareLocal temp_1: System.Boolean
DeclareLocal temp_0: System.Boolean
DeclareLocal temp_3: System.Boolean
DeclareLocal temp_4: System.Boolean
DeclareLocal temp_2: System.Boolean
DefineLabel Label0
DefineLabel Label1
ldc.i4.1
stloc a
ldloc a
ldc.i4.1
xor
stloc temp_1
ldloc temp_1
ldc.i4.1
xor
stloc temp_0
ldloc temp_0
stloc b
ldc.r8 1,1
stloc c
ldloc a
ldc.i4.1
ceq
stloc temp_3
ldloc b
ldc.i4.0
ceq
stloc temp_4
ldloc temp_3
ldloc temp_4
and
stloc temp_2
ldloc temp_2
brtrue Label0
ldc.i4 2
stloc d
br Label1
MarkLabel Label0
ldc.r8 3,14
stloc c
ldc.i4 1
stloc d
MarkLabel Label1
ldloc c
WriteLine
ldloc d
WriteLine
```

Пример запуска:
```
Execute:
1,1
2
```



