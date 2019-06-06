# Тестирующая система

 ## qwerty

 ### Постановка задачи

Задача состояла в реализации системы тестирования алгоритмов. 

 ### Зависимости задач в графе задач

Данная задача зависит от всех задач в графе задач, но не порождает новых зависимостей.

### Теория

Работа системы тестирования заключается в сравнении результата работы программы с ожидаемым результатом.

### Особенности реализации

Вызов теста:

```csharp
var Tester = new TestSystem();
Tester.LaunchTest("DAG");
Tester.LaunchTest("PullCopies");
Tester.LaunchTest("CommonExpr");
Tester.LaunchTest("DefBUseB");
Tester.LaunchTest("JumpThroughJump");
Tester.LaunchTest("DeepSpanTree");
Tester.LaunchTest("AvailableExprs");
Tester.LaunchTest("DeadOrAliveOnGraph");
Tester.LaunchTest("DeadOrAliveOptimization");
Tester.LaunchTest("GraphDepth");
Tester.LaunchTest("IterativeAlgorithm");
Tester.LaunchTest("LVNOptimization");
Tester.LaunchTest("TransferFunction");
Tester.LaunchTest("ApplyAlgebraicIdentities");
Tester.LaunchTest("DistributionOfConstant");
Tester.LaunchTest("ThreeAddressCode");
Tester.LaunchTest("EvalConstExpr");
```
Метод _LaunchTest_ класса _Tester_ принимает в качестве аргумента название оптимизации или алгоритма, который нужно применить. По этому названию находится папка с тестами и запускается работа алгоритма. Результат работы алгоритма сравнивается с ожидаемым результатом. Если что-то не правильно, то система предупредит об этом.


### Тесты

Запуск тестирования оптимизации "протяжка копий":
```
var Tester = new TestSystem();
Tester.LaunchTest("PullCopies");
```

Результат работы тестирующей системы в случае корректной работы алгоритма:
```
Тестирование PullCopies

Тест source1.txt успешно пройден!
Тест source2.txt успешно пройден!
Тест source3.txt успешно пройден!
Тест source4.txt успешно пройден!
Тест source5.txt успешно пройден!
Тест source6.txt успешно пройден!
```

Результат работы тестирующей системы в случае некорректной работы алгоритма:
```
Тест source1.txt успешно пройден!
Тест source2.txt успешно пройден!
Тест source3.txt успешно пройден!
Тест source4.txt не пройден: кол-во команд в ББл #1 не совпадает с ожидаемым
Тест source5.txt не пройден: ошибка в строке #2 в ББл #1
Тест source6.txt успешно пройден!
```