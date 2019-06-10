# Удаление мертвого кода и протяжка копий !heading

### Команда Intel

## Протяжка копий !heading

#### Постановка задачи

Задача состояла в реализации локальной оптимизации «протяжка копий».

#### Зависимости задач в графе задач

Данная задача не зависит от других задач в графе задач и не порождает новых зависимостей.

#### Теория

Данная оптимизация заключается в том, чтобы заменять переменную в правой части, на значение этой же переменной в строке, где она была определена и далее не использовалась. 


#### Особенности реализации

```
        public void PullCopies() {
            var list = Program.ToList();
            for (int i = 0; i < Program.Count - 1; i++) {

                string left;
                ThreeAddressValueType right;

                if (list[i].arg2 == null) {
                    left = list[i].result;
                    right = list[i].arg1;
                } else continue;

                for (int j = i + 1; j < Program.Count; j++) {
                    if (list[j].result == left)
                        break;
                    if (list[j].arg1.ToString() == left)
                        list[j].arg1 = right;

                    if (list[j].arg2 != null && list[j].arg2.ToString() == left)
                        list[j].arg2 = right;
                }
            }
        }
```
Конструктор класса PullofCopies_ принимает в качестве аргумента трёхадресный код программы в виде списка команд трёхадресного кода. 
Функция PullCopies() для всех строк, где нет arg2, запоминает значение result (left) и arg1 (right), после этого перебирает все следующие команды, пока они не закончатся или не будет найдено новое определение left, то есть команда, где result = left. Если метод встречает arg1 = left или arg2 = left, то выполняет замену arg1 или arg2 на значение right соотвественно. 


#### Тесты

Программа до применения оптимизации:
```
a = b
c = 0
d = c + 1
e = d * b
a = x - y
k = c + a
```
Программа после применения оптимизации:
```
a = b
c = 0
d = 0 + 1
e = d * b
a = x - y
k = 0 + a
```

## Удаление мертвого кода !heading

#### Постановка задачи

Задача состояла в реализации локальной оптимизации «удаление мертвого кода».

#### Зависимости задач в графе задач

Данная задача не зависит от других задач в графе задач и не порождает новых зависимостей.

#### Теория

Данная оптимизация заключается в том, чтобы удалять мертвые код, то есть код, который может быть исполнен, но результаты его вычислений не влияет на дальнейшую программу. 


#### Особенности реализации

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


#### Тесты

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

[Вверх](#содержание)