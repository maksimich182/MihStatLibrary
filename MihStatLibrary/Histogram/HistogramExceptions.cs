using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MihStatLibrary.Histogram
{
    /// <summary>
    /// Исключение, возникающее при попытке пересчитать гистограмму частот на меньшую размерность
    /// </summary>
    public class ReduceException : Exception
    {
        /// <summary>
        /// Исключение, возникающее при попытке задать сдвиг больше размерности
        /// </summary>
        public ReduceException(string pMessage) : base(pMessage) { }
    }

    /// <summary>
    /// Исключение, возникающее при попытке задать сдвиг больше размерности
    /// </summary>
    public class InvalidShiftException : Exception
    {
        /// <summary>
        /// Исключение, возникающее при попытке задать сдвиг больше размерности
        /// </summary>
        public InvalidShiftException(string pMessage) : base(pMessage) { }
    }
}
