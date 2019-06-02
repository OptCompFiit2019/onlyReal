CFG edge classification+DeepSpanTree
Классификация рёбер CFG+построение глубинного остовного дерева с соответствующей нумерацией вершин
Файл DeepSpanTree.cs
var controlFlowGraph=new CFG(treeCode);
var tt = new SpanTree(controlFlowGraph);
var edges=tt.buildSpanTree();