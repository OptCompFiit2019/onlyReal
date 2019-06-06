AvailableExprs
Доступные выражения-множества e_genB, e-KILLb. Передаточная функция ББл В.
Файл AvailableExprs.cs
var ae = new AvaliableExprs(); // новые класс доступных выражений
var rr = ae.GetGenAndKillerSets(blocks); // генерирует два элемента, первый это e_gen, второй e_kill. Для доступа rr.Item1 (Item2) общее для всех блоков
var ttt = ae.TransferByGenAndKiller(rr.Item1[i], rr.Item1[i], rr.Item2[i]); // вызывается для каждого блока, i-номер блока. Возвращает HashSet<(string, string, string)>