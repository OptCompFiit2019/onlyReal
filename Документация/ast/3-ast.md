# Оптимизация перемножения констант !heading

### Команда Komanda

#### Постановка задачи
Задача состояла в замене умножения констант на их произведение

#### Зависимости в графе задач
Данная задача не зависит от других задач в графе задач и не порождает новых зависимостей

#### Теория
Очевидно, что замена выражения вида const*const на значение этого произведения является эквивалентным преобразованием

#### Особенности реализации
```csharp
    namespace SimpleLang.Visitors
    {
        class MultiplicationComputeVisitor : ChangeVisitor
        {
            public override void VisitBinOpNode(BinOpNode binop)
            {
                if (binop.Left is ExprNode el)
                    el.Visit(this);
                if (binop.Right is ExprNode er)
                    er.Visit(this);
                if (binop.Left is IntNumNode l && binop.Right is IntNumNode r && binop.Op == "*")
                    ReplaceExpr(binop, new IntNumNode(l.Num * r.Num));
                else
                    base.VisitBinOpNode(binop);
            }
        }
    }
```
Для каждого выражения вызываем визитор рекурсивно и при встрече умножения двух констант заменяем их на произведение

#### Тесты
исходный код:
```csharp
    int a;
    a = 2 * 3;
```
результат:
```csharp
    int a;
    a = 6;
```

[Вверх](#содержание)