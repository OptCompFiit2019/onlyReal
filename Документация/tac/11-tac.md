# Построение Графа Потока Управления !heading

### Команда Nvidia

#### Постановка задачи
Требуется по списку базовых блоков построить CFG.

#### Зависимости задач в графе задач
Задача зависит от:
- Выделение Базовых Блоков

#### Теория
Граф потока управления - множество всех возможных путей исполнения программы, представленное в виде графa. На основе CFG проводится анализ потоков данных и производится множество эфективных оптимизаций.

#### Особенности реализации
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

Для удобства дальнейшего использования был введён класс 'Graph'.
```
public class Graph
{
public int[] g;
public int n;

public Graph(int _n)
{
    this.n = _n;
    this.g = new int[this.n*this.n];

    for (var i = 0; i < this.n * this.n; ++i)
    this.g[i] = 0;
}

public void AddArc(int i, int j)
{
    this.g[i * this.n + j] = 1;
}

public void ResetArc(int i, int j)
{
    this.g[i * this.n + j] = 0;
}

public int[,] GetAdjacencyMatrix()
{
    var result = new int[this.n, this.n];

    for (var i = 0; i < this.n; ++i)
        for (var j = 0; j < this.n; ++j)
            result[i, j] = this.g[i * this.n + j];

return result;
}

public List<List<int>> GetAdjacencyList()
{
    var result = new List<List<int>>();

    for (var i = 0; i < this.n; ++i)
    {
        result.Add(new List<int>());
        for (var j = 0; j < this.n; ++j)
            if (this.g[i*this.n + j] == 1)
                result.Last().Add(j);
    }

    return result;
}

public override string ToString()
{
var result = "";
    for (var i = 0; i < this.n; ++i)
    {
        result += i.ToString() + ": ";
        for (var j = 0; j < this.n; ++j)
            if (this.g[i * this.n + j] == 1)
            result += j.ToString() + " ";
        result += '\n'.ToString();
    }

return result;
}

public List<int> GetInputNodes(int node_id)
{
    var res = new List<int>();
    for (var i = 0; i < this.n; ++i)
        if (this.g[i * this.n + node_id] == 1)
            res.Add(i);
    return res;
}

public List<int> GetOutputNodes(int node_id)
{
    var res = new List<int>();
    for (var j = 0; j < this.n; ++j)
        if (this.g[node_id * this.n + j] == 1)
            res.Add(j);
        return res;
    }
}
```

[Вверх](#содержание)
