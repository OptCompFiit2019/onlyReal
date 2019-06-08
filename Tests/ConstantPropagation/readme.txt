ConstantPropagationOptimizer
Распространение констант на основе информации, полоученной в результате применения итеративного алгоритма.

Запуск данной оптимизации:

// Построение базовых блоков по трёхадресному коду исходной программы
var blocks = new Block(treeCode).GenerateBlocks();
// Применение оптимизации (итеративный алгоритм запускается внутри метода ApplyOptimization)
var constPropOptimizer = new ConstantPropagationOptimizer();
CFG cfg1 = constPropOptimizer.ApplyOptimization(blocks);
