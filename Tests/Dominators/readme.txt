DominatorsFinder

ѕоиск доминаторов дл€ каждой вершины графа потока управлени€

ƒл€ запуска оптимизаций необходимо добавить экземл€р класса DefUseDeadCodeOp/DefUseConstOpt в экземпл€р класса AutoThreeCodeOptimiser и вызывать метод Apply:

var finder = new SimpleLang.Dominators.DominatorsFinder(controlFlowGraph);
var dominators = finder.Find();

√де controlFlowGraph - экземпл€р класса ControlFlowGraph.