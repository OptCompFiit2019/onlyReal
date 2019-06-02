using System;
using System.Collections.Generic;
using System.Linq;

namespace GenericTransferFunction
{
    /// <summary>
    /// Класс, реализующий передаточную функцию
    /// </summary>
    public class TransferFunction<T>
    {
        private List<Func<T, T>> algorithms;

        /// <summary>
        /// Создает объект передаточной функции по одному алгоритму
        /// </summary>
        /// <param name="algorithm"></param>
        public TransferFunction(Func<T, T> algorithm)
        {
            algorithms = new List<Func<T, T>>();
            algorithms.Add(algorithm);
        }

        /// <summary>
        /// Создает объект передаточной функции по множеству алгоритмов
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
                if (a != null)
                    set = a.Invoke(set);
                else
                    throw new ArgumentException("Transfer function must not be null");

            return set;
        }
    }
}