using System;
namespace SimpleLang.Visitors
{
    public class AutoApplyVisitorInterface: ChangeVisitor
    {
        public AutoApplyVisitorInterface()
        {
        }
        private bool _apply = false;
        protected void SetApply(bool val = true) { _apply = val;  }
        public void Reset() { _apply = false; }
        public bool Applyed() { return _apply; }
    }
}
