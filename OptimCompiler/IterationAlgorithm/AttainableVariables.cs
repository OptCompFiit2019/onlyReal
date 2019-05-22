using System;
using System.CodeDom.Compiler;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Security.AccessControl;
using ProgramTree;
using SimpleLang.Visitors;
using SimpleLang.Block;
using SimpleLang.ControlFlowGraph;
using SimpleLang.Visitors;


namespace SimpleCompiler.IterationAlgorithm
{
    public class VariableUsingHistory
    {
        public List<int> blocks_inds_hist;
        public string variable_name;
        public List<string> valiable_name_hist;
        
        public VariableUsingHistory(string name)
        {
            blocks_inds_hist = new List<int>();
            variable_name = name;
            valiable_name_hist = new List<string>();
        }

        public bool AddUsingStep(Graph g, int inner_block_ind, string name)
        {
            if (blocks_inds_hist.Count == 0)
            {
                blocks_inds_hist.Add(inner_block_ind);
                valiable_name_hist.Add(name);
                return true;
            }

            if (
                g.NodesIsLinked(blocks_inds_hist.Last(), inner_block_ind) == 1 
                || 
                blocks_inds_hist.Last() == inner_block_ind
            )
            {
                blocks_inds_hist.Add(inner_block_ind);
                valiable_name_hist.Add(name);
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            var result = variable_name + ":";
            for (var i = 0; i < blocks_inds_hist.Count; ++i)
            {
                result += " (" + blocks_inds_hist[i] + " as " + valiable_name_hist[i] + ")";
            }
            return result;
        }
    }

    public class ByteVector
    {
        public List<int> v;

        public int this[int i]
        {
            get { return v[i]; }
        }
        
        public ByteVector(int size)
        {
            v = new List<int>();
            for (var i = 0; i < size; ++i)
                v.Add(0);
        }
        
        public ByteVector(List<int> inpv)
        {
            v = new List<int>();
            for (var i = 0; i < inpv.Count; ++i)
                v.Add(inpv[i]);
        }

        public ByteVector(ByteVector b)
        {
            v = new List<int>();
            for (var i = 0; i < b.Size(); ++i)
                v.Add(b.v[i]);
        }

        public int Size()
        {
            return v.Count;
        }
        
        public static ByteVector operator + (ByteVector b1, ByteVector b2)
        {
            if (b1.Size() != b2.Size())
                throw new System.Exception("Byte vectors has non equal size");
            
            var res = new List<int>();
            for (var i = 0; i < b1.Size(); ++i)
                res.Add(b1.v[i] + b2.v[i] > 0 ? 1 : 0);
            
            return new ByteVector(res);
        }
        
        public static ByteVector operator - (ByteVector b1, ByteVector b2)
        {
            if (b1.Size() != b2.Size())
                throw new System.Exception("Byte vectors has non equal size");
            
            var res = new List<int>();
            for (var i = 0; i < b1.Size(); ++i)
                res.Add(b1.v[i] - b2.v[i] > 0 ? 1 : 0);
            
            return new ByteVector(res);
        }
        
        public static ByteVector operator * (ByteVector b1, ByteVector b2)
        {
            if (b1.Size() != b2.Size())
                throw new System.Exception("Byte vectors has non equal size");
            
            var res = new List<int>();
            for (var i = 0; i < b1.Size(); ++i)
                res.Add(b1.v[i] * b2.v[i]);
            
            return new ByteVector(res);
        }

        public static bool operator == (ByteVector b1, ByteVector b2)
        {
            if (b1.Size() != b2.Size())
                throw new System.Exception("Byte vectors has non equal size");
            
            for (var i = 0; i < b1.Size(); ++i)
                if (b1.v[i] != b2.v[i])
                    return false;
            return true;
        }
        
        public static bool operator != (ByteVector b1, ByteVector b2)
        {
            return !(b1 == b2);
            
        }

        public override string ToString()
        {
            string res = "";
            for (var i = 0; i < this.Size(); ++i)
                res += v[i].ToString();
            return res;
        }

        public static explicit operator int (ByteVector b)
        {
            int res = 0;
            int pow = 2;
            for (var i = 0; i < b.Size(); ++i)
            {
                if (b.v[i] > 0)
                    res += pow;
                pow *= 2;
            }

            return res;
        }
    }

    public class BlockVariables
    {
        public List<string> variables_names;
        public List<int> variable_nums;

        public BlockVariables()
        {
            variables_names = new List<string>();
            variable_nums = new List<int>();
        }

        public void Add(string var_name, int num)
        {
            variables_names.Add(var_name);
            variable_nums.Add(num);
        }

        public List<int> In(string name)
        {
            var res = new List<int>();
            for (var i = 0; i < variables_names.Count; ++i)
                if (String.Equals(variables_names[i], name))
                    res.Add(variable_nums[i]);
            return res;
        }

        public int Count()
        {
            return variables_names.Count;
        }
    }
    
    public class GenKillList
    {
        public List<ByteVector> GEN;
        public List<ByteVector> KILL;

        public GenKillList(int blocks_count, int assigns_count)
        {
            GEN = new List<ByteVector>();
            KILL = new List<ByteVector>();

            for (var i = 0; i < blocks_count; ++i)
            {
                GEN.Add(new ByteVector(assigns_count));
                KILL.Add(new ByteVector(assigns_count));
            }
        }

