
# Оптимизация по AST-дереву №13 

(AST-дерево: оптимизация №13)

 ## qwerty

 ### Постановка задачи

Задача состояла в реализации оптимизации по AST-дереву условных операторов вида: 
``` 
if(expression)
	null;
else
	null;
```
 ### Зависимости задач в графе задач

Данная задача не зависит от других задач в графе задач и не порождает новых зависимостей.

### Теория

Данная оптимизация заключается в том, чтобы заменять любой условный оператор вида
``` 
if(expression)
	null;
else
	null;
```
на выражение ``` null```, так как при любом ```expression``` в условном операторе будет получен ``` null```.
### Особенности реализации

```csharp
class OptVisitor_13 : ChangeVisitor2  
{  
    public override void VisitBlockNode(BlockNode bl)  
    {  
        for (int i = 0; i < bl.StList.Count; i++)  
            if (bl.StList[i] is IfNode ifn)  
            {  
                var stlist1 = ifn.If as BlockNode;  
                var stlist2 = ifn.Else as BlockNode;  
                bool null1, null2;  
                null1 = null2 = false;  
                if (stlist1.StList.Count == 1 & stlist1.StList[0] is NullNode)  
                    null1 = true;  
                if (stlist2.StList.Count == 1 & stlist2.StList[0] is NullNode)  
                    null2 = true;  
  
                if (null1 && null2)  
                    bl.StList[i] = new NullNode();  
                else  
                    base.VisitIfNode(ifn);  
            }  
    }  
}
```
Класс _OptVisitor_13_ перегружает метод _VisitBlockNode_, который при посещении узла AST-дерева, являющегося узлом условного оператора вида: 
``` 
if(expression)
	null;
else
	null;
```
 заменяет этот узел на   ```NullNode()```.

### Тесты
Код до применения оптимизации:
```
if(a!=b)
	null;
else
	null;
```

Код после применения оптимизации:
```
null;
```
