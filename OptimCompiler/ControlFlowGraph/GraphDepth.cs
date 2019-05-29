using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang.ControlFlowGraph
{
    class GraphDepth
    {
        /// <summary>
        /// Возвращает глубину CFG для графа <paramref name="graph"/> с правильной нумерацией
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        public static int GetGraphDepth(ControlFlowGraph graph)
        {
            int maxDepth = 0;
            var backwardArcs = new Dictionary<int, int>();

            for(int nodeId = 0; nodeId < graph.blocks.Count; nodeId++)
            {
                var successors = graph.cfg.GetOutputNodes(nodeId);
                foreach (var s in successors)
                    if(s <= nodeId)                    
                        backwardArcs.Add(nodeId, s);                
            }

            foreach(var arc in backwardArcs)
            {
                int currentDepth = 0;
                var currentNode = arc.Key;
                while(backwardArcs.ContainsKey(currentNode))
                {
                    currentNode = backwardArcs[currentNode];
                    currentDepth++;
                }
                maxDepth = currentDepth > maxDepth ? currentDepth : maxDepth; 
            }
            return maxDepth;
        }
    }

    class GraphToDOTHelper
    {
        /// <summary>
        /// Вспомогательный метод сохранения графа <paramref name="graph"/> 
        /// в DOT-файл для дальнейшей визуальзации в GraphViz
        /// </summary>
        /// <param name="path"></param>
        /// <param name="graph"></param>
        public static void SaveAsDOT(string path, ControlFlowGraph graph)
        {
            var sb = new StringBuilder();
            sb.Append("digraph G {\n");
            for (int i = 0; i < graph.blocks.Count; i++)
            {
                sb.Append("b" + i + " [label=\"");
                foreach (var line in graph.blocks[i])
                {
                    if (line.label == "")
                        sb.Append("\t\t\t");
                    sb.Append(line.ToString().TrimEnd() + "\\l");
                }
                
                sb.Append("\" shape = \"rectangle\" ] \n");
            }

            for (int i = 0; i < graph.blocks.Count; i++)
                foreach (var s in graph.cfg.GetOutputNodes(i))
                {
                    sb.Append("b" + i + " -> " + "b" + s);
                    if (s <= i)
                        sb.Append("[style = \"dotted\"]");
                    sb.Append("\n");
                }

            sb.Append("\n}");

            using (System.IO.StreamWriter sw = System.IO.File.CreateText(path))
                sw.Write(sb.ToString());
        }
    }
}
