using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang.Visitors
{
    class DAGBlocks
    {
        private LinkedList<ThreeCode> program;
        public LinkedList<ThreeCode> Program
        {
            set { program = value; }
            get { return program; }
        }
        public ThreeAddressCodeVisitor treecode;

        public DAGBlocks(ThreeAddressCodeVisitor code)
        {
            treecode = code;
            Program = code.GetCode();
        }

        public void Optimize()
        {
            var blocks = new SimpleLang.Block.Block(treecode).GenerateBlocks();
            var result = new List<ThreeCode>();

            for (int i = 0; i < blocks.Count; i++)
            {
                if (blocks[i].Count > 1)
                {
                    var graph = new DAG(blocks[i].ToList());
                    var prog = graph.Optimize();
                    foreach (var cmd in prog)
                        result.Add(cmd);
                }
                else
                    result.Add(blocks[i].First.Value);
            }
            Program = new LinkedList<ThreeCode>(result);
        }
    }


    class GraphNode
    {
        public ThreeCode code;
        public GraphNode left;
        public GraphNode right;

        public GraphNode(ThreeCode code)
        {
            this.code = code;
            left = null;
            right = null;
        }

        public GraphNode(ThreeCode code, GraphNode l, GraphNode r)
        {
            this.code = code;
            left = l;
            right = r;
        }

        public override bool Equals(object obj)
        {
            if (obj is GraphNode n)
                return this.code == n.code;
            else
                return false;
        }

        public override string ToString()
        {
            string l = (left != null) ? left.code.result : "null";
            string r = (right != null) ? right.code.result : "null";
            return code.ToString() + string.Format("  left: {0};   right: {1}", l, r);
        }
    }

    class DAG
    {
        public GraphNode root;
        public List<ThreeCode> program;
        public Dictionary<GraphNode, List<string>> vars;
        public Dictionary<ThreeCode, GraphNode> GraphNodes;
        public Dictionary<string, List<string>> defs;

        public DAG(List<ThreeCode> prog)
        {
            program = MakeProgram(prog);
            vars = new Dictionary<GraphNode, List<string>>();
            GraphNodes = new Dictionary<ThreeCode, GraphNode>();
            defs = new Dictionary<string, List<string>>();
            foreach (var cmd in program)
            {
                if (cmd.operation == ThreeOperator.Assign || cmd.operation == ThreeOperator.Goto
                    || (cmd.label != "" && cmd.arg1 == null))
                    continue;
                //Переименовываем переменные, которые были переприсвоены
                if (defs.ContainsKey(cmd.arg1.ToString()))
                {
                    var temp = defs[cmd.arg1.ToString()];
                    cmd.arg1 = new ThreeAddressStringValue(temp[temp.Count - 1]);
                }
                else
                    defs[cmd.arg1.ToString()] = new List<string>() { cmd.arg1.ToString() };

                if (defs.ContainsKey(cmd.arg2.ToString()))
                {
                    var temp = defs[cmd.arg2.ToString()];
                    cmd.arg2 = new ThreeAddressStringValue(temp[temp.Count - 1]);
                }
                else
                    defs[cmd.arg2.ToString()] = new List<string>() { cmd.arg2.ToString() };

                if (!defs.ContainsKey(cmd.result))
                    defs[cmd.result] = new List<string>() { cmd.result };
                else
                {
                    defs[cmd.result].Add(cmd.result + "#" + defs[cmd.result].Count.ToString());
                    cmd.result = defs[cmd.result][defs[cmd.result].Count - 1];
                }

                //Если есть трёхадресные коды, у которых такая же правая часть, то запоминаем
                //имена результатов таких кодов
                var sameCodes = GraphNodes.Keys.Where(x => RightPartsEquals(cmd, x)).ToList();
                if (sameCodes.Count > 0)
                {
                    foreach (var code in sameCodes)
                        vars[GraphNodes[code]].Add(cmd.result);
                    continue;
                }
                //Если код встретили впервые, его нет в графе
                var currentGraphNode = new GraphNode(cmd);
                //Ищем коды, результат которых является аргументом кода cmd
                foreach (var res in GraphNodes.Keys)
                {
                    if (res.result.Equals(currentGraphNode.code.arg1.ToString()))
                    {
                        currentGraphNode.left = GraphNodes[res];
                        continue;
                    }
                    if (currentGraphNode.code.arg2 != null && res.result.Equals(currentGraphNode.code.arg2.ToString()))
                    {
                        currentGraphNode.right = GraphNodes[res];
                        continue;
                    }
                }
                GraphNodes[cmd] = currentGraphNode;
                vars[currentGraphNode] = new List<string>() { cmd.result };
                //Корень - тот узел, который был добавлен в граф, а не в vars,
                //поэтому присваиваем на каждой итерации, в конце останется узел, соответсвующий последнему 3-х адресному коду из программы
                root = currentGraphNode;
            }
        }

        public List<ThreeCode> MakeProgram(List<ThreeCode> prog)
        {
            var program = new List<ThreeCode>();
            int i = 0;
            for (i = 0; i < prog.Count - 1; i++)
            {
                if (prog[i + 1].arg1 != null && prog[i].result.Equals(prog[i + 1].arg1.ToString()) && prog[i].result.Contains("temp_") && prog[i + 1].arg2 == null)
                {
                    if (prog[i].label != "")
                        program.Add(new ThreeCode(prog[i].label, prog[i + 1].result, prog[i].operation, prog[i].arg1, prog[i].arg2));
                    else
                        program.Add(new ThreeCode(prog[i + 1].result, prog[i].operation, prog[i].arg1, prog[i].arg2));
                    i++;
                }
                else
                    program.Add(prog[i]);
            }
            if (i == prog.Count - 1)
                program.Add(prog[i]);
            return program;
        }

        public void ShowGraph(GraphNode root)
        {
            if (root == null)
                return;
            Console.Write(root.code.operation.ToString() + "  " + root.code.arg1 + "   " + root.code.arg2);
            Console.Write("  Variables: ");
            foreach (var v in vars[root])
                Console.Write(v.ToString() + ", ");
            Console.WriteLine();
            if (root.left != null)
                ShowGraph(root.left);
            if (root.right != null)
                ShowGraph(root.right);
        }

        public bool RightPartsEquals(ThreeCode x, ThreeCode y)
        {
            return (x.arg1.ToString() == y.arg1.ToString() && x.arg2.ToString() == y.arg2.ToString());
        }

        public void RemoveVarNumbers(List<ThreeCode> program)
        {
            for (int i = 0; i < program.Count; i++)
            {
                if (program[i].label != "" && program[i].arg1 == null)
                    continue;
                var ind = program[i].result.IndexOf('#');
                if (ind != -1)
                    program[i].result = program[i].result.Remove(ind);

                var arg = program[i].arg1.ToString();
                ind = arg.IndexOf('#');
                if (ind != -1)
                    program[i].arg1 = new ThreeAddressStringValue(arg.Remove(ind));

                if (program[i].arg2 != null)
                {
                    arg = program[i].arg2.ToString();
                    ind = arg.IndexOf('#');
                    if (ind != -1)
                        program[i].arg2 = new ThreeAddressStringValue(arg.Remove(ind));
                }
            }
        }

        public LinkedList<ThreeCode> Optimize()
        {
            var result = new List<ThreeCode>();
            foreach (var cmd in this.program)
            {
                if (cmd.operation == ThreeOperator.Assign || cmd.operation == ThreeOperator.Goto || (cmd.label != "" && cmd.arg1 == null))
                {
                    result.Add(cmd);
                    continue;
                }

                var sameCommand = GraphNodes.Where(x => RightPartsEquals(x.Key, cmd)).First();
                if (vars[sameCommand.Value][0].Equals(cmd.result))
                    result.Add(cmd);
                else
                    result.Add(new ThreeCode(cmd.result, ThreeOperator.Assign, new ThreeAddressStringValue(vars[sameCommand.Value][0])));
            }
            RemoveVarNumbers(result);
            return new LinkedList<ThreeCode>(result);
        }
    }
}
