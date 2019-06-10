# Оптимизация суммирования с нулем !heading

### Команда South Park

#### Постановка задачи

Задача состояла в реализации оптимизации AST-дерева вида 0+expr

#### Зависимости задач в графе задач

Данная задача не зависит от других задач в графе задач и не порождает новых зависимостей.

#### Теория
При возникновении узла _BinOpNode_, один из аргументов которого является нулём, необходимо удалить ноль, а также осуществить обход дерева внутрь ненулевого выражения.

#### Особенности реализации

```csharp
    class PlusNonZero : AutoApplyVisitorInterface
    {
        public override void VisitBinOpNode(BinOpNode binop)
        {
            if (binop.Left is IntNumNode && (binop.Left as IntNumNode).Num == 0 &&
            binop.Op[0] == '+')
            {
                binop.Right.Visit(this); 
                ReplaceExpr(binop, binop.Right); 
            }
            else
            {
                if (binop.Right is IntNumNode && (binop.Right as IntNumNode).Num == 0 &&
            binop.Op[0] == '+')
                {
                    binop.Left.Visit(this);
                    ReplaceExpr(binop, binop.Left);
                }
                else
                {
                    base.VisitBinOpNode(binop);
                }
                
            }
        }
    }
```
Класс _PlusNonZero_ является визитором и реализует интерфейс _AutoApplyVisitorInterface_. При посещении узла проверяется, является ли левый оператор целым нулевым числом, а операция-сложением. Если да, то происходит обход правого оператора и замена всего узла на правый оператор. Иначе происходит та же проверка на правый оператор. Если нужной ситуации не нашлось, продолжается обход дерева в обычном режиме.

#### Тесты
Пример входной программы:
```
{
    real b;
    int c;
    b=3.14+0;
    c = 0;
}
```
Пример программы после применения оптимизации:
```
{
    var b;
    var c;
    b = 3.14;
    c = 0;
}
```

[Вверх](#содержание)
