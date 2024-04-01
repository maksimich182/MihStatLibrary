using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MihStatLibrary
{
    /// <summary>
    /// Константы
    /// </summary>
    public class Tools
    {
        /// <summary>
        /// Количество бит в байте
        /// </summary>
        static public readonly int BITS_IN_BYTE = 8;

        /// <summary>
        /// Количество байт в блоке данных
        /// </summary>
        static public readonly int SIZE_BLOCK_BYTES = 100000000;

        //TO DELETE
        static public readonly int SIZE_BLOCK_BITS = BITS_IN_BYTE * SIZE_BLOCK_BYTES;

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

        /// <summary>
        /// Вычисление процента
        /// </summary>
        /// <param name="number">Число</param>
        /// <param name="maxNumber">Максимальное число</param>
        /// <returns>Процент</returns>
        public static int GetPercent(long number, long maxNumber)
        {
            return (int)((double)number / maxNumber) * 100;
        }

        /// <summary>
        /// Функция для деления больших чисел <see cref="BigInteger"/>
        /// </summary>
        /// <param name="dividend">Делимое</param>
        /// <param name="divisor">Дулитель</param>
        /// <returns>Результат деления в <see cref="double"/></returns>
        public static double BigIntDevider(BigInteger dividend, BigInteger divisor)
        {
            const long numberForDivision = 1000000000000000000;
            double result = 0;
            BigInteger remainder;
            BigInteger intResDiv = BigInteger.DivRem(dividend, divisor, out remainder);
            result = (double)intResDiv + ((double)BigInteger.Divide((remainder * numberForDivision), divisor) / numberForDivision);
            return result;
        }

        /// <summary>
        /// Функция для деления числа <see cref="double"/> на число <see cref="BigInteger"/>
        /// </summary>
        /// <param name="dividend">Делимое</param>
        /// <param name="divisor">Дулитель</param>
        /// <returns>Результат деления в <see cref="double"/></returns>
        public static double BigIntDevider(double dividend, BigInteger divisor)
        {
            const long numberForDivision = 1000000000000000000;
            BigInteger divident = (BigInteger)(dividend * numberForDivision);
            double result = BigIntDevider(divident, divisor);
            result /= numberForDivision;
            return result;
        }
    }
}
