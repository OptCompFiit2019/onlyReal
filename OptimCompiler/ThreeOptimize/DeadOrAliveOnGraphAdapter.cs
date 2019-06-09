using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleLang.ThreeCodeOptimisations;
using SimpleLang.Visitors;
using CFG = SimpleLang.ControlFlowGraph.ControlFlowGraph;


namespace SimpleLang.ThreeOptimize
{
    public class DeadOrAliveOnGraphAdapter: ThreeCodeOptimiser
    {
        private bool IsApplyed = false;

        public void Apply(ref LinkedList<ThreeCode> program)
        {
            throw new NotImplementedException();
        }

        public void Apply(ref List<LinkedList<ThreeCode>> res)
        {
            // Добавление фиктивных узлов
            var entry = new LinkedList<ThreeCode>();
            entry.AddFirst(new ThreeCode("entry", "", ThreeOperator.None, null, null));
            var exit = new LinkedList<ThreeCode>();
            exit.AddFirst(new ThreeCode("exit", "", ThreeOperator.None, null, null));
            res.Insert(0, entry);
            res.Add(exit);

            // построение CFG по блокам
            CFG controlFlowGraph = new CFG(res);
            // вычисление множеств Def и Use для всего графа потоков данных
            var DefUse = new DefUseBlocks(controlFlowGraph);
            // вычисление множеств IN, OUT на основе DefUse
            var Out = new InOutActiveVariables(DefUse, controlFlowGraph).OutBlocks;
            // Выполнение оптимизации
            controlFlowGraph = ControlFlowOptimisations.DeadOrAliveOnGraph(Out, controlFlowGraph);

            // Удаление фиктивных узлов
            res = controlFlowGraph.blocks;
            res.RemoveAt(0);
            res.RemoveAt(res.Count - 1);
        }

        public bool Applyed() => IsApplyed;

        /// <summary>
        /// Применяет к графу <paramref name="graph"/> удаление мертвых переменных на основе
        /// информации о множествах IN и OUT для блоков из <paramref name="InOut"/>.
        /// Возвращает новый граф с выполненной на нем оптимизацией без изменения исходного графа.
        /// </summary>
        /// <param name="InOut"></param>
        /// <param name="graph"></param>
        /// <returns></returns>
        public CFG DeadOrAliveOnGraph(List<HashSet<string>> OutBlocks, CFG graph)
            {
                IsApplyed = false;
             
                var resGraph = new ControlFlowGraph.ControlFlowGraph(
                    new List<LinkedList<Visitors.ThreeCode>>(graph.blocks));

                if (OutBlocks.Count != resGraph.blocks.Count)
                    throw new ArgumentException("The number of elements in the sets OUT, graph.blocks must be equal");

                for (int i = resGraph.blocks.Count - 1; i >= 0; i--)
                {
                    var variables = new Dictionary<string, bool>();
                    foreach (var ъуъ in OutBlocks[i])
                        variables[ъуъ] = true;

                    var Optimizer = new DeadOrAliveOptimizationAdapter();

                    resGraph.blocks[i] = Optimizer.DeleteDeadVariables(resGraph.blocks[i], variables);
                    IsApplyed = IsApplyed || Optimizer.Applyed();
                }
                return resGraph;
            }

        public bool NeedFullCode() => true;
    }
}
