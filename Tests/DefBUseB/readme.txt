Вычисление множеств DEFb и USEb для активных переменных

Вычисляется локально в каждом базовом блоке B в графе потока управления.

DEFb - мн-во переменных, определенных в B
USEb - мн-во переменных, значения которых могут использоваться в B до любого их определения. 

Исходный код:
{
  int i, j, k, l;
  i = k + 1;
  j = l + 1;
  k = i;
  l = j;
}

Множества:
DEFb: i j k l
USEb: k l

Запуск:
var blocks = new Block(treeCode).GenerateBlocks(); //генерируем базовые блоки
CFG controlFlowGraph = new CFG(blocks); //строим CFG (using CFG = SimpleLang.ControlFlowGraph.ControlFlowGraph;)
var DefUse = new DefUseBlocks(controlFlowGraph); //в этом классе два поля - DefBs и UseBs = new List<HashSet<string>>();