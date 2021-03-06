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