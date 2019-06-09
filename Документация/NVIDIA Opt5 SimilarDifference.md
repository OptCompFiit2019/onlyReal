# Название задачи
Оптимизация тождественного оператора присваивания вида `a - a`.

# Название команды
Nvidia

# Постановка задачи
Требуется найти в AST-дереве выражения разности, оба операнда которых являются одной переменной на этапе компиляции и заменить их на константу `0`.

# Зависимости задач в графе задач
Данная задача не зависит от других задач в графе задач и не порождает новых зависимостей.

# Теория
В исходном коде программы возникают случаи, когда аргументы оператора разности равны. Такие операторы необходимо заменить на 0.

# Особенности реализации
```
public class OptSimilarDifference : AutoApplyVisitorInterface
    {
        public override void VisitBinOpNode(BinOpNode binop)
        {
            if ((binop.Left is IdNode) && (binop.Right is IdNode) &&
                String.Equals((binop.Left as IdNode).Name, (binop.Right as IdNode).Name) &&
                (binop.Op == "-"))
            {
                ReplaceExpr(binop, new IntNumNode(0));
            }
            else
            {
                base.VisitBinOpNode(binop); // Обойти потомков обычным образом
            }
        }
        public override void VisitIfNode(IfNode ifn)
        {
            ifn.Cond.Visit(this);
        }
        public override void VisitWhileNode(WhileNode w)
        {
            w.Expr.Visit(this);
        }

        public override string ToString()
        {
            return "";
        }
    }
```
Был реализован класс `OptSimilarDifference`, который переопределяет процедуру `VisitBinOpNode`. Оптимизация происходит в случае, если оба операнда являются `IdNode`, их имена равны и бинарная операция - это разность.

# Тесты
Программа до применения оптимизации:
```
```
Программа после применения оптимизации:
```
```