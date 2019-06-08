using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleLang.Visitors;

namespace SimpleLang.ThreeCodeOptimisations
{
    public class CommonExprOpt : ThreeCodeOptimiser
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
            Optimize();
            program = Program;
        }

        public void Optimize()
        {
            var result = new List<ThreeCode>();
            var program = Program.ToList();
            for (int i = 0; i < Program.Count - 1; i++)
            {
                if (program[i].arg1 == null && program[i].arg2 == null)
                    continue;
                else
                {
                    for (int j = i + 1; j < program.Count; j++)
                    {
                        if (program[j].arg1.ToString() == program[i].arg1.ToString() && program[j].arg2 != null && program[i].arg2 != null && program[j].arg2.ToString() == program[i].arg2.ToString() && program[j].operation == program[i].operation)
                        {
                            program[j].operation = ThreeOperator.Assign;
                            program[j].arg1 = new ThreeAddressStringValue(program[i].result);
                            program[j].arg2 = null;
                        }
                        if (program[j].result == program[i].result || program[j].result == program[i].arg1.ToString() || (program[i].arg2 != null && program[j].result == program[i].arg2.ToString()))
                            break;
                    }
                }
            }

            for (int i = 0; i < program.Count; i++)
                result.Add(program[i]);
            Program = new LinkedList<ThreeCode>(result);
            _apply = true;
        }
    }
}
