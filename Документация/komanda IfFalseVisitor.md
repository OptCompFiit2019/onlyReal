# Удаление недостижимого кода в операторе if с условием = false

Удаление недостижимого кода в операторе if с условием = false


## Komanda

### Постановка задачи
Задача состояла в удалении недостижимого кода в операторе if с условием = false

## Зависимости в графе задач

## Теория

### Особенности реализации
    namespace SimpleLang.Visitors
    {
        class IfFalseVisitor : ChangeVisitor
        {
            public override void VisitIfNode(IfNode ifn)
            {
                if (ifn.Cond is BooleanNode boolVal && !boolVal.Val)
                {
                    ReplaceStat(ifn, ifn.Else);
                }
            }
        }
    }

Для узла if проверяется, чтобы значение условия являлось констаной false, оператор if заменяется блоком else

### Тесты

исходный код:

	int a;
	if (false)
		a = 1;
	else
		a = 2;

результат:

    int a;
    a = 2;