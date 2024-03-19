using MihStatLibrary.Data;
using MihStatLibrary.Histogram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MihStatLibrary.Calculators
{
    /// <summary>
	/// Класс подсчета количества единичных бит
	/// </summary>
	public static class OnesCalculator
    {
        /// <summary>
        /// Рассчет количества "1" в блоке данных
        /// </summary>
        /// <param name="blockData">Блок данных</param>
        /// <returns>Количество единичных бит</returns>
        static public double Calculate(BlockData blockData)
        {
            double result = 0;
            foreach (var element in blockData.Data)
            {
                result += Tools.ArNumberOne[element];
            }
            return result;
        }

        /// <summary>
        /// Рассчет количества "1" в гистограмме частот. СМЕЩЕНИЕ ГИСТОГРАММЫ ДОЛЖНО БЫТЬ РАЗНО РАЗМЕРНОСТИ!
        /// </summary>
        /// <param name="freqHistogram">Гистограмма частот. СМЕЩЕНИЕ ГИСТОГРАММЫ ДОЛЖНО БЫТЬ РАЗНО РАЗМЕРНОСТИ!</param>
        /// <returns>Количество единичных бит</returns>
        /// <exception cref="OnesCounterException">Попытка посчитать количество единичных бит на гистограмме с разными значениями смещения и размерности</exception>
        static public double Calculate(FreqHistogram freqHistogram)
        {
            if (freqHistogram.Dimension != freqHistogram.SzShift)
                throw new OnesCounterException("Смещение гистограммы должно быть равно размерности!");

            double result = 0;
            for (int i = 0; i < freqHistogram.Histogram.Length; i++)
            {
                result += freqHistogram.Histogram[i] * Calculate(i);
            }
            return result;
        }

        /// <summary>
        /// Рассчет количества "1" в первых <paramref name="szDataBits"/> бит из <see cref="BigInteger"/>
        /// </summary>
        /// <param name="data"><see cref="BigInteger"/> из которого берется определенное количество бит</param>
        /// <param name="szDataBits">Количество бит для обсчета</param>
        /// <returns>Количество единичных бит</returns>
        static public long Calculate(BigInteger data, int szDataBits)
        {
            BigInteger dataBitsMask = ((BigInteger)1 << szDataBits) - 1;
            BigInteger dataBuffer = data & dataBitsMask;
            return Calculate(dataBuffer);
        }

        /// <summary>
		/// Рассчет количества единичных бит в <see cref="BigInteger"/>
		/// </summary>
		/// <param name="data">Данные</param>
		/// <returns>Количество единичных бит</returns>
		static public long Calculate(BigInteger data)
        {
            long result = 0;
            BigInteger dataBuffer = data;
            long szDataBits = (long)Math.Ceiling(BigInteger.Log(data + 1, 2));
            long nmDataBytes = (long)Math.Ceiling((double)szDataBits / Tools.BITS_IN_BYTE);
            byte dataByteMask = 0xFF;
            for (int i = 0; i < nmDataBytes; i++)
            {
                result += Tools.ArNumberOne[(int)(dataBuffer & dataByteMask)];
                dataBuffer >>= Tools.BITS_IN_BYTE;
            }
            return result;
        }
    }
}
