### Название задачи

Для достигающих определений вычислить genB, killB для любого B.


### Название команды

Intel



### Постановка задачи

Задача состояла вычислении genB, killB и передаточной функции для реализации итерационного алгоритма для достигающих определений.



### Зависимости задач в графе задач

Оптимизация протяжка const на основе инф., полученной в результате применения ИтА для достиг. Определений


### Теория
Будем говорить, что определение d достигает точки p, если существует путь от точки, непосредственно следующей за d, к точке p, такой, что d не уничтожается вдоль этого пути.
genB – множество определений, генерируемых и не переопределённых базовым блоком B.
killB – множество остальных определений переменных, определяемых в определениях genB.


### Особенности реализации

```
    public class GenKillList

    {

        public List<ByteVector> GEN;

        public List<ByteVector> KILL;




        public GenKillList(int blocks_count, int assigns_count)

        {

            GEN = new List<ByteVector>();

            KILL = new List<ByteVector>();




            for (var i = 0; i < blocks_count; ++i)

            {

                GEN.Add(new ByteVector(assigns_count));

                KILL.Add(new ByteVector(assigns_count));

            }

        }




        public GenKillList Generate(List<BlockVariables> blocks)

        {

            for (var i = 0; i < blocks.Count; ++i)

                for (var k = 0; k < blocks[i].Count(); ++k)

                {

                    GEN[i].v[blocks[i].variable_nums[k]] = 1;




                    for (var j = 0; j < blocks.Count; ++j)

                        if (i != j)

                            foreach (var in_var_num in blocks[j].In(blocks[i].variables_names[k]))

                                KILL[i].v[in_var_num] = 1;

                }

            return this;

        }

    }




    public class TransferFunction

    {

        public ByteVector Apply(ByteVector GEN, ByteVector KILL, ByteVector IN)

        {

            return GEN + (IN - KILL);

        }




    }

```


### Тесты

Программа до применения оптимизации:
```
       { 
       int i, j, m, n, a, u1, u2, u3, t, x; 
/*d1*/ 	i = m - 1; 
/*d2*/ 	j = n; 
/*d3*/ 	a = u1; 
       while (true) 
       { 
/*d4*/ 		i = i + 1; 
/*d5*/ 		j = j - 1; 
       if (false) 
       { 
/*d6*/ 			a = u2; 
       } 
/*d7*/ 		i = u3; 
       } 
       } 
```
Пример формирования множеств genB и killB: 
```
GenB
Block 1: {d1, d2, d3}
Block 2: {d4, d5}
Block 3: {d6}
Block 4: {d7}

KillB
Block 1: {d4, d5, d6, d7}
Block 2: {d1, d2, d7}
Block 3: {d3}
Block 4: {d1, d4} 
```



