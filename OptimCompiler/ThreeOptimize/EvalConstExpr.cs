using System;
using System.Collections.Generic;

namespace SimpleLang.ThreeCodeOptimisations{

    public class EvalConstExpr : ThreeCodeOptimiser{
        private bool _apply = false;

        public EvalConstExpr()
        {
        }
        public bool Applyed() => _apply;

        private void Set(LinkedListNode<Visitors.ThreeCode> it, int res) {
            it.Value.operation = Visitors.ThreeOperator.Assign;
            it.Value.arg1 = new Visitors.ThreeAddressIntValue(res);
            _apply = true;
        }
        private void Set(LinkedListNode<Visitors.ThreeCode> it, double res) {
            it.Value.operation = Visitors.ThreeOperator.Assign;
            it.Value.arg1 = new Visitors.ThreeAddressDoubleValue(res);
            _apply = true;
        }
        private void Set(LinkedListNode<Visitors.ThreeCode> it, bool res){
            it.Value.operation = Visitors.ThreeOperator.Assign;
            it.Value.arg1 = new Visitors.ThreeAddressLogicValue(res);
            _apply = true;
        }
        public bool NeedFullCode() { return false; }
        public void Apply(ref List<LinkedList<Visitors.ThreeCode>> res) { throw new Exception("Not implemented"); }
        public void Apply(ref LinkedList<Visitors.ThreeCode> program) {
            _apply = false;

            for (var it = program.First; it != null; it = it.Next) {
                int iarg1 = 0;
                bool iarg1val = false;
                int iarg2 = 0;
                bool iarg2val = false;

                bool barg1 = false;
                bool barg1val = false;
                bool barg2 = false;
                bool barg2val = false;

                double darg1 = 0;
                bool darg1val = false;
                double darg2 = 0;
                bool darg2val = false;

                if (it.Value.arg1 != null) {
                    if (it.Value.arg1 is Visitors.ThreeAddressIntValue val) {
                        iarg1val = true;
                        iarg1 = val.Value;
                    }
                    if (it.Value.arg1 is Visitors.ThreeAddressLogicValue vall) {
                        barg1val = true;
                        barg1 = vall.Value;
                    }
                    if (it.Value.arg1 is Visitors.ThreeAddressDoubleValue val2) {
                        darg1val = true;
                        darg1 = val2.Value;
                    }
                }

                if (it.Value.arg2 != null) {
                    if (it.Value.arg2 is Visitors.ThreeAddressIntValue val) {
                        iarg2val = true;
                        iarg2 = val.Value;
                    }
                    if (it.Value.arg2 is Visitors.ThreeAddressLogicValue vall) {
                        barg2val = true;
                        barg2 = vall.Value;
                    }
                    if (it.Value.arg2 is Visitors.ThreeAddressDoubleValue val2) {
                        darg2val = true;
                        darg2 = val2.Value;
                    }
                }

                // public enum ThreeOperator {  Logic_or, Logic_and, Logic_less, Logic_equal, Logic_greater, Logic_geq, Logic_leq,
                //Logic_not, Logic_neq };

                if (it.Value.operation == Visitors.ThreeOperator.Logic_or && barg1val && barg2val) {
                    Set(it, barg1 || barg2);
                }
                if (it.Value.operation == Visitors.ThreeOperator.Logic_and && barg1val && barg2val){
                    Set(it, barg1 && barg2);
                }

                // For int
                if (it.Value.operation == Visitors.ThreeOperator.Logic_less && iarg1val && iarg2val){
                    Set(it, iarg1 < iarg2);
                }
                if (it.Value.operation == Visitors.ThreeOperator.Logic_equal && iarg1val && iarg2val) {
                    Set(it, iarg1 == iarg2);
                }
                if (it.Value.operation == Visitors.ThreeOperator.Logic_equal && barg1val && barg2val) {
                    Set(it, barg1 == barg2);
                }
                if (it.Value.operation == Visitors.ThreeOperator.Logic_greater && iarg1val && iarg2val) {
                    Set(it, iarg1 > iarg2);
                }
                if (it.Value.operation == Visitors.ThreeOperator.Logic_geq && iarg1val && iarg2val) {
                    Set(it, iarg1 >= iarg2);
                }
                if (it.Value.operation == Visitors.ThreeOperator.Logic_leq && iarg1val && iarg2val){
                    Set(it, iarg1 <= iarg2);
                }
                if (it.Value.operation == Visitors.ThreeOperator.Logic_neq){
                    Set(it, it.Value.arg1.Equals(it.Value.arg2));
                }
                if (it.Value.operation == Visitors.ThreeOperator.Logic_not && barg1val){
                    Set(it, !barg1);
                }


                //Double logic
                if (it.Value.operation == Visitors.ThreeOperator.Logic_less && darg1val && darg2val){
                    Set(it, darg1 < darg2);
                }
                if (it.Value.operation == Visitors.ThreeOperator.Logic_equal && darg1val && darg2val) {
                    Set(it, Math.Abs(darg1 - darg2) < 0.000001);
                }
                if (it.Value.operation == Visitors.ThreeOperator.Logic_greater && darg1val && darg2val){
                    Set(it, darg1 > darg2);
                }
                if (it.Value.operation == Visitors.ThreeOperator.Logic_geq && darg1val && darg2val){
                    Set(it, darg1 >= darg2);
                }
                if (it.Value.operation == Visitors.ThreeOperator.Logic_leq && darg1val && darg2val) {
                    Set(it, darg1 <= darg2);
                }

                // For int double
                if (it.Value.operation == Visitors.ThreeOperator.Logic_less && iarg1val && darg2val) {
                    Set(it, iarg1 < darg2);
                }
                if (it.Value.operation == Visitors.ThreeOperator.Logic_equal && iarg1val && darg2val){
                    Set(it, Math.Abs(iarg1 - darg2) < 0.000001);
                }
                if (it.Value.operation == Visitors.ThreeOperator.Logic_greater && iarg1val && darg2val){
                    Set(it, iarg1 > darg2);
                }
                if (it.Value.operation == Visitors.ThreeOperator.Logic_geq && iarg1val && darg2val) {
                    Set(it, iarg1 >= darg2);
                }
                if (it.Value.operation == Visitors.ThreeOperator.Logic_leq && iarg1val && darg2val){
                    Set(it, iarg1 <= darg2);
                }

                // For double int
                if (it.Value.operation == Visitors.ThreeOperator.Logic_less && darg1val && iarg2val){
                    Set(it, darg1 < iarg2);
                }
                if (it.Value.operation == Visitors.ThreeOperator.Logic_equal && darg1val && iarg2val){
                    Set(it, Math.Abs(darg1 - iarg2) < 0.000001);
                }
                if (it.Value.operation == Visitors.ThreeOperator.Logic_greater && darg1val && iarg2val){
                    Set(it, darg1 > iarg2);
                }
                if (it.Value.operation == Visitors.ThreeOperator.Logic_geq && darg1val && iarg2val) {
                    Set(it, darg1 >= iarg2);
                }
                if (it.Value.operation == Visitors.ThreeOperator.Logic_leq && darg1val && iarg2val) {
                    Set(it, darg1 <= iarg2);
                }

                // Ariphetic
                // int
                if (it.Value.operation == Visitors.ThreeOperator.Minus && iarg1val && iarg2val){
                    Set(it, iarg1 - iarg2);
                }
                if (it.Value.operation == Visitors.ThreeOperator.Plus && iarg1val && iarg2val) {
                    Set(it, iarg1 + iarg2);
                }
                if (it.Value.operation == Visitors.ThreeOperator.Mult && iarg1val && iarg2val) {
                    Set(it, iarg1 * iarg2);
                }
                if (it.Value.operation == Visitors.ThreeOperator.Div && iarg1val && iarg2val) {
                    Set(it, iarg1 / iarg2);
                }

                // double
                if (it.Value.operation == Visitors.ThreeOperator.Minus && darg1val && darg2val) {
                    Set(it, darg1 - darg2);
                }
                if (it.Value.operation == Visitors.ThreeOperator.Plus && darg1val && darg2val) {
                    Set(it, darg1 + darg2);
                }
                if (it.Value.operation == Visitors.ThreeOperator.Mult && darg1val && darg2val) {
                    Set(it, darg1 * darg2);
                }
                if (it.Value.operation == Visitors.ThreeOperator.Div && darg1val && darg2val) {
                    Set(it, darg1 / darg2);
                }
                // int double
                if (it.Value.operation == Visitors.ThreeOperator.Minus && iarg1val && darg2val) {
                    Set(it, iarg1 - darg2);
                }
                if (it.Value.operation == Visitors.ThreeOperator.Plus && iarg1val && darg2val) {
                    Set(it, iarg1 + darg2);
                }
                if (it.Value.operation == Visitors.ThreeOperator.Mult && iarg1val && darg2val) {
                    Set(it, iarg1 * darg2);
                }
                if (it.Value.operation == Visitors.ThreeOperator.Div && iarg1val && darg2val) {
                    Set(it, iarg1 / darg2);
                }

                // double int
                if (it.Value.operation == Visitors.ThreeOperator.Minus && darg1val && iarg2val){
                    Set(it, darg1 - iarg2);
                }
                if (it.Value.operation == Visitors.ThreeOperator.Plus && darg1val && iarg2val) {
                    Set(it, darg1 + iarg2);
                }
                if (it.Value.operation == Visitors.ThreeOperator.Mult && darg1val && iarg2val) {
                    Set(it, darg1 * iarg2);
                }
                if (it.Value.operation == Visitors.ThreeOperator.Div && darg1val && iarg2val) {
                    Set(it, darg1 / iarg2);
                }
            }
        }
    }
}
