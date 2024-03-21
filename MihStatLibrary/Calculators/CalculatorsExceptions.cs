using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MihStatLibrary.Calculators
{
    /// <summary>
    /// Исключение, возникающее при попытке посчитать количество единиц на маркировочной таблице со смещением отличным от размерности
    /// </summary>
    public class OnesCalculatorException : Exception
    {
        /// <summary>
        /// Исключение, возникающее при попытке посчитать количество единиц на маркировочной таблице со смещением отличным от размерности
        /// </summary>
        public OnesCalculatorException(string pMessage) : base(pMessage) { }
    }

    /// <summary>
    /// Исключение, возникающее при попытке посчитать оценки вероятностей бит 1 и 0
    /// </summary>
    public class BitFrequencyCalculatorException : Exception
    {
        /// <summary>
        /// Исключение, возникающее при попытке посчитать оценки вероятностей бит 1 и 0
        /// </summary>
        public BitFrequencyCalculatorException(string pMessage) : base(pMessage) { }
    }
}
