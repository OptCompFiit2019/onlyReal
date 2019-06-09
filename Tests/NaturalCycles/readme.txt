NaturalCycles
Определение всех естественных циклов в CFG с информацией об их вложенности

Просмотреть все веришны графа, найти естественные циклы и записать в List. На консоль выводится информация о том, какие графы в какие вложены.

Запуск:
var blocks = new Block(treeCode).GenerateBlocks();
CFG controlFlowGraph = new CFG(blocks);
int cycles = NaturalCycles.SearchNaturalCycles(controlFlowGraph);