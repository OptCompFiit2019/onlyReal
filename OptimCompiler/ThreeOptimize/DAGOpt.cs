using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleLang.Visitors;

namespace SimpleLang.ThreeCodeOptimisations
{
    public class DAGOpt : ThreeCodeOptimiser
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
            var dag = new DAG(Program.ToList());
            Program = dag.Optimize(ref _apply);
            program = Program;
        }
    }
}
