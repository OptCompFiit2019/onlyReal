DeletePlusZero (файл PlusNonZero)
Оптимизация по дереву вида 0+expr
Создать визитор opt, посетить его:
PlusNonZero opt = new PlusNonZero();
r.Visit(opt);
Вывести что получилось можно вызвав после этого PrettyPrintVisitor:
PrettyPrintVisitor ppvis = new PrettyPrintVisitor();
r.Visit(ppvis);
Console.WriteLine(ppvis.Text);