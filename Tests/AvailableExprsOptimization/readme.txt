AvailableExprsOptimizer
Оптимизация для общих выражений на основе информации, полоученной в результате применения итеративного алгоритма.

Запуск данной оптимизации:

// Построение базовых блоков по трёхадресному коду исходной программы
var blocks = new Block(treeCode).GenerateBlocks();
// Применение оптимизации (итеративный алгоритм запускается внутри метода ApplyOptimization)
var availableExprsOptimizer = new AvailableExprsOptimizer();
CFG cfg1 = availableExprsOptimizer.ApplyOptimization(blocks);
