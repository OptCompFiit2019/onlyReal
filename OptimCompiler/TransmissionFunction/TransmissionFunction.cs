using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang.TransmissionFunction
{

    struct InOut
    {
        public ISet<string> In;
        public ISet<string> Out;

        public InOut(ISet<string> inSet, ISet<string> outSet)
        {
            In = inSet;
            Out = outSet;
        }
    }

    class TransmissionFunction
    {
        private Func<InOut, InOut> algorithm;

        public TransmissionFunction(Func<InOut, InOut> algorithm)
        {
            this.algorithm = algorithm;
        } 

        public InOut Apply(InOut inOut) => algorithm.Invoke(inOut);        
    }
}