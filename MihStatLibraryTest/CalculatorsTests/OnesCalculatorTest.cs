using MihStatLibrary;
using MihStatLibrary.Calculators;
using MihStatLibrary.Data;
using MihStatLibrary.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MihStatLibraryTest.OnesCalculatorTests
{
    /// <summary>
    /// Тесты класса OnesCalculator
    /// </summary>
    [TestClass]
    public class OnesCalculatorTest
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
            Assert.AreEqual(szBlock * 4, OnesCalculator.Calculate(blockData));
        }

        /// <summary>
        /// Тест на вычисление количества единичных бит в маркировочной таблице:
        /// 1. Сравнивается количество единичных бит в маркировочной таблице размерностью 16 (чтоб не осталось необсчитанных хвостов), 
        /// посчитанной на файле, содержащем 131 МБ байт 010100101 с размером файла в битах, 
        /// поделенном на 2 (в каждом байте половина бит единичные)
        /// </summary>
        [TestMethod]
        public void CalculateOneInMarkTableTest()
        {
            MarkTable fq = new MarkTable(16);
            fq.Calculate(DataFiles.File01010101_131MB);
            Assert.AreEqual(OnesCalculator.Calculate(fq), (new FileInfo(DataFiles.File01010101_131MB).Length * Tools.BITS_IN_BYTE) / 2);
        }

        /// <summary>
        /// Тест на исключение при вычислении количества единичных бит в маркировочной таблице с разными смещением и размерностью:
        /// 1. Проверяется возникновение исключения при попытке посчитать количество единичных бит на маркировочной таблице с разными значениями смещения и размерности
        /// </summary>
        [TestMethod]
        public void CalculateOneInMarkTableWithDifferentShiftAndDimTest()
        {
            MarkTable fq = new MarkTable(16, 3);
            fq.Calculate(DataFiles.File11110000_5MB);
            Assert.ThrowsException<BitFrequencyCalculatorException>(() => OnesCalculator.Calculate(fq));
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
            Assert.AreEqual(OnesCalculator.Calculate(data), 37);
            Assert.AreEqual(OnesCalculator.Calculate(data, 30), 11);
        }

        /// <summary>
        /// Тест на вычисление количества единичных бит в файле:
        /// 1. Вычисление количества единичных бит в файле размером 131 МБ из байт 01010101.
        /// Сравниваются полученное значение умноженное на 2 с размером файла в битах
        /// </summary>
        [TestMethod]
        public void CalculateOneFile01010101_131MBTest()
        {
            long nmOnes = OnesCalculator.Calculate(DataFiles.File01010101_131MB);
            
            Assert.AreEqual(nmOnes * 2, new FileInfo(DataFiles.File01010101_131MB).Length * Tools.BITS_IN_BYTE);
        }

        /// <summary>
        /// Тест на вычисление количества единичных бит в <see cref="long"/>:
        /// 1. Вычисление количества единичных бит в числах типа <see cref="long"/>.
        /// Сравниваются полученные значение со значениями, посчитанными вручную
        /// </summary>
        [TestMethod]
        public void CalculateOneLongDataTest()
        {
            long data = 345;
            Assert.AreEqual(OnesCalculator.Calculate(data), 5);

            data = 45_688_764;
            Assert.AreEqual(OnesCalculator.Calculate(data), 15);

            data = 864_534_354_384;
            Assert.AreEqual(OnesCalculator.Calculate(data), 17);
        }
    }
}
