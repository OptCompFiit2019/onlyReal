using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleLang.Visitors;
using SimpleLang.Dominators;
using static SpanTree;

namespace SimpleLang
{
    class DetectReversibleEdges
    {
        Dictionary<int, List<int>> dominators;
        List<Edge> edges;
        // словарь, где каждому ребру соответствует значение bool, если ребро обратимо-true, иначе false
        Dictionary<Edge, bool> isReversible = new Dictionary<Edge, bool>();

        //Конструктор
        public DetectReversibleEdges(ControlFlowGraph.ControlFlowGraph cfg)
        {
            var dom = new DominatorsFinder(cfg);
            dominators = dom.Find();

            var spt = new SpanTree(cfg);
            edges = spt.buildSpanTree();

            foreach (var e in edges)
                isReversible[e] = dominators.Keys.Contains(e.v1.num)
                    && dominators[e.v1.num].Contains(e.v2.num);
        }

        // возвращает заполненный словарь isReversible
        public Dictionary<Edge, bool> isRevers()
        {
            return isReversible;
        }

        // Выводит словарь isReversible в консоль
        public void PrintIsReverseDic()
        {
            var dic = isRevers();
            foreach (var x in dic)
                Console.WriteLine("Edge {0} -> {1} is {2}", x.Key.v1.num.ToString(),  
                    x.Key.v2.num.ToString(), x.Value? "reverse" : "not reverse");
        }

        //Функция определяет, является ли граф приводимым.
        //Если количество отступающих рёбер равно количеству обратимых, то граф приводим
        public bool isReducible()  
        {
            int countBack = 0;
            int countReversible = 0;
            foreach(var e in edges)
                if (e.type == EdgeType.Back)
                    countBack++;

            foreach (var i in isReversible)
                if (i.Value)
                    countReversible++;

            return countBack == countReversible;
        }

        // Выводит в консоль факт приводимости CFG
        public void PrintisReducible()
        {
            Console.WriteLine("CFG is {0}", isReducible() ? "reducibile" : "not reducibile");
        }
    }
}
