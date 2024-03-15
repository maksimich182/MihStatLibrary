using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MihStatLibrary
{
    /// <summary>
    /// Константы
    /// </summary>
    internal class Tools
    {
        /// <summary>
        /// Количество бит в байте
        /// </summary>
        static public readonly int BITS_IN_BYTE = 8;

        /// <summary>
        /// Количество байт в блоке данных
        /// </summary>
        static public readonly long SIZE_BLOCK_BYTES = 100000000;

        //TO DELETE
        static public readonly long SIZE_BLOCK_BITS = BITS_IN_BYTE * SIZE_BLOCK_BYTES;

        /// <summary>
        /// Квантиль
        /// </summary>
        static public readonly double QUANTILE_1_A2 = 2.80703;

        /// <summary>
        /// Массив количества единиц в байтах
        /// </summary>
        static public readonly int[] ArNumberOne = { 0, 1, 1, 2, 1, 2, 2, 3, 1, 2, 2, 3, 2, 3,
            3, 4, 1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5, 1, 2, 2, 3, 2, 3, 3,
            4, 2, 3, 3, 4, 3, 4, 4, 5, 2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
            1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5, 2, 3, 3, 4, 3, 4, 4, 5, 3,
            4, 4, 5, 4, 5, 5, 6, 2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6, 3, 4,
            4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7, 1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3,
            4, 3, 4, 4, 5, 2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6, 2, 3, 3, 4,
            3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6, 3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5,
            6, 6, 7, 2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6, 3, 4, 4, 5, 4, 5,
            5, 6, 4, 5, 5, 6, 5, 6, 6, 7, 3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6,
            7, 4, 5, 5, 6, 5, 6, 6, 7, 5, 6, 6, 7, 6, 7, 7, 8 };
    }
}
