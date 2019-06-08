### Название задачи

Трехадресный код, устранение переходов к переходам


### Название команды

BOOM


### Постановка задачи

Задача состояла в реализации локальной оптимизации «Устранение переходов к переходам».


### Зависимости задач в графе задач

Данная задача не зависит от других задач в графе задач и не порождает новых зависимостей.


### Теория

Данная оптимизация заключается в том, чтобы устранить переходы к переходам, поскольку это получается дополнительная операция, от которой результат все равно не меняется.


### Особенности реализации

```
private List<LinkedListNode<ThreeCode>> FindGotoNodes(LinkedList<ThreeCode> code)
{
    var currentNode = code.First;
    var gotoNodes = new List<LinkedListNode<ThreeCode>>();
    var res = new List<LinkedListNode<ThreeCode>>();
    var targetLabels = new Dictionary<string, int>();

    while (currentNode != null)
    {
        var currentValue = currentNode.Value;

        if (currentValue.operation == ThreeOperator.Goto)
        {
            var tacGoto = currentValue;
            var label = tacGoto.arg1.ToString();
            if (targetLabels.ContainsKey(label))
                targetLabels[label]++;
            else targetLabels.Add(label, 1);

            gotoNodes.Add(currentNode);
        }

        currentNode = currentNode.Next;
    }

    var usingTargets = new HashSet<string>(targetLabels
       .Where(x => x.Value == 1)
       .Select(x => x.Key));

    foreach (var node in gotoNodes)
    {
        var gotoNode = node.Value;
        if (usingTargets.Contains(gotoNode.arg1.ToString()))
            res.Add(node);
    }

    return gotoNodes;
}

public LinkedListNode<ThreeCode> FindLabel(LinkedList<ThreeCode> code, string lbl)
{
    var currentNode = code.First;
    while (currentNode != null)
    {
        var line = currentNode.Value;

        if (Equals(line.label, lbl))
        {
            return currentNode;
        }
        currentNode = currentNode.Next;
    }
    return null;
}

public LinkedListNode<ThreeCode> ConvertGotoToIfGotoWithoutLabel(LinkedListNode<ThreeCode> IfGoto)
{
    var res = IfGoto;
    res.Value.label = null;
    return res;
}

public LinkedList<ThreeCode> TranToTranOpt(LinkedList<ThreeCode> code)
{
    var currentNode = code.First;
    var gotoNodes = FindGotoNodes(code);
    LinkedListNode<ThreeCode> temp;

    while (currentNode != null)
    {
        var line = currentNode.Value;

        if (line.operation is ThreeOperator.Goto && (temp = FindLabel(code, line.arg1.ToString())).Value.operation is ThreeOperator.Goto)
        {
            line.arg1 = temp.Value.arg1;
            _apply = true;
        }

        if (line.operation is ThreeOperator.IfGoto && (temp = FindLabel(code, line.arg2.ToString())).Value.operation is ThreeOperator.Goto)
        {
            line.arg1 = temp.Value.arg1;
            _apply = true;
        }

        currentNode = currentNode.Next;
    }

    foreach (var gotoNode in gotoNodes)
    {
        var gotoValue = gotoNode.Value;
        var gotoNode2 = gotoNode;

        var targetNode = FindLabel(code, gotoValue.arg1.ToString());
        if (targetNode == null || targetNode.Value.operation != ThreeOperator.IfGoto)
            continue;

        var nextNode = targetNode.Next;
        if (nextNode == null || nextNode.Value.label == "")
            continue;

        gotoNode2 = ConvertGotoToIfGotoWithoutLabel(targetNode);
        code.Find(gotoNode.Value).Value = gotoNode2.Value;
        var label = new ThreeAddressStringValue(nextNode.Value.label);
        code.AddAfter(gotoNode2, new ThreeCode("", ThreeOperator.Goto, label));
        code.Remove(targetNode);
        _apply = true;
    }

    return code;
}


```

Конструктор класса EliminationTranToTranOpt принимает в качестве аргумента трёхадресный код программы в виде списка команд трёхадресного кода.
Функция FindGotoNodes(LinkedList<ThreeCode> code) находит все команды, содержащие переходы Goto. Проходим по списку команд, если встречаем переходы типов Goto или IfGoto, то проверяем, содержит ли команда по этой метке другой переход. Если да, то устраняем переход к переходу. После этого проходим по всем командам содержащим Goto и находим команду, к которой перешли по метке. Если это IfGoto, то проверяем, что следующая за ней команда содержит метку. Если все условия выполнены, то заменяем Goto на IfGoto и заменяем метку у перехода Goto.


### Тесты

Программа до применения оптимизации:
```
{
    while(false){
        while(true){
	          println(1);
	      }
    }
}
```

Программа после применения оптимизации:
```
{
    while(false){
        while(true){
	          println(1);
	      }
    }
}
```

Однако трехадресный код результирующей программы изменится.
```
label_0:   temp_0 = False
           if temp_0 goto label_1
           goto label_2
label_1:   temp_1 = True
           if temp_1 goto label_3
           goto label_0
label_3:   println 1
           goto label_1
label_4:   goto label_0
label_2:   
```
