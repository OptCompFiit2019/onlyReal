ReachingDefsAnalysis
Выполняет анализ достигающих определений. На выходе алгоритма — INы и OUTы базовых блоков программы.

Чтобы провести этот анализ, нужно выполнить следующий код:

// Построение базовых блоков по трёхадресному коду исходной программы
var blocks = new Block(treeCode).GenerateBlocks();
// Запуск итеративного алгоритма для анализа достигающих определений
var reachingDefsAnalysis = new ReachingDefsAnalysis();
reachingDefsAnalysis.IterativeAlgorithm(blocks);

Чтобы получить результат:
var ins = reachingDefsAnalysis.Ins;
var outs = reachingDefsAnalysis.Outs;

Чтобы наглядно распечатать результат, как в файле expectation:
reachingDefsAnalysis.PrintOutput();