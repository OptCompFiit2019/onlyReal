using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using SimpleLang.Visitors;

namespace SimpleLang.Compiler
{
    class ILCodeGenerator
    {
        private Dictionary<string, LocalBuilder> variables;
        private Dictionary<string, Label> labels;
        private GenCodeCreator genc;

        private void Init(LinkedList<ThreeCode> code) {
            genc = new GenCodeCreator();
        }
        public void Generate(LinkedList<ThreeCode> code)
        {

        }

        public void Execute()
        {

        }
    }
}