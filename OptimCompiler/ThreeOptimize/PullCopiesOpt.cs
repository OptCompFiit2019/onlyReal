using System;
namespace OptLib.ThreeOptimize
{
    public class PullCopiesOpt : ThreeCodeOptimiser
    {
        private bool _apply = false;

        public bool Applyed() => _apply;

        public PullCopiesOpt()
        {
        }

        public void Apply(LinkedList<Visitors.ThreeCode> program)
        {
            _apply = false;
        }
    }
}
