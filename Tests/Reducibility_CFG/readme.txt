Reducibility_CFG

Определение того, является ли CFG Приводимым

Для запуска оптимизаций необходимо добавить экземляр класса DetectReversibleEdgesи вызывать метод PrintisReducible:

var isR = new DetectReversibleEdges(ControlFlowGraph);
isR.PrintisReducible();

Где controlFlowGraph - экземпляр класса ControlFlowGraph.
