DetectReversibleEdges

Определение того, является ли ребро CFG обратным

Для запуска оптимизаций необходимо добавить экземляр класса DetectReversibleEdgesи вызывать метод PrintIsReverseDic:

var isR = new DetectReversibleEdges(ControlFlowGraph);
isR.PrintIsReverseDic();

Где controlFlowGraph - экземпляр класса ControlFlowGraph.