        public GenKillList Generate(List<BlockVariables> blocks)
        {
            for (var i = 0; i < blocks.Count; ++i)
                for (var k = 0; k < blocks[i].Count(); ++k)
                {
                    GEN[i].v[blocks[i].variable_nums[k]] = 1;

                    for (var j = 0; j < blocks.Count; ++j)
                        if (i != j)
                            foreach (var in_var_num in blocks[j].In(blocks[i].variables_names[k]))
                                KILL[i].v[in_var_num] = 1;
                }
            return this;
        }
    }
    
    public class AttainableGraph
    {
        public List<ByteVector> IN_byte;
        public List<ByteVector> OUT_byte;
        
        public List<List<int>> IN;
        public List<List<int>> OUT;
        
        public AttainableGraph(int blocks_count, int assigns_count)
        {
            IN = new List<List<int>>();
            OUT = new List<List<int>>();
            
            IN_byte = new List<ByteVector>();
            OUT_byte = new List<ByteVector>();

            for (var i = 0; i < blocks_count; ++i)
            {
                IN.Add(new List<int>());
                OUT.Add(new List<int>());
                IN_byte.Add(new ByteVector(assigns_count));
                OUT_byte.Add(new ByteVector(assigns_count));
            }
        }
        
        public override string ToString()
        {
            var result = "";
            for (var i = 0; i < this.IN_byte.Count; ++i)
                result += "BLOCK: " + i.ToString() + ", IN: " + this.IN_byte[i].ToString() +
                          ", OUT: " + this.OUT_byte[i].ToString() + "\n";
            return result;
        }
    }
    
    public class AttainableVariables
    {
        public ControlFlowGraph cfg;
        public List<BlockVariables> blocks_variables;

        public AttainableVariables(ControlFlowGraph _cfg)
        {
            this.cfg = _cfg;
            blocks_variables = new List<BlockVariables>();
            for (var i = 0; i < cfg.cfg.n; ++i)
                blocks_variables.Add(new BlockVariables());
        }

        public bool CheckToUsefulOperation(ThreeOperator op)
        {
            return !(op == ThreeOperator.Goto || op == ThreeOperator.None || op == ThreeOperator.IfGoto);
        }

        public bool IsInnerInOperation(string name, ThreeCode a)
        {
            bool y1 = false, y2 = false;
            
            if (a.arg1 != null & a.arg1.ToString().Length > 0)
                y1 = a.arg1.ToString().IndexOf(name, 0) != -1;
            
            if (a.arg2 != null && a.arg2.ToString().Length > 0)
                y2 = a.arg2.ToString().IndexOf(name, 0) != -1;
            
            return y1 || y2;
        }
        
        public AttainableGraph GenerateAttainableVariables()
        {
            // Finding all initialized variables
            int var_count = 0;
            for (var i = 0; i < cfg.cfg.n; ++i)
            {
                LinkedList<ThreeCode> check_block = cfg.blocks[i];
                // First analise this block
                // Find first assign or variable initialisation
                foreach (var operation in check_block)
                    if (this.CheckToUsefulOperation(operation.operation))
                        if (operation.result.ToString().Length > 0)
                            blocks_variables[i].Add(operation.result.ToString(), var_count++);
            }
            
            GenKillList genkilllst = new GenKillList(cfg.cfg.n, var_count).Generate(blocks_variables);

            var att_graph = new AttainableGraph(cfg.cfg.n, var_count);

            var steps_list = new Queue<int>();
            var used_nodes = new List<bool>();
            for (var i = 0; i < cfg.cfg.n; ++i)
                used_nodes.Add(false);
            used_nodes[0] = true;
            steps_list.Enqueue(0);

            while (steps_list.Count > 0)
            {
                var node_ind = steps_list.Dequeue();

                foreach (var out_node_id in cfg.cfg.GetOutputNodes(node_ind))
                    if (!used_nodes[out_node_id])
                        steps_list.Enqueue(out_node_id);
                
                // att_graph.OUT_byte[node_ind] = new ByteVector(var_count);
                
                foreach (var out_node_id in cfg.cfg.GetOutputNodes(node_ind))
                    if (out_node_id != node_ind)
                        att_graph.OUT_byte[out_node_id] = new ByteVector(var_count);

                bool changes = true;
                while (changes && cfg.cfg.GetOutputNodes(node_ind).Count > 0)
                    foreach (var out_node_id in cfg.cfg.GetOutputNodes(node_ind))
                    {
                        ByteVector previous_out = new ByteVector(att_graph.OUT_byte[out_node_id]);
                        
                        if (out_node_id != node_ind)
                        {
                            foreach (var in_out_node_id in cfg.cfg.GetInputNodes(out_node_id))
                                att_graph.IN_byte[out_node_id] =
                                    att_graph.IN_byte[out_node_id] + att_graph.OUT_byte[in_out_node_id];
                            att_graph.OUT_byte[out_node_id] =
                                genkilllst.GEN[out_node_id] +
                                (att_graph.IN_byte[out_node_id] - genkilllst.KILL[out_node_id]);
                        }

                        changes = changes && previous_out != att_graph.OUT_byte[out_node_id];
                    }
            }
            
            return att_graph;
        }

        public override string ToString()
        {
            var result = "";
            foreach (var varnl in this.blocks_variables)
                foreach (var varn in varnl.variables_names)
                    result += varn + "\n";
            return result;
        }
    }
}