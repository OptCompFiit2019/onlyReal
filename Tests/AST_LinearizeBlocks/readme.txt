LinearizeBlocks 
Оптимизация по дереву: все блоки, вложенные в блоки, сделать линейными
Создать визитор opt, посетить его:
SimpleLang.AstOptimisations.LinearizeBlocks opt = new SimpleLang.AstOptimisations.LinearizeBlocks();
r.Visit(opt);
Вывести что получилось можно вызвав после этого Console.WriteLine(r);