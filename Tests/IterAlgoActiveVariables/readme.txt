Запуск:
var blocks = new Block(treeCode).GenerateBlocks(); //генерируем базовые блоки
CFG controlFlowGraph = new CFG(blocks); //строим CFG (using CFG = SimpleLang.ControlFlowGraph.ControlFlowGraph;)
var DefUse = new DefUseBlocks(controlFlowGraph); //в этом классе два поля - DefBs и UseBs = new List<HashSet<string>>();
var InOut = new InOutActiveVariables(DefUse, controlFlowGraph);