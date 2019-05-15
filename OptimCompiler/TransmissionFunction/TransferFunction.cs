using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang.TransmissionFunction
{
    
    /// <summary>
    /// Класс, реализующий передаточную функцию
    /// </summary>
    class TransferFunction<T>
    {
        private List<Func<T, T>> algorithms;

        public TransferFunction(Func<T, T> algorithm)
        {
            algorithms = new List<Func<T, T>>();
            algorithms.Add(algorithm);
        }

        /// <summary>
        /// rr
        /// </summary>
        /// <param name="algorithms"></param>
        public TransferFunction(List<Func<T, T>> algorithms) =>
            this.algorithms = algorithms;


        /// <summary>
        /// Оператор * реализует суперпозицию передаточных функций
        /// </summary>
        /// <param name="leftFunc"></param>
        /// <param name="rightFunc"></param>
        /// <returns></returns>
        public static TransferFunction<T> operator *(TransferFunction<T> leftFunc, TransferFunction<T> rightFunc) =>
            new TransferFunction<T>(leftFunc.algorithms.Concat(rightFunc.algorithms).ToList());

        /// <summary>
        /// Применении передаточной функции  (или суперпозиции) к аргументу <paramref name="set"/>
        /// </summary>
        /// <param name="set">Множество входов/выходов</param>
        /// <returns></returns>
        public T Apply(T set)
        {
            foreach (var a in algorithms)
            {
                if (a != null)
                    set = a.Invoke(set);
                else
                    throw new ArgumentException("Transfer function must not be null");
            }

            return set;
        }
    }
}