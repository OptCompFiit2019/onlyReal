using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleLang.ControlFlowGraph;
using SimpleLang.Visitors;

public class SpanTree
{
    List<Edge> edges = new List<Edge>();
    List<Vertex> verts = new List<Vertex>();
    ControlFlowGraph cfg;
    int pre;

    public enum VertStatus
    {
        Init,
        InProcess,
        Done
    }

    public enum EdgeType
    {
        Tree,
        Back,
        Cross,
        Forward
    }
    public SpanTree(ControlFlowGraph CFG)
    {
        cfg = CFG;
        pre = 1;
    }
    public class Vertex
    {
        public int num;
        public int pre;
        public VertStatus status;
        public int post;

        public Vertex() { }
    }
    public class Edge
    {
        public Vertex v1;
        public Vertex v2;
        public EdgeType type;

        public Edge() { }
    }

    public void Traverse(Vertex v)
    {
        v.pre = pre++;
        v.status = VertStatus.InProcess;

        foreach (var e in edges)
        {
            if (e.v1.num == v.num)
            {
                var w = e.v2;
                switch (w.status)
                {
                    case VertStatus.Init:
                        e.type = EdgeType.Tree;
                        Traverse(w);
                        break;
                    case VertStatus.InProcess:
                        e.type = EdgeType.Back;
                        break;
                    case VertStatus.Done:
                        if (w.pre < v.pre)
                            e.type = EdgeType.Cross;
                        else e.type = EdgeType.Forward;
                        break;
                }
            }           
        }
        v.status = VertStatus.Done;
    }

    public List<Edge> buildSpanTree()
    {
        int[,] matr = cfg.cfg.GetAdjacencyMatrix();
        int[] vv = cfg.cfg.g;
        int m = cfg.cfg.n;

        //формирование списка вершин
        for (var i = 0;i< m;i++)
        {
            var vert = new SpanTree.Vertex();
            vert.num = i+1;
            vert.status = VertStatus.Init;

            verts.Add(vert);
        }

        //формирование списка рёбер
        foreach (var i in verts)
        {
            foreach (var j in verts)
            {
                if (matr[i.num-1, j.num-1] == 1)
                {
                    var ee = new Edge();
                    ee.v1 = i;
                    ee.v2 = j;
                    edges.Add(ee);
                }
            }
        }
        Traverse(verts[0]);
        return edges;
    }

    public void writeAllEdgesWithTypes()
    {
        foreach (var e in edges)
        {
            Console.WriteLine("v1={0}, v2={1}, type={2}, new num v1={3}, new num v2={4}", e.v1.num, e.v2.num, e.type, e.v1.pre, e.v2.pre);
        }
    }

    public void writeAllSpanTreeEdges()
    {
        foreach (var e in edges)
        {
            if (e.type == SpanTree.EdgeType.Tree)
            {
                Console.WriteLine("v1={0}, v2={1}, type={2}, new num v1={3}, new num v2={4}", e.v1.num, e.v2.num, e.type, e.v1.pre, e.v2.pre);
            }     
        }
    }
}
