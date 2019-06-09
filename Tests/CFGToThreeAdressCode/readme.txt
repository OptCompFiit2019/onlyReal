ControlFlowGraph.ToThreeAddressCode() Method
Переводит ControlFlowGraph в трёхадресный код

Пример запуска:

// Построение базовых блоков по трёхадресному коду исходной программы
var blocks = new Block(treeCode).GenerateBlocks();
// Построение графа потока управления
CFG controlFlowGraph = new CFG(blocks);

foreach (var c in controlFlowGraph.ToThreeAddressCode())
    Console.WriteLine(c);
	
Результат вызова метода должен быть эквивалентен исходному коду treeCode.
