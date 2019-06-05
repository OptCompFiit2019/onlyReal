### Название задачи

Трехадресный код, удаление мертвого кода


### Название команды

Intel


### Постановка задачи

Задача состояла в реализации локальной оптимизации «удаление мертвого кода».


### Зависимости задач в графе задач

Данная задача не зависит от других задач в графе задач и не порождает новых зависимостей.


### Теория

Данная оптимизация заключается в том, чтобы удалять мертвые код, то есть код, который может быть исполнен, но результаты его вычислений не влияет на дальнейшую программу. 


### Особенности реализации

```
        public void DeleteDeadCode() {
            string a;
            bool abool;
            List<int> removeIndexList = new List<int>();
            var list = Program.ToList();
            int i = list.Count - 1;
            while (i > 0) {
                bool arg2IsNull = true;
                if (list[i].arg2 != null) {
                    a = list[i].arg2.ToString();
                    abool = true;
                    Deleting(list, i, ref removeIndexList, a, abool);
                    arg2IsNull = false;
                }

                a = list[i].arg1.ToString();
                abool = true;
                Deleting(list, i, ref removeIndexList, a, abool);

                if (arg2IsNull) {
                    if (list[i].result != list[i].arg1.ToString()) {
                        a = list[i].result;
                        abool = false;
                        Deleting(list, i, ref removeIndexList, a, abool);
                    }
                } else {
                    if (list[i].result != list[i].arg1.ToString() && list[i].result != list[i].arg2.ToString()) {
                        a = list[i].result;
                        abool = false;
                        Deleting(list, i, ref removeIndexList, a, abool);
                    }
                }
                i--;
            }
            List<ThreeCode> newlist = new List<ThreeCode>();
            for (int ii = 0; ii < Program.Count; ii++) {
                if (removeIndexList.IndexOf(ii) == -1)
                    newlist.Add(list[ii]);
            }
            Program = new LinkedList<ThreeCode>(newlist);
        }


```

Конструктор класса DeleateOfDeadCode принимает в качестве аргумента трёхадресный код программы в виде списка команд трёхадресного кода. 
Функция DeleateOfDeadCode() идет с конца списка команд и запоминает по очереди переменные arg1, arg2 (если есть) и result. Если переменная встречается в правой части, она живая, если встречается в левой – мертвая. Если уже мертвую переменную встретили в левой части, то эту строку нужно удалить из списка команд и в removeIndexList добавляется индекс строки, так как в данной реализации удаление всех мертвых команд происходит в конце.  


### Тесты

Программа до применения оптимизации:
```
a = b
c = 0
d = 1
e = b
a = x - y
k = a
```

Программа после применения оптимизации:
```
c = 0
d = 1
e = b
a = x - y
k = a
```
