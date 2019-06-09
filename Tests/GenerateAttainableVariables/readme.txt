Генерация достигающих определений.

Позволяет построить глобальный Use-Def граф.

Применение:
var cfg = new CFG(treeCode);
var av = new AttainableVariables(cfg);
var att_vars = av.GenerateAttainableVariables();
Console.WriteLine(att_vars);

