using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MihStatLibrary.Counters
{
    /// <summary>
    /// Исключение, возникающее при попытке посчитать количество единиц на гистограмме частот со смещением отличным от размерности
    /// </summary>
    public class OnesCounterException : Exception
    {
        /// <summary>
        /// Исключение, возникающее при попытке посчитать количество единиц на гистограмме частот со смещением отличным от размерности
        /// </summary>
        public OnesCounterException(string pMessage) : base(pMessage) { }
    }
}
