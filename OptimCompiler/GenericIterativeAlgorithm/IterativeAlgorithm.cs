using System;
using System.Collections.Generic;
using TransferFunctionGeneric;

namespace SimpleLang.GenericIterativeAlgorithm
{
    class test
    {
        IterativeAlgorithm it = new IterativeAlgorithm(true, null, string, string);
    }

    class IterativeAlgorithm
    {
        public bool IsForward { get;}
        public ControlFlowGraph.ControlFlowGraph Graph { get; }
        public List<IBlockInfo> IN { get; }
        public List<IBlockInfo> OUT { get; }
        public Func<ControlFlowGraph.ControlFlowGraph, int, IBlockInfo> MeetOperator;
        public TransferFunction<IBlockInfo> Function { get; }
        public IBlockInfo InitEntryExit { get; }
        public IBlockInfo InitOther { get; }

        public IterativeAlgorithm(bool isForward, ControlFlowGraph.ControlFlowGraph graph, IBlockInfo initValueEntryExit, IBlockInfo initValueOthers)
        {
            IsForward = isForward;
            Graph = graph;
            IN = new List<IBlockInfo>();
            OUT = new List<IBlockInfo>();
        }

        public void Perform()
        {
            int countBlocks = Graph.blocks.Count;
            // инициализация OUT или IN входа или выхода заданным значением
            // for(каждый ББл, отличный от того, который выше) инициализация Остальных OUT или IN
            for (int i = 0; i < countBlocks; i++)
            {
                IN.Add(InitOther);
                OUT.Add(InitOther);
            }

            List<IBlockInfo> MeetResult, FunctionResult;
            MeetResult = IsForward ? IN : OUT;
            FunctionResult = IsForward ? OUT : IN;        

            if (IsForward)
                FunctionResult[0] = InitEntryExit;
            else
                FunctionResult[countBlocks - 1] = InitEntryExit;

            int startIndex = IsForward ? 1 : 0;
            int endIndex = IsForward ? countBlocks : countBlocks - 1;

            bool isChanged = true;
            while (isChanged)
            {
                isChanged = false;
                for(int index = startIndex; index < endIndex; index++)
                {
                    var prevFunctionResult = (IBlockInfo)FunctionResult[index].Clone();
                    MeetResult[index] = MeetOperator(Graph, index);
                    FunctionResult[index] = Function.Apply(MeetResult[index]);
                    isChanged = isChanged || prevFunctionResult.Changed(FunctionResult[index]);
                }
            }
        }
    }


    public interface IBlockInfo : ICloneable
    {
        bool Changed(IBlockInfo previousState);
    }

}
