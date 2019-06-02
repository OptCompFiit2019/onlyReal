using System;
using System.Collections.Generic;
using GenericTransferFunction;
using SimpleLang.Visitors;

namespace SimpleLang.GenericIterativeAlgorithm
{
    using BasicBlock = LinkedList<ThreeCode>;

    /// <summary>
    /// Вспомогательный класс, хранящий информацию о множествах отдельного блока, таких как
    /// IN, OUT, def, use, gen, kill, e_gen, e_kill
    /// </summary>
    public class BlockInfo<T>
    {
        public BasicBlock Commands;
        public ISet<T> IN;
        public ISet<T> OUT;
        public ISet<T> HelpFirst; // Аналог def_b, gen_b, e_gen_b
        public ISet<T> HelpSecond; // Аналог use_b, kill_b, e_kill_b

        /// <summary>
        /// Определяет равенство текущего объекта и <paramref name="other"/> по совпадению полей.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(BlockInfo<T> other)
        {
            if (IN.Count != other.IN.Count || OUT.Count != other.OUT.Count
                || HelpFirst.Count != other.HelpFirst.Count || HelpSecond.Count != other.HelpSecond.Count)
                return false;

            return IN.SetEquals(other.IN) && OUT.SetEquals(other.OUT)
                && HelpFirst.SetEquals(other.HelpFirst) && HelpSecond.SetEquals(other.HelpSecond);
        }
        
        public BlockInfo(BasicBlock commands)
        {
            Commands = commands;
            IN = new HashSet<T>();
            OUT = new HashSet<T>();
            HelpFirst = new HashSet<T>();
            HelpSecond = new HashSet<T>();
        }

        public BlockInfo(BlockInfo<T> source)
        {
            Commands = new LinkedList<ThreeCode>(source.Commands);
            IN = new HashSet<T>(source.IN);
            OUT = new HashSet<T>(source.OUT);
            HelpFirst = new HashSet<T>(source.HelpFirst);
            HelpSecond = new HashSet<T>(source.HelpSecond);
        }

        public BlockInfo(ISet<T> helpFirst, ISet<T> helpSecond)
        {
            IN = new HashSet<T>();
            OUT = new HashSet<T>();
            HelpFirst = helpFirst;
            HelpSecond = helpSecond;
        }

        public BlockInfo(ISet<T> helpFirst, ISet<T> helpSecond, ISet<T> In, ISet<T> Out)
        {
            IN = In;
            OUT = Out;
            HelpFirst = helpFirst;
            HelpSecond = helpSecond;
        }
    }
    
    public class IterativeAlgorithm<T>
    {
        // направление обхода: true - прямой, false - обратный
        public bool IsForward { get; }
        public ControlFlowGraph.ControlFlowGraph Graph { get; }
        // информация о блоках. Каждому блоку с индексом i в графе соответствует экземпляр
        // класса BlockInfo<T> в списке BlocksInfo с тем же индексом
        public List<BlockInfo<T>> BlocksInfo { get; }
        // оператор сбора
        public Func<List<BlockInfo<T>>, ControlFlowGraph.ControlFlowGraph, int, BlockInfo<T>> MeetOperator;
        // передаточная функция
        public TransferFunction<BlockInfo<T>> Function { get; }
        // значение инициализации IN/OUT первого или последнего блока для прямого или обратного обходов соответственно
        public ISet<T> InitEntryExit { get; }
        // значение инициализации IN/OUT для остальных блоков
        public ISet<T> InitOther { get; }

        public IterativeAlgorithm(List<BlockInfo<T>> blocksInfo, ControlFlowGraph.ControlFlowGraph graph,
            Func<List<BlockInfo<T>>, ControlFlowGraph.ControlFlowGraph, int, BlockInfo<T>> meetOperator, bool isForward,
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
        }

        /// <summary>
        /// Выполняет оптимизационный алгоритм и возвращает результат как List<BlockInfo<T>>,
        /// </summary>
        /// <returns></returns>
        public List<BlockInfo<T>> Perform()
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
                for (int index = startIndex; index < endIndex; index++)
                {
                    var prevFunctionResult = new BlockInfo<T>(BlocksInfo[index]);
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
