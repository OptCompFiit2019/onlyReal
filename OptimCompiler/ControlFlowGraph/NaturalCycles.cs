using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang.ControlFlowGraph
{
    class NaturalCycles
    {
        public static string ListToString(List<int> list)
        {
            string s = "";
            for (int i = 0; i < list.Count; i++)
                s = s + list[i] + " ";
            return s;
        }
        private static List<int> GetWay(List<List<int>> list, int begin, int end)
        {
            var ways = new List<List<int>>();
            var way = new List<int>();
            var copyWay = new List<int>();
            for (int k = 0; k < list[begin].Count; k++)
            {
                way = new List<int>();
                if (begin < list[begin][k])
                {
                    way.Add(begin);
                    way.Add(list[begin][k]);
                    ways.Add(way);
                }
            }
            int i = 0;
            while (i < ways.Count)
            {
                way = ways[i];
                int last = way[way.Count - 1];
                bool f = false;
                for (int j = 0; j < list[last].Count; j++)
                {
                    copyWay = new List<int>();
                    if ((last < list[last][j]) && (last != end))
                    {
                        copyWay.AddRange(way);
                        copyWay.Add(list[last][j]);
                        ways.Add(copyWay);
                        f = true;
                    }
                }
                if (f)
                    ways.RemoveAt(i);
                else
                    i++;
            }
            var vertices = new List<int>();
            for (i = 0; i < ways.Count; i++)
            {
                way = new List<int>();
                if (ways[i].Contains(end))
                {
                    way.AddRange(ways[i]);
                    int j = 0;
                    while ((j < way.Count) && way.Contains(end))
                    {
                        if (!vertices.Contains(way[j]))
                        {
                            vertices.Add(way[j]);
                            way.RemoveAt(j);
                        }
                        else
                            j++;
                    }
                }
            }
            vertices.Sort();
            return vertices;
        }
        private static List<List<int>> GetListCycles(List<List<int>> list)
        {
            var way = new List<int>();
            var ways = new List<List<int>>();
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < list[i].Count; j++)
                {
                    if (list[i][j] < i)
                    {
                        way = NaturalCycles.GetWay(list, list[i][j], i);
                        ways.Add(way);
                    }
                }
            }

            for (int i = 0; i < ways.Count; i++)
            {
                int j = 0;
                while (j < ways.Count)
                {
                    if ((ways[i][0] == ways[j][0]) && (i != j))
                        ways.RemoveAt(j);
                    else
                        j++;
                }
            }

            for (int i = 0; i < ways.Count; i++)
            {
                for (int j = 0; j < ways.Count; j++)
                {
                    var gg = ways[i].Intersect(ways[j]).ToList();
                    if ((gg.Count == 1) && (ways[i][ways[i].Count - 1] == ways[j][0]))
                    {
                        for (int l = 1; l < ways[j].Count; l++)
                        {
                            ways[i].Add(ways[j][l]);
                        }
                    }
                    else
                    {
                        if ((gg.Count != ways[j].Count) && (ways[i][0] < ways[j][0]))
                        {
                            for (int l = 0; l < ways[j].Count; l++)
                            {
                                if (!ways[i].Contains(ways[j][l]))
                                    ways[i].Add(ways[j][l]);
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < ways.Count; i++)
                ways[i].Sort();
            return ways;
        }

        public static List<List<int>> SearchNaturalCycles(ControlFlowGraph cfg)
        {
            var list = cfg.GetAsAdjacencyList();
            var cycles = NaturalCycles.GetListCycles(list);
            for (int i = 0; i < cycles.Count; i++)
            {
                for (int j = 0; j < cycles.Count; j++)
                {
                    var a = cycles[j].All(cycles[i].Contains) && cycles[j].Count < cycles[i].Count;
                    if (a)
                        Console.WriteLine(NaturalCycles.ListToString(cycles[j]) + "contains in " + NaturalCycles.ListToString(cycles[i]));
                }
            }
            return cycles;
        }
    }
}
