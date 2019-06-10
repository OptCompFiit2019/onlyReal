# Оптимизация операций больше, не равно !heading

### Команда Komanda

#### Постановка задачи
Задача состояла в замене логических выражений x != x, x > x на false

#### Зависимости в графе задач

Данная задача не зависит от других задач в графе задач и не порождает новых зависимостей

#### Теория

Логические выражения x != x и x > x равны false при любых значениях x, поэтому можно выполнять замену.

#### Особенности реализации

    using ProgramTree;
    using SimpleLang.Visitors;
    
    namespace SimpleLang.AstOptimisations
    {
        public class FalseExprMoreAndNonEqualVisitor : AutoApplyVisitorInterface
        {
            public override void VisitLogicOpNode(LogicOpNode lop)
            {
                if ((lop.Operation == ">" || lop.Operation == "!=") && lop.Right.ToString() == lop.Left.ToString())
                {
                    ReplaceExpr(lop, new BooleanNode(false));
                }
            }
        }
    }
    

Для всех логических операций проверяем, чтобы бинарная операция была = ">" или "!=" и левый и правый операнды были равны  

#### Тесты

исходный код:

    int a;
	bool b;
    b = a > a;
	b = a != a;

результат:

    int a;
    bool b;
    b = false;
    b = false;
    
[Вверх](#содержание)