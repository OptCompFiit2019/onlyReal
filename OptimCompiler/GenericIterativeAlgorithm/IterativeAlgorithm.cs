using System;
using System.Collections.Generic;
using GenericTransferFunction;

namespace SimpleLang.GenericIterativeAlgorithm
{
    /// <summary>
    /// Вспомогательный класс, хранящий информацию о множествах отдельного блока, таких как
    /// IN, OUT, def, use, gen, kill, e_gen, e_kill
    /// </summary>
    class BlockInfo
    {
        public ISet<string> IN;
        public ISet<string> OUT;
        public ISet<string> HelpFirst; // Аналог def_b, gen_b, e_gen_b
        public ISet<string> HelpSecond; // Аналог use_b, kill_b, e_kill_b

        /// <summary>
        /// Определяет равенство текущего объекта и <paramref name="other"/> по совпадению полей.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(BlockInfo other)
        {
            if (IN.Count != other.IN.Count || OUT.Count != other.OUT.Count
                || HelpFirst.Count != other.HelpFirst.Count || HelpSecond.Count != other.HelpSecond.Count)
                return false;

            return IN.SetEquals(other.IN) && OUT.SetEquals(other.OUT) 
                && HelpFirst.SetEquals(other.HelpFirst) && HelpSecond.SetEquals(other.HelpSecond);
        }

        public BlockInfo(BlockInfo source)
        {
            IN = new HashSet<string>(source.IN);
            OUT = new HashSet<string>(source.OUT);
            HelpFirst = new HashSet<string>(source.HelpFirst);
            HelpSecond = new HashSet<string>(source.HelpSecond);
        }

        public BlockInfo(ISet<string> helpFirst, ISet<string> helpSecond)
        {
            IN = new HashSet<string>();
            OUT = new HashSet<string>();
            HelpFirst = helpFirst;
            HelpSecond = helpSecond;
        }

        public BlockInfo(ISet<string> helpFirst, ISet<string> helpSecond, ISet<string> In, ISet<string> Out)
        {
            IN = In;
            OUT = Out;
            HelpFirst = helpFirst;
            HelpSecond = helpSecond;
        }
    }

    class IterativeAlgorithm
    {
        // направление обхода: true - прямой, false - обратный
        public bool IsForward { get;}
        public ControlFlowGraph.ControlFlowGraph Graph { get; }
        // информация о блоках. Каждому блоку с индексом i в графе соответствует экземпляр
        // класса BlockInfo в списке BlocksInfo с тем же индексом
        public List<BlockInfo> BlocksInfo { get; }
        // оператор сбора
        public Func<List<BlockInfo>, ControlFlowGraph.ControlFlowGraph, int, BlockInfo> MeetOperator;
        // передаточная функция
        public TransferFunction<BlockInfo> Function { get; }
        // значение инициализации IN/OUT первого или последнего блока для прямого или обратного обходов соответственно
        public ISet<string> InitEntryExit { get; }
        // значение инициализации IN/OUT для остальных блоков
        public ISet<string> InitOther { get; }

        public IterativeAlgorithm(List<BlockInfo> blocksInfo, ControlFlowGraph.ControlFlowGraph graph, 
            Func<List<BlockInfo>, ControlFlowGraph.ControlFlowGraph, int, BlockInfo> meetOperator, bool isForward, 
            IEnumerable<string> initValueEntryExit, IEnumerable<string> initValueOthers,
            TransferFunction<BlockInfo> function)
        {
            BlocksInfo = blocksInfo;
            Graph = graph;
            MeetOperator = meetOperator;
            IsForward = isForward;
            InitEntryExit = new HashSet<string>(initValueEntryExit);
            InitOther = new HashSet<string>(initValueOthers);
            Function = function;
        }

        /// <summary>
        /// Выполняет оптимизационный алгоритм и возвращает результат как List<BlockInfo>,
        /// </summary>
        /// <returns></returns>
        public List<BlockInfo> Perform()
        {
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

            int startIndex = IsForward ? 1 : 0;
            int endIndex = IsForward ? countBlocks : countBlocks - 1;

            bool isChanged = true;
            while (isChanged)
            {
                isChanged = false;
                for(int index = startIndex; index < endIndex; index++)
                {
                    var prevFunctionResult = new BlockInfo(BlocksInfo[index]);
                    BlocksInfo[index] = MeetOperator(BlocksInfo, Graph, index);
                    BlocksInfo[index] = Function.Apply(BlocksInfo[index]);
                    isChanged = isChanged || !prevFunctionResult.Equals(BlocksInfo[index]);
                }
            }
            return BlocksInfo;
        }

        /// <summary>
        /// Возвращает список множеств OUT для блоков
        /// </summary>
        /// <returns></returns>
        public List<HashSet<string>> GetOUTs()
        {
            var res = new List<HashSet<string>>();
            foreach (var info in BlocksInfo)
                res.Add(new HashSet<string>(info.OUT));
            return res;
        }

        /// <summary>
        /// Возвращает список множеств IN для блоков
        /// </summary>
        /// <returns></returns>
        public List<HashSet<string>> GetINs()
        {
            var res = new List<HashSet<string>>();
            foreach (var info in BlocksInfo)
                res.Add(new HashSet<string>(info.IN));
            return res;
        }

    }

}
