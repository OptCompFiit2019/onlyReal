using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GenericTransferFunction;
using SimpleLang.GenericIterativeAlgorithm;

namespace SimpleLang
{
    class MOPAlgorithm<T>
    {
        // направление обхода: true - прямой, false - обратный
        public bool IsForward { get; }
        public ControlFlowGraph.ControlFlowGraph Graph { get; }
        // информация о блоках. Каждому блоку с индексом i в графе соответствует экземпляр
        // класса BlockInfo<T> в списке BlocksInfo с тем же индексом
        public List<BlockInfo<T>> BlocksInfo { get; }
        // оператор сбора
        public Func<IEnumerable<BlockInfo<T>>, BlockInfo<T>, BlockInfo<T>> MeetOperator;
        // передаточная функция
        public TransferFunction<BlockInfo<T>> Function { get; }
        // значение инициализации IN/OUT первого или последнего блока для прямого или обратного обходов соответственно
        public ISet<T> InitEntryExit { get; }
        // значение инициализации IN/OUT для остальных блоков
        public ISet<T> InitOther { get; }

        private LinkedList<LinkedList<BlockInfo<T>>> ways;

        public MOPAlgorithm(List<BlockInfo<T>> blocksInfo, ControlFlowGraph.ControlFlowGraph graph,
            Func<IEnumerable<BlockInfo<T>>, BlockInfo<T>, BlockInfo<T>> meetOperator, bool isForward,
            IEnumerable<T> initValueEntryExit, IEnumerable<T> initValueOthers,
            TransferFunction<BlockInfo<T>> function)
        {
            BlocksInfo = blocksInfo;
            Graph = graph;
            MeetOperator = meetOperator;
            IsForward = isForward;
            InitEntryExit = new HashSet<T>(initValueEntryExit);
            InitOther = new HashSet<T>(initValueOthers);
            Function = function;
            ways = new LinkedList<LinkedList<BlockInfo<T>>>();

            int countBlocks = Graph.blocks.Count;

            for (int i = 0; i < countBlocks; i++)
            {
                BlocksInfo[i].IN = InitEntryExit;
                BlocksInfo[i].OUT = InitOther;
            }

            if (IsForward)
                BlocksInfo[0].OUT = InitEntryExit;
            else
                BlocksInfo[countBlocks - 1].IN = InitEntryExit;
        }

        /// <summary>
        /// Выполняет оптимизационный алгоритм и возвращает результат как List<BlockInfo<T>>,
        /// </summary>
        /// <returns></returns>
        public List<BlockInfo<T>> Perform()
        {
            int start = IsForward ? BlocksInfo.Count - 1 : 0;
            InternalPerform(new LinkedList<BlockInfo<T>>(), start);

            BlocksInfo[start] = MeetOperator(ways.Distinct().Select(w =>
                ApplyTransferFunction(w)).ToList(), BlocksInfo[start]);
            return BlocksInfo;
        }

        private void InternalPerform(LinkedList<BlockInfo<T>> track, int index)
        {
            var indexes = IsForward ? Graph.cfg.GetInputNodes(index)
                : Graph.cfg.GetOutputNodes(index);

            if (indexes.Count == 0)
                ways.AddLast(track);
            foreach (var ii in indexes)
            {
                track.AddLast(BlocksInfo[ii]);
                InternalPerform(track, ii);
            }
        }

        private BlockInfo<T> ApplyTransferFunction(IEnumerable<BlockInfo<T>> way)
        {
            var w = way.Reverse().ToList();
            for (int i = 1; i < w.Count; ++i)
            {
                if (IsForward)
                    w[i].IN = w[i - 1].OUT;
                else
                    w[i].OUT = w[i - 1].IN;
                w[i] = Function.Apply(w[i]);
            }
            return w[w.Count - 1];
        }

        /// <summary>
        /// Возвращает список множеств OUT для блоков
        /// </summary>
        /// <returns></returns>
        public List<HashSet<T>> GetOUTs()
        {
            var res = new List<HashSet<T>>();
            foreach (var info in BlocksInfo)
                res.Add(new HashSet<T>(info.OUT));
            return res;
        }

        /// <summary>
        /// Возвращает список множеств IN для блоков
        /// </summary>
        /// <returns></returns>
        public List<HashSet<T>> GetINs()
        {
            var res = new List<HashSet<T>>();
            foreach (var info in BlocksInfo)
                res.Add(new HashSet<T>(info.IN));
            return res;
        }
    }
}
