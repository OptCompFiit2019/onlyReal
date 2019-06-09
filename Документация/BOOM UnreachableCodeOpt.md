### Название задачи

Трехадресный код, устранение недостижимого кода


### Название команды

BOOM


### Постановка задачи

Задача состояла в реализации локальной оптимизации «Устранение недостижимого кода».


### Зависимости задач в графе задач

Данная задача не зависит от других задач в графе задач и не порождает новых зависимостей.


### Теория

Данная оптимизация заключается в том, чтобы удалить недостижимый код, то есть код, который не может быть достигнут.


### Особенности реализации

```
private HashSet<object> FindAllTargetLabels(LinkedList<ThreeCode> tac)
{
    var setOfLables = new HashSet<object>();
    foreach (var line in tac) {
        if (line.operation is ThreeOperator.IfGoto)
            setOfLables.Add(line.arg2);
        if (line.operation is ThreeOperator.Goto)
            setOfLables.Add(line.arg1);
    }
    return setOfLables;
}


private bool CheckLabels(LinkedListNode<ThreeCode> ifNode, object targetLabel, List<ThreeCode> NodesToRemove)
{
    var currentNode = ifNode;
    var line = currentNode.Value;

    while (!Equals(line.label, targetLabel))
    {
        NodesToRemove.Add(line);
        currentNode = currentNode.Next;
        line = currentNode.Value;
    }

    return true;
}

public ThreeCode ConvertIfGotoToGoto(ThreeCode code)
{
    var new_code = code;
    new_code.arg1 = code.arg2;
    new_code.arg2 = null;
    new_code.operation = ThreeOperator.Goto;
    return new_code;
}


public LinkedList<ThreeCode> DeleteUnreachCode(LinkedList<ThreeCode> code)
{
    var targetLabels = FindAllTargetLabels(code);

    var currentNode = code.First;
    var linesToDelete = new List<ThreeCode>();

    while (currentNode != null)
    {
        var line = currentNode.Value;

        if (line.operation is ThreeOperator.IfGoto && line.arg1.ToString().Equals("True"))
        {
            if (CheckLabels(currentNode.Next, line.arg2.ToString(), linesToDelete))
            {
                line = ConvertIfGotoToGoto(line);
                foreach (var line_del in linesToDelete)
                    code.Remove(line_del);
                linesToDelete.Clear();
                _apply = true;
            }
        }

        currentNode = currentNode.Next;
    }

    return code;
}



```

Конструктор класса UnreachableCodeOpt принимает в качестве аргумента трёхадресный код программы в виде списка команд трёхадресного кода.
Функция FindAllTargetLabels(LinkedList<ThreeCode> tac) находит все метки, по которым в коде делаются переходы. Проходим по списку команд, если встречаем условный оператор с переходом по метке, то проверяем все команды после до тех пор, пока не встретим нужную метку, добавляя все рассмотренные команды в список для дальнейшего удаления. После этого удаляем ненужные команды.


### Тесты

Программа до применения оптимизации:
```
{
    int a,b,c,d;
    a = 5;
    b = 3;
    if (true){
	      c = 2;
    }
    d = 7;
    if (true){
	      a = a + 1;
    }
}
```

Программа после применения оптимизации:
```
{
    int a,b,c,d;
    a = 5;
    b = 3;
    if (true){
	      c = 2;
    }
    d = 7;
    if (true){
	      a = a + 1;
    }
}
```

Однако трехадресный код результирующей программы изменится.
```
a = 5
b = 3
goto label_0
label_0:   
c = 2
label_1:   d = 7
goto label_2
label_2:   
a = a + 1
label_3:    
```
