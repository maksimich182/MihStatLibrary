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
    internal class ReduceException : Exception
    {
        public ReduceException(string pMessage) : base(pMessage) { }
    }
}
