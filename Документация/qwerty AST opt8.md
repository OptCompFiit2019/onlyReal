
# Оптимизация по AST-дереву №8 

(AST-дерево: оптимизация №8)

 ## qwerty

 ### Постановка задачи

Задача состояла в реализации оптимизации по AST-дереву выражений вида: ``` a==a``` и ``` a>=a```.

 ### Зависимости задач в графе задач

Данная задача не зависит от других задач в графе задач и не порождает новых зависимостей.

### Теория

Данная оптимизация заключается в том, чтобы заменять любое выражение вида ``` a==a``` или ``` a>=a``` на значение ```true```, так как эти выражения всегда дают истину.

### Особенности реализации

```csharp
class OptVisitor_8 : ChangeVisitor2  
{  
    public override void VisitBinOpNode(BinOpNode binop)  
    {  
        if ((binop.Left is IdNode) && (binop.Right is IdNode) &&  
            (binop.Left as IdNode).Name == (binop.Right as IdNode).Name &&  
            (binop.Op == "==" || binop.Op == ">="))  
        {  
            if (binop.Parent is IfNode ifn)  
                ifn.Cond = new BooleanNode(true);  
            else if (binop.Parent is WhileNode w)  
                w.Expr = new BooleanNode(true);  
            else  
                ReplaceExpr2(binop, new BooleanNode(true));  
        }  
        else if ((binop.Left is ExprNode) && (binop.Right is ExprNode) &&  
                 (binop.Left.ToString() == binop.Right.ToString()) &&  
                 (binop.Op == "==" || binop.Op == ">="))  
        {  
            if (binop.Parent is IfNode ifn)  
                ifn.Cond = new BooleanNode(true);  
            else if (binop.Parent is WhileNode w)  
                w.Expr = new BooleanNode(true);  
            else  
                ReplaceExpr2(binop, new BooleanNode(true));  
        }  
        else    
            base.VisitBinOpNode(binop); // Обойти потомков обычным образом  
    }  

    public override void VisitIfNode(IfNode ifn)  
    {  
        ifn.Cond.Visit(this);  
    }  

    public override void VisitWhileNode(WhileNode w)  
    {  
        w.Expr.Visit(this);  
    }  
}
```
Класс _OptVisitor_8_ перегружает метод _VisitBinOpNode_, который при посещении узла AST-дерева, содержащего бинарное выражение типов: ```a==a``` или ```a>=a```, заменяет это выражение на        ```BooleanNode(true)```  с помощью метода _ReplaceExpr2_.

### Тесты
Код до применения оптимизации:
```
if(a==a)
	x = x * 10;
```

Код после применения оптимизации:
```
if(true)
	x = x * 10;
```
