DeadOrAliveOnGraph
Применяет оптимизацию удаления мертвых переменных для всего CFG.
Чтобы провести эту оптимизацию нужно построить CFG. В графе должны присутствовать фиктивные узлы входа и выхода программы. Данный метод возвращает новый граф. Это необходимо если нужно будет сравнивать варианты до и после оптимизации. Код использования:

CFG controlFlowGraph = new CFG(blocks); // построение графа
var DefUse = new DefUseBlocks(controlFlowGraph); // нахождение Def,Use
InOutActiveVariables inOutActive = new InOutActiveVariables(DefUse, controlFlowGraph); // ИтА для активных переменных

controlFlowGraph = ControlFlowOptimisations.DeadOrAliveOnGraph(inOutActive.OutBlocks, controlFlowGraph); // оптимизация