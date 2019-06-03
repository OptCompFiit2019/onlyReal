using System;
using System.Collections.Generic;
using SimpleLang.Visitors;

namespace SimpleLang.ThreeCodeOptimisations {

    public interface ThreeCodeOptimiser {
        /// <summary>
        /// Применить оптимизацию к базовому блоку.
        /// Если оптимизации необходим сразу весь код программы, то сделать заглушку.
        /// </summary>
        /// <param name="program">базовый блок.</param>
        void Apply(ref LinkedList<ThreeCode> program);
        /// <summary>
        /// Необходим ли оптимизации весь код программы, а не базового блока
        /// </summary>
        /// <returns><c>true</c>, if full code was needed, <c>false</c> otherwise.</returns>
        bool NeedFullCode();
        /// <summary>
        /// Применить оптимизацию ко всему коду программы.
        /// Если оптимизацию необходимо выполнять только для блока, то сделать заглушку.
        /// </summary>
        /// <param name="res">Res.</param>
        void Apply(ref List<LinkedList<ThreeCode>> res);
        /// <summary>
        /// Удалось ли выполнить оптимизацию?
        /// </summary>
        /// <returns>The applyed.</returns>
        bool Applyed();
    }
}
