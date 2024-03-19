using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MihStatLibrary.Calculators
{
    /// <summary>
    /// Исключение, возникающее при попытке посчитать количество единиц на гистограмме частот со смещением отличным от размерности
    /// </summary>
    public class OnesCalculatorException : Exception
    {
        /// <summary>
        /// Исключение, возникающее при попытке посчитать количество единиц на гистограмме частот со смещением отличным от размерности
        /// </summary>
        public OnesCalculatorException(string pMessage) : base(pMessage) { }
    }

    /// <summary>
    /// Исключение, возникающее при попытке посчитать вероятность битов 1 и 0
    /// </summary>
    public class ProbabilityCalculatorException : Exception
    {
        /// <summary>
        /// Исключение, возникающее при попытке посчитать вероятность битов 1 и 0
        /// </summary>
        public ProbabilityCalculatorException(string pMessage) : base(pMessage) { }
    }
}
