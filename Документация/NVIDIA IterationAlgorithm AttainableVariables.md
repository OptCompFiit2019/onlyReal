# Название задачи
Итерационный алгоритм для достигающих определений.

# Название команды
Nvidia

# Постановка задачи
Требуется по графу потока управления найти множество достигающих определений.

# Зависимости задач в графе задач
Задача зависит от:
- генерации графа потока управления.

# Теория
Основными этапами данного итерационного алгоритма являются:
1. Инициализация множеств gen_b, kill_b
2. OUT всех базовых блоков, отличных от входного, помечается пустым
3. Основной цикл алгоритма, на каждой итерации которого производится обновление IN и OUT для всего графа. Анализ заканчивается когда множества IN, OUT более не претерпевают изменений.

# Особенности реализации
Ниже представлен код использования данного класса:
```csharp
using SimpleCompiler.IterationAlgorithm;

var cfg = new CFG(treeCode);  // Generate Control Flow Graph

var av = new AttainableVariables(cfg);  // Initialize class instance with CFG
var att_vars = av.GenerateAttainableVariables();  // Run algorithm
Console.WriteLine(att_vars);  // Write byte representation of attainable variables for each block
```