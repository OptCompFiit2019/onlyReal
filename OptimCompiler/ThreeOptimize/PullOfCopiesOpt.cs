using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleLang.Visitors;

namespace SimpleLang.ThreeCodeOptimisations
{
    public class PullOfCopiesOpt : ThreeCodeOptimiser
    {
        private bool _apply = false;

        public LinkedList<ThreeCode> Program { set; get; }

        public bool NeedFullCode()
        {
            return false;
        }

        public void Apply(ref List<LinkedList<ThreeCode>> res)
        {
            throw new NotImplementedException();
        }

        public bool Applyed()
        {
            return this._apply;
        }

        public void Apply(ref LinkedList<ThreeCode> program)
        {
            _apply = false;
            Program = program;
            PullCopies();
            program = Program;
        }

        public void PullCopies()
        {
            var list = Program.ToList();
            for (int i = 0; i < Program.Count - 1; i++)
            {

                string left;
                ThreeAddressValueType right;

                if (list[i].arg2 == null)
                {
                    left = list[i].result;
                    right = list[i].arg1;
                }
                else continue;

                for (int j = i + 1; j < Program.Count; j++)
                {
                    if (list[j].result == left)
                        break;
                    if (list[j].arg1.ToString() == left)
                    {
                        list[j].arg1 = right;
                        _apply = true;
                    }

                    if (list[i].arg2 != null && list[j].arg2.ToString() == left) {
                        list[j].arg2 = right;
                        _apply = true;
                    }
                }
            }
        }

        
    }
}
