# Название задачи
Оптимизация тождественного оператора присваивания вида `x = x`.

# Название команды
Nvidia

# Постановка задачи
Требуется найти в AST-дереве операторы присваивания, оба операнда которых являются одной переменной на этапе компиляции и заменить их на `null`.

# Зависимости задач в графе задач
Данная задача не зависит от других задач в графе задач и не порождает новых зависимостей.

# Теория
В исходном коде программы возникают случаи, когда операторы присваивания не выполняют своего прямого назначения и замедляют программу. Например, когда переменной присваивается она сама. На этапе компиляции можно выявить такие операторы и произвести оптимизацию.

# Особенности реализации
```
public class OptSimilarAssignment : AutoApplyVisitorInterface
    {
        public override void VisitAssignNode(AssignNode a)
        {
            if ((a.Expr is IdNode) && String.Equals(a.Id.Name, (a.Expr as IdNode).Name))
            {
                ReplaceStat(a, new EmptyNode());
            }
            else
            {
                base.VisitAssignNode(a);
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
Был реализован класс `OptSimilarAssignment`, который переопределяет процедуру `VisitAssignNode`. Оптимизация происходит в случае, если оба операнда являются `IdNode` и их имена равны.

# Тесты
Программа до применения оптимизации:
```
```
Программа после применения оптимизации:
```
```