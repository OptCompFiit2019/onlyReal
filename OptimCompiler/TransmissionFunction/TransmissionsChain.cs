using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang.TransmissionFunction
{
    class TransmissionsChain
    {

        private List<TransmissionFunction> functions;

        public TransmissionsChain()
        {
            functions = new List<TransmissionFunction>();
        }

        public TransmissionsChain(List<TransmissionFunction> functions)
        {
            this.functions = functions ?? throw new ArgumentException("Argument 'function' must be not null");
        }


        public TransmissionsChain Add(TransmissionFunction function)
        {
            functions.Add(function);
            return this;
        }

        public TransmissionsChain AddRange(List<TransmissionFunction> functions)
        {
            functions.AddRange(functions);
            return this;
        }


        public InOut Aplly(InOut inOut)
        {
            foreach (var f in functions)
            {
                if (f != null) inOut = f.Apply(inOut);
            }
            return inOut;
        }


    }
}
