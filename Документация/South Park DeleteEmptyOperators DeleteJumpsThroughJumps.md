# Очистка от пустых операторов, устранение переходов через переходы

(Трехадресный код: очситка от пустых операторов, устранение переходов через переходы)

## South Park

#### Постановка задачи

Задача состояла в реализации локальной оптимизации "очистка от пустых операторов" и в реализации локальной оптимизации "устранение переходов через переходы".

#### Зависимости задач в графе задач

Данная задача не зависит от других задач в графе задач и не порождает новых зависимостей.

#### Теория

Очистка от пустых операторов:
Данная задача состояла в том, чтобы при появлении пустого оператора его удалять.

Устранение переходов через переходы:
Данная задача состояла в том, чтобы удалить возникающие в трехадресном коде переходы через переходы. Эта ситуация возникает в случае оператора ```if```, а именно возникает лишний переход ```goto``` через другой переход ```goto```.

#### Особенности реализации
(код и описанием к нему)
```csharp
    public class NonZero_JTJ:ThreeCodeOptimiser
    {
        public LinkedList<ThreeCode> code;
        public NonZero_JTJ(ThreeAddressCodeVisitor Code)
        {
            this.code = Code.GetCode();
        }

        public void DeleteEmptyOp() 
        {
            var line = this.code.First;
            while (line.Next != null)
            {
                if (line.Next.Value.arg1 == null && line.Next.Value.arg2 == null && line.Next.Value.result == null)
                {
                    line = line.Next.Next;
                    continue;
                }
                line = line.Next;
            }
        }

        public void DeleteJumpThroughJump() 
        {
            var line = this.code.First;
            string lblOld = "", lblNew = "";
            bool inIf = false;
            while (line != null)
            {
                if (inIf && line.Value.operation == ThreeOperator.Goto && line.Value.arg1.ToString() == lblNew)
                {
                    var nextLine = line.Next;
                    code.Remove(line);
                    line = nextLine;
                    continue;
                }

                if (line.Value.operation == ThreeOperator.IfGoto)
                {
                    inIf = true;

                    var tmp = line.Value.arg1 as ThreeAddressLogicValue;
                    var t = !tmp.Value;
                    (line.Value.arg1 as ThreeAddressLogicValue).Value = t;

                    if (!string.IsNullOrEmpty(line.Value.arg2.ToString()))
                    {
                        var ee = line.Value.arg2.ToString();
                        lblOld = line.Value.arg2.ToString();
                        var nn = line.Next.Value.arg1;
                        (line.Value.arg2 as ThreeAddressStringValue).Value = (line.Next.Value.arg1 as ThreeAddressStringValue).Value;
                        code.Remove(line.Next);
                        code.Remove(line.Next);
                        line = line.Next;                      
                    }
                    line = line.Next.Next;

                    continue;
                }
                line = line.Next;
            }
        }
    }
```
При вызове конструктор класса _NonZero_JTJ_ принимает на вход _ThreeAddressCodeVisitor_ чтобы получить трехадресный код программы. 
Метод _DeleteEmptyOp_ присваивает текущей строке с пустым оператором указатель на следующую строку кода. 
Метод _DeleteJumpThroughJump_ проходит по всему трехадресному коду чтобы найти необходимую ситуацию с лишним ```goto``` (ситуация согласно заданию). Как только он ее находит, меняет логическое значение в узле ```if``` на противоположное и удаляет лишние метки и строки трехадресного кода.

#### Тесты

Трехадресный код входной программы:
```
            if True goto label_0
            goto label_1
label_0:
            b = 3,14
label_1:    c = 0
```

Трехадресный код после применения оптимизаций:
```
            if False goto label_1
            b = 3,14
label_1:    c = 0
```

