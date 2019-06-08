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
using SimpleLang.TransferFunction_Intel_;
using SimpleLang.ControlFlowGraph;
using SimpleLang.Visitors;


namespace SimpleCompiler.IterationAlgorithm
{
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

            GenKillList genkilllist = new GenKillList(cfg.cfg.n, var_count).Generate(blocks_variables);

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
                            var transferFunction = new TransferFunction();
                            att_graph.OUT_byte[out_node_id] = transferFunction.Apply(genkilllist.GEN[out_node_id], genkilllist.KILL[out_node_id], att_graph.IN_byte[out_node_id]);
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