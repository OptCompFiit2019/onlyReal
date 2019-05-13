using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang.TransmissionFunction
{
    
    /// <summary>
    /// Класс, реализующий передаточную функцию
    /// </summary>
    class TransmissionFunction<T>
    {
        private List<Func<T, T>> algorithms;

        /// <summary>
        /// Конструктор. Просто конструктор.
        /// </summary>
        /// <param name="algorithm">Алгоритм передаточной функции.</param>
        public TransmissionFunction(Func<T, T> algorithm)
        {
            algorithms = new List<Func<T, T>>();
            algorithms.Add(algorithm);
        }



        public TransmissionFunction(List<Func<T, T>> algorithms)
        {
            this.algorithms = algorithms;
        }



        public static TransmissionFunction<T> operator *(TransmissionFunction<T> leftFunc, TransmissionFunction<T> rightFunc)
        {
            return new TransmissionFunction<T>(leftFunc.algorithms.Concat(rightFunc.algorithms).ToList());
        }

        public T Apply(T set)
        {
            foreach (var a in algorithms)
            {
                if (a != null && set != null)
                {
                    set = a.Invoke(set);
                }
                else
                {
                    throw new ArgumentException("Some shit");
                }
            }

            return set;
        }
    }
}