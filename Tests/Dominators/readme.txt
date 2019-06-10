DominatorsFinder

Поиск доминаторов для каждой вершины графа потока управления

Для запуска выполнить следующий код:

var finder = new SimpleLang.Dominators.DominatorsFinder(controlFlowGraph);
var dominators = finder.Find();

Где controlFlowGraph - экземпляр класса ControlFlowGraph.