GraphDepth
Вычисляет глубину CFG.
Чтобы провести эту оптимизацию нужно построить CFG и выполнить следующий код:
using SimpleLang.ControlFlowGraph;

var blocks = new Block(treeCode).GenerateBlocks();
CFG controlFlowGraph = new CFG(blocks);
int depth = GraphDepth.GetGraphDepth(controlFlowGraph);