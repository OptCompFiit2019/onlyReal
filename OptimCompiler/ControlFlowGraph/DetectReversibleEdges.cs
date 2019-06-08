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
        }

        // возвращает заполненный словарь isReversible
        public Dictionary<Edge, bool> isRevers()
        {
            foreach (var e in edges)
                isReversible[e] = (e.type == EdgeType.Back)
                    && dominators.Keys.Contains(e.v1.num)
                    && dominators[e.v1.num].Contains(e.v2.num);

            return isReversible;
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
                if (i.Value == true)
                    countReversible++;

            return countBack == countReversible;
        }
    }
}
