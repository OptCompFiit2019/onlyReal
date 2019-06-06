using SimpleLang.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang.ThreeCodeOptimisations
{
    public class DefUseConstOpt : ThreeCodeOptimiser
    {
        private bool _applyed;
        public bool NeedFullCode() { return false; }
        public void Apply(ref List<LinkedList<Visitors.ThreeCode>> res) { throw new Exception("Not implemented"); }

        public void Apply(ref LinkedList<ThreeCode> program)
        {
            _applyed = false;
            var blocksDefs = new List<VarDef>();

            for (var it = program.First; it != null; it = it.Next)
            {
                if (it.Value.operation != ThreeOperator.Goto &&
                    it.Value.operation != ThreeOperator.IfGoto &&
                    it.Value.operation != ThreeOperator.None)
                {
                    if (it.Value.arg2 == null)
                        blocksDefs.Add(new VarDef(it));
                    if (it.Value.arg1 is ThreeAddressStringValue fst)
                    {
                        var findedDef = blocksDefs.LastOrDefault(d => d.Def.Value.result == fst.Value);
                        if (findedDef != null)
                            findedDef.VarUses.Add(new VarUse(it, findedDef));
                    }
                    if (it.Value.arg2 != null && it.Value.arg2 is ThreeAddressStringValue snd)
                    {
                        var findedDef = blocksDefs.LastOrDefault(d => d.Def.Value.result == snd.Value);
                        if (findedDef != null)
                            findedDef.VarUses.Add(new VarUse(it, findedDef));
                    }
                }
            }

            foreach (var def in blocksDefs)
            {
                if (def.Def.Value.arg1 is ThreeAddressIntValue fstInt ||
                    def.Def.Value.arg1 is ThreeAddressLogicValue fstBool ||
                    def.Def.Value.arg1 is ThreeAddressDoubleValue fstDouble)
                {
                    foreach (var use in def.VarUses)
                    {
                        if (use.Line.Value.arg1 is ThreeAddressStringValue fst && fst.Value == def.Def.Value.result)
                        {
                            use.Line.Value.arg1 = def.Def.Value.arg1;
                            _applyed = true;
                        }
                        else if (use.Line.Value.arg2 != null && use.Line.Value.arg2 is ThreeAddressStringValue snd && snd.Value == def.Def.Value.result)
                        {
                            use.Line.Value.arg2 = def.Def.Value.arg1;
                            _applyed = true;
                        }
                    }
                }
            }
        }

        bool ThreeCodeOptimiser.Applyed() => _applyed;
    }
    /// <summary>
    /// Использовать только если базовый блок один
    /// </summary>
    public class DefUseDeadCodeOpt : ThreeCodeOptimiser
    {
        private bool _applyed;

        public bool NeedFullCode() { return false; }
        public void Apply(ref List<LinkedList<Visitors.ThreeCode>> res) { throw new Exception("Not implemented"); }

        public void Apply(ref LinkedList<ThreeCode> program)
        {
            _applyed = false;
            var blocksDefs = new List<VarDef>();

            for (var it = program.First; it != null; it = it.Next)
            {
                if (it.Value.operation != ThreeOperator.Goto &&
                    it.Value.operation != ThreeOperator.IfGoto &&
                    it.Value.operation != ThreeOperator.None)
                {
                    if (it.Value.arg2 == null)
                        blocksDefs.Add(new VarDef(it));
                    if (it.Value.arg1 is ThreeAddressStringValue fst)
                    {
                        var findedDef = blocksDefs.LastOrDefault(d => d.Def.Value.result == fst.Value);
                        if (findedDef != null)
                            findedDef.VarUses.Add(new VarUse(it, findedDef));
                    }
                    if (it.Value.arg2 != null && it.Value.arg2 is ThreeAddressStringValue snd)
                    {
                        var findedDef = blocksDefs.LastOrDefault(d => d.Def.Value.result == snd.Value);
                        if (findedDef != null)
                            findedDef.VarUses.Add(new VarUse(it, findedDef));
                    }
                }
            }
            foreach (var def in blocksDefs)
            {
                if (def.VarUses.Count == 0)
                {
                    program.Remove(def.Def);
                    _applyed = true;
                }
            }
        }

        public bool Applyed() => _applyed;
    }

    class VarDef
    {
        public LinkedListNode<ThreeCode> Def { get; set; }

        public List<VarUse> VarUses { get; }

        public VarDef(LinkedListNode<ThreeCode> defNode)
        {
            Def = defNode;
            VarUses = new List<VarUse>();
        }

    }

    class VarUse
    {
        public LinkedListNode<ThreeCode> Line { get; }

        public VarDef VarDef { get; }

        public VarUse(LinkedListNode<ThreeCode> line, VarDef varDef1)
        {
            Line = line;
            VarDef = varDef1;
        }
    };
}
