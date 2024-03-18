using MihStatLibrary;
using MihStatLibrary.Counters;
using MihStatLibrary.Data;
using MihStatLibrary.Histogram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MihStatLibraryTest.OnesCounterTests
{
    /// <summary>
    /// Тесты класса OneCounter
    /// </summary>
    [TestClass]
    public class OneCounterTest
    {
        /// <summary>
        /// Тест на вычисление количества единичных бит в блоке данных:
        /// 1. Сравнивается количество высчитанных бит в блоке данных размером 100.000.000 байт из файла, содержащего байты 00001111 с 
        /// количеством байт в блоке, умноженным на 4 (количество единичных бит в каждом байте)
        /// </summary>
        [TestMethod]
        public void CalculateOneInBlockDataTest()
        {
            int szBlock = 100_000_000;
            FileStream dataStream = new FileStream(DataFiles.File00001111_131MB, FileMode.Open);
            BlockData blockData = new BlockData(new BlockDataFileSource(dataStream));
            blockData.GetBlockData(szBlock);
            Assert.AreEqual(szBlock * 4, OnesCounter.Calculate(blockData));
        }

        /// <summary>
        /// Тест на вычисление количества единичных бит в гистограмме частот:
        /// 1. Сравнивается количество единичных бит в гистограмме частот размерностью 16 (чтоб не осталось необсчитанных хвостов), 
        /// посчитанной на файле, содержащем 131 МБ байт 010100101 с размером файла в битах, 
        /// поделенном на 2 (в каждом байте половина бит единичные)
        /// </summary>
        [TestMethod]
        public void CalculateOneInFreqHistogramTest()
        {
            FreqHistogram fq = new FreqHistogram(16);
            fq.Calculate(DataFiles.File01010101_131MB);
            Assert.AreEqual(OnesCounter.Calculate(fq), (new FileInfo(DataFiles.File01010101_131MB).Length * Tools.BITS_IN_BYTE) / 2);
        }

        /// <summary>
        /// Тест на исключение при вычислении количества единичных бит в гистограмме частот с разными смещением и размерностью:
        /// 1. Проверяется возникновение исключения при попытке посчитать количество единичных бит на гистограмме частот с разными значениями смещения и размерности
        /// </summary>
        [TestMethod]
        public void CalculateOneInFreqHistogramWithDifferentShiftAndDimTest()
        {
            FreqHistogram fq = new FreqHistogram(16, 3);
            fq.Calculate(DataFiles.File11110000_5MB);
            Assert.ThrowsException<OnesCounterException>(() => OnesCounter.Calculate(fq));
        }

        /// <summary>
        /// Тест на вычисление количества единичных бит данных из <see cref="BigInteger"/>:
        /// 1. Сравнивается количество единичных бит в числе 987345834572187234912834 = 
        /// 1101_0001_0001_0100_0001_1111_1111_1100_0011_0000_1111_1001_0111_0000_0011_0010_1100_1010_0100_0010
        /// с 37 (посчитанно вручную)
        /// 2. Сравнивается количество единичных бит в первых 30 битах в числа 987345834572187234912834 = 
        /// 1101_0001_0001_0100_0001_1111_1111_1100_0011_0000_1111_1001_0111_0000_0011_0010_1100_1010_0100_0010
        /// с 37 (посчитанно вручную)
        /// </summary>
        [TestMethod]
        public void CalculateOneInBigIntegerTest()
        {
            BigInteger data = BigInteger.Parse("987345834572187234912834");
            Assert.AreEqual(OnesCounter.Calculate(data), 37);
            Assert.AreEqual(OnesCounter.Calculate(data, 30), 11);
        }
    }
}
