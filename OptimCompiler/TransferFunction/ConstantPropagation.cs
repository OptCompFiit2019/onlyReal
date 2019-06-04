using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GenericTransferFunction;
using SimpleLang.GenericIterativeAlgorithm;
using SimpleLang.Visitors;

namespace SimpleLang.ThreeCodeOptimisations
{
    using ConstPropBlockInfo = BlockInfo<KeyValuePair<string, ConstPropSemilatticeEl>>;
    using ConstPropKeyValue = KeyValuePair<string, ConstPropSemilatticeEl>;

    public enum ValConstType { NAC, Const, Undef }

    public class ConstPropSemilatticeEl
    {
        public ValConstType Constantness;
        public int Value;

        public ConstPropSemilatticeEl(ValConstType ctype, int value = 0)
        {
            Constantness = ctype;
            Value = value;
        }

        public override bool Equals(object obj)
            => obj is ConstPropSemilatticeEl other &&
                Constantness == other.Constantness && Value == other.Value;

        public override int GetHashCode() => base.GetHashCode();
    }

    public partial class ConstantPropagationOptimizer
    {
        public static TransferFunction<ConstPropBlockInfo> TransferFunction()
            => new TransferFunction<ConstPropBlockInfo>(bi =>
            {
                var m = bi.IN.ToDictionary(e => e.Key);
                foreach (var command in bi.Commands)
                {
                    if (command.operation == ThreeOperator.Goto
                            || command.operation == ThreeOperator.IfGoto
                            || command.operation == ThreeOperator.None
                            || command.operation == ThreeOperator.Println
                            || command.arg1 is ThreeAddressLogicValue
                            || command.arg1 is ThreeAddressDoubleValue
                            || command.arg2 is ThreeAddressLogicValue
                            || command.arg2 is ThreeAddressDoubleValue)
                        continue;
                    if (command.operation == ThreeOperator.Assign)
                        m[command.result] = new ConstPropKeyValue(command.result,
                            GetSemilatticeEl(command.arg1, m));
                    else
                    {
                        var el1 = GetSemilatticeEl(command.arg1, m);
                        var el2 = GetSemilatticeEl(command.arg2, m);
                        if (el1.Constantness == ValConstType.Const
                                && el2.Constantness == ValConstType.Const)
                            m[command.result] = new ConstPropKeyValue(command.result,
                                new ConstPropSemilatticeEl(ValConstType.Const,
                                    EvalConst(el1.Value, el2.Value, command.operation)));
                        else if (el1.Constantness == ValConstType.NAC
                                || el2.Constantness == ValConstType.NAC)
                            m[command.result] = new ConstPropKeyValue(command.result,
                                new ConstPropSemilatticeEl(ValConstType.NAC));
                        else
                            m[command.result] = new ConstPropKeyValue(command.result,
                                new ConstPropSemilatticeEl(ValConstType.Undef));
                    }
                }
                var Out = new ConstPropBlockInfo(bi);
                Out.OUT = new HashSet<ConstPropKeyValue>(m.Values);
                return Out;
            });

        private static int EvalConst(int c1, int c2, ThreeOperator op)
        {
            switch (op)
            {
                case ThreeOperator.Plus:  return c1 + c2;
                case ThreeOperator.Minus: return c1 - c2;
                case ThreeOperator.Mult:  return c1 * c2;
                case ThreeOperator.Div:   return c1 / c2;
                default: throw new Exception("Logic error");
            }
        }

        private static ConstPropSemilatticeEl GetSemilatticeEl
            (ThreeAddressValueType val,
             Dictionary<string, ConstPropKeyValue> m)
        {
            ConstPropSemilatticeEl semilatticeEl = null;
            if (val is ThreeAddressStringValue v)
                semilatticeEl = m[v.Value].Value;
            else if (val is ThreeAddressIntValue c)
                semilatticeEl = new ConstPropSemilatticeEl(ValConstType.Const, c.Value);

            return semilatticeEl;
        }
    }
}
