# Выделение Базовых Блоков !heading

### Команда Nvidia

#### Постановка задачи
Требуется по трехадресному коду построить список базовых блоков.

#### Зависимости задач в графе задач
Задача зависит от:
- генерации трехадресного кода.

#### Теория
Базовый блок - это максимальная последовательность команд трехадресного кода, удовлетворяющая следующим условиям:
- поток управления может входить в ББл только через первую команду 
- управление покидает ББл без останова или ветвления, за исключением, возможно, последней команды
Для находления базовых блоков необходимо найти все команды-лидеры, которыми явзяются:
- первая команда
- любая команда, на которую есть переход
- любая команда, непосредственно следующая за переходом

Базовый блок - это блок команд от лидера до лидера.

#### Особенности реализации
```
	public class Block
	{
        public LinkedList<ThreeCode> code;
        public Block(ThreeAddressCodeVisitor _code)
        {
            this.code = _code.GetCode();
        }

        public List<int> FindLeaders()
        {
            var Leaders = new List<int>();
            int i = 1;

            bool PreviousIsGoto = false;

            foreach (var line in this.code)
            {
                if (i == 1)
                    Leaders.Add(i);
                else
                    if (!String.IsNullOrEmpty(line.label))
                        Leaders.Add(i);
                    else
                        if (PreviousIsGoto)
                            Leaders.Add(i);

                PreviousIsGoto = line.operation == ThreeOperator.Goto || line.operation == ThreeOperator.IfGoto;
                
                i += 1;
            }

            return Leaders;
        }

        public List<LinkedList<ThreeCode>> GenerateBlocks()
        {
            var Leaders = FindLeaders();
            int i = 1;
            int LiderInd = 0;
            
            var Blocks = new List<LinkedList<ThreeCode>>();

            foreach (var line in this.code)
            {
                if (LiderInd < Leaders.Count && i == Leaders[LiderInd])
                {
                    Blocks.Add(new LinkedList<ThreeCode>());
                    LiderInd += 1;
                }
                Blocks.Last().AddLast(line);
                i += 1;
            }

            return Blocks;
        }
    }
```
Был определен класс `Block`, инициализирующийся трехадресным кодом, с методом `GenerateBlocks`, который возращает `List<LinkedList<ThreeCode>>`, то есть, список блоков.
[Вверх](#содержание)