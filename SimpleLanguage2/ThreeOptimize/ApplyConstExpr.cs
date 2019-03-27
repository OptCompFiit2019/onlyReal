using System;
namespace SimpleLang.ThreeOptimize
{
    public class ApplyConstExpr : ThreeCodeApplyer
    {
        private bool _apply = false;

        public ApplyConstExpr()
        {
        }
        public bool Applyed()
        {
            return _apply;
        }
        private void Set(System.Collections.Generic.LinkedListNode<Visitors.ThreeCode> it, int res) {
            it.Value.operation = Visitors.ThreeOperator.Assign;
            it.Value.arg1 = new Visitors.ThreeAddressIntValue(res);
            _apply = true;
        }
        private void Set(System.Collections.Generic.LinkedListNode<Visitors.ThreeCode> it, bool res){
            it.Value.operation = Visitors.ThreeOperator.Assign;
            it.Value.arg1 = new Visitors.ThreeAddressLogicValue(res);
            _apply = true;
        }
        public void Apply(System.Collections.Generic.LinkedList<Visitors.ThreeCode> program)
        {
            _apply = false;
            // ToDO
            // Оптимизировать для базовых блоков.
            // Работать будет и без базовых блоков, но нужно под базовые блоки переделть
            for (var it = program.First; it != null; it = it.Next) {
                int iarg1 = 0;
                bool iarg1val = false;
                int iarg2 = 0;
                bool iarg2val = false;

                bool barg1 = false;
                bool barg1val = false;
                bool barg2 = false;
                bool barg2val = false;

                if (it.Value.arg1 != null) {
                    if (it.Value.arg1 is Visitors.ThreeAddressIntValue val) {
                        iarg1val = true;
                        iarg1 = val.Value;
                    }
                    if (it.Value.arg1 is Visitors.ThreeAddressLogicValue vall) {
                        iarg1val = true;
                        barg1 = vall.Value;
                    }
                }

                if (it.Value.arg2 != null) {
                    if (it.Value.arg2 is Visitors.ThreeAddressIntValue val) {
                        iarg2val = true;
                        iarg2 = val.Value;
                    }
                    if (it.Value.arg2 is Visitors.ThreeAddressLogicValue vall) {
                        iarg2val = true;
                        barg2 = vall.Value;
                    }
                }

                // public enum ThreeOperator {  Logic_or, Logic_and, Logic_less, Logic_equal, Logic_greater, Logic_geq, Logic_leq,
                //Logic_not, Logic_neq };

                if (it.Value.operation == Visitors.ThreeOperator.Logic_or && barg1val && barg2val) {

                }
                if (it.Value.operation == Visitors.ThreeOperator.Logic_and && barg1val && barg2val){
                    Set(it, barg1 && barg2);
                }


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

                // Ariphetic

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
            }
        }
    }
}
