using System;
namespace SimpleLang.ThreeOptimize
{
    public class ApplyConst: ThreeCodeApplyer
    {
        private bool _apply = false;

        public ApplyConst()
        {
        }
        public bool Applyed()
        {
            return _apply;
        }
        public void Apply(System.Collections.Generic.LinkedList<Visitors.ThreeCode> program){
            _apply = false;
            //ToDO
            //код оптимизирует for, что неправильно. Нужнообновить код после добавления 
            // разбиения на базовые блоки
            System.Collections.Generic.Dictionary<String, Visitors.ThreeAddressValueType> replace = new System.Collections.Generic.Dictionary<string, Visitors.ThreeAddressValueType>();
            for (var it = program.First; it != null; it = it.Next){

                if (replace.ContainsKey(it.Value.result)) {
                    replace.Remove(it.Value.result);
                }


                if (it.Value.operation == Visitors.ThreeOperator.Assign){
                    if (it.Value.arg1 is Visitors.ThreeAddressStringValue val) {
                        if (replace.ContainsKey(val.Value)) {
                            it.Value.arg1 = replace[val.Value];
                            _apply = true;
                        }
                        continue;
                    }
                    replace[it.Value.result] = it.Value.arg1;
                    continue;
                }

                if (it.Value.arg1 is Visitors.ThreeAddressStringValue name && replace.ContainsKey(name.Value)){
                    it.Value.arg1 = replace[name.Value];
                    _apply = true;
                }
                if (it.Value.arg2 is Visitors.ThreeAddressStringValue name2 && replace.ContainsKey(name2.Value)){
                    it.Value.arg2 = replace[name2.Value];
                    _apply = true;
                }

            }
        }
    }
}
