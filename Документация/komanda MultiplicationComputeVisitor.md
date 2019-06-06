# Замена умножения констант на их произведение

Замена умножения констант на их произведение

## Komanda

### Постановка задачи
Задача состояла в замене умножения констант на их произведение

## Зависимости в графе задач

## Теория

### Особенности реализации

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

Для каждого выражения вызываем визитор рекурсивно и при встрече умножения двух констант заменяем их на произведение

### Тесты

исходный код:

    int a;
    a = 2 * 3;

результат:

    int a;
    a = 6;