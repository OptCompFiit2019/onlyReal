LVNOptimization
Применяет оптимизацию LVN для каждого из блоков в CFG.
Чтобы провести эту оптимизацию нужно построить CFG. Данный метод возвращает новый граф. Это необходимо если нужно будет сравнивать варианты до и после оптимизации. Код использования:
CFG controlFlowGraph = new CFG(blocks); // построение графа
controlFlowGraph = LVNOptimization.LVNOptimize(controlFlowGraph); // оптимизация
