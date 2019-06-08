# Название задачи
Построение Графа Потока Управления (Control Flow Graph, CFG)

# Название команды
Nvidia

# Постановка задачи
Требуется по списку базовых блоков построить CFG.

# Зависимости задач в графе задач
Задача зависит от:
- генерации базовых блоков.

# Теория
Граф потока управления - множество всех возможных путей исполнения программы, представленное в виде графa. На основе CFG проводится анализ потоков данных и производится множество эфективных оптимизаций.

# Особенности реализации
```
public class ControlFlowGraph
    {
        public List<LinkedList<ThreeCode>> blocks;
        public Graph cfg;
        
        public ControlFlowGraph(List<LinkedList<ThreeCode>> b)
        {
            this.blocks = b;
            cfg = new Graph(this.blocks.Count);
            GenerateCFG();
        }

        public ControlFlowGraph(ThreeAddressCodeVisitor code)
        {
            var code_blocks = new Block.Block(code);
            this.blocks = code_blocks.GenerateBlocks();
            cfg = new Graph(this.blocks.Count);
            GenerateCFG();
        }

        private ControlFlowGraph GenerateCFG()
        {

            var Labels = new Dictionary<string, int>();

            for (int i = 0; i < blocks.Count; i++)
                if (blocks[i].First.Value.label.Length > 0)
                    Labels[blocks[i].First.Value.label] = i;


            for (int i = 0; i < blocks.Count-1; i++)
                switch (blocks[i].Last.Value.operation)
                {
                    case ThreeOperator.Goto:
                        cfg.AddArc(i, Labels[blocks[i].Last.Value.arg1.ToString()]);
                        break;
                    case ThreeOperator.IfGoto:
                        cfg.AddArc(i, Labels[blocks[i].Last.Value.arg2.ToString()]);
                        cfg.AddArc(i, i + 1);
                        break;
                    default:
                        cfg.AddArc(i, i + 1);
                        break;
                }
```
Был определен класс `ControlFlowGraph`, инициализирующийся списком базовых блоков, по которому, посредством метода `GenerateCFG`, строится граф потока управления. Для удобства использования были определены методы `GetAsAdjacencyMatrix` и `GetAsAdjacencyList`, возвращающие соответственно матрицу смежности и список смежности.
