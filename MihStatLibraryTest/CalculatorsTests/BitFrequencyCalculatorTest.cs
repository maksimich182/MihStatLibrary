using MihStatLibrary;
using MihStatLibrary.Calculators;
using MihStatLibrary.Data;
using MihStatLibrary.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MihStatLibraryTest.CalculatorsTests
{
    /// <summary>
    /// Тесты класса BitFrequencyCalculator
    /// </summary>
    [TestClass]
    public class BitFrequencyCalculatorTest
    {
        /// <summary>
        /// Тест метода Calculate на блоке данных:
        /// 1. Генерируется блок данных размером 100.000.000 байт случайной последовательности с XOR-ом 3. Расчитывается оценка вероятности бит.
        /// На том же блоке рассчитывается количество бит 1 с использованием OnesCalculator и делится на количество бит в блоке. 
        /// Сравниваются полученные значения
        /// </summary>
        [TestMethod]
        public void BitFrequencyCalculateBlockTest()
        {
            BlockData data = new BlockData(new BlockDataRandomSource(3));
            data.GetBlockData(Tools.SIZE_BLOCK_BYTES);
            BitFrequencyCalculator calculator = new BitFrequencyCalculator();
            calculator.Calculate(data);

            long nmOnes = OnesCalculator.Calculate(data);
            double probOne = (double)nmOnes / (data.SzBlockData * Tools.BITS_IN_BYTE);
            Assert.AreEqual(calculator.FrequencyOne, probOne);

        }

        /// <summary>
        /// Тест ассинхронного метода CalculateAsync на файле:
        /// 1. Методами CalculateAync и Calculate на файле размером 131 МБ из байт 01010101 рассчитываются оценки вероятностей.
        /// Сравниваются полученные значения
        /// </summary>
        [TestMethod]
        public async Task BitFrequencyAsyncCalculateFile01010101_131MBTest()
        {
            BitFrequencyCalculator calculatorAsync = new BitFrequencyCalculator();
            await calculatorAsync.CalculateAsync(DataFiles.File01010101_131MB);
            BitFrequencyCalculator calculatorSync = new BitFrequencyCalculator();
            calculatorSync.Calculate(DataFiles.File01010101_131MB);
            Assert.AreEqual(calculatorAsync.FrequencyOne, calculatorSync.FrequencyOne);
        }

        /// <summary>
        /// Тест метода Calculate на файле:
        /// 1. Методом Calculate на файле размером 131 МБ из байт 01010101 рассчитываются оценки вероятностей.
        /// Проверяется, что оценка вероятности 1 = 0.5, а оценка вероятности 0 равна оценке вероятности 1.
        /// </summary>
        [TestMethod]
        public void BitFrequencyCalculateFile01010101_131MBTest()
        {
            BitFrequencyCalculator calculator = new BitFrequencyCalculator();
            calculator.Calculate(DataFiles.File01010101_131MB);
            Assert.AreEqual(calculator.FrequencyOne, 0.5);
            Assert.AreEqual(calculator.FrequencyOne, calculator.FrequencyZero);
        }

        /// <summary>
        /// Тест метода Calculate на блоке данных:
        /// 1. Методом Calculate блоке данных размером 100.000.000 байт из файла размером 131 МБ из байт 01010101 рассчитываются оценки вероятностей.
        /// Проверяется, что оценка вероятности 1 = 0.5, а оценка вероятности 0 равна оценке вероятности 1.
        /// </summary>
        [TestMethod]
        public void BitFrequencyCalculateBlockDataTest()
        {
            FileStream fs = new FileStream(DataFiles.File01010101_131MB, FileMode.Open);
            BlockData data = new BlockData(new BlockDataFileSource(fs));
            BitFrequencyCalculator calculator = new BitFrequencyCalculator();

            data.GetBlockData(Tools.SIZE_BLOCK_BYTES);
            fs.Close();

            calculator.Calculate(data);
            Assert.AreEqual(calculator.FrequencyOne, 0.5);
            Assert.AreEqual(calculator.FrequencyOne, calculator.FrequencyZero);
        }

        /// <summary>
        /// Тест метода Calculate на маркировочной таблице:
        /// 1. Рассчитывается маркировочная таблица размерностью 16 (кратно размеру файла в битах) и смещением 16
        /// файла размером 131 МБ из байт 01010101 рассчитываются оценки вероятностей.
        /// Проверяется, что оценка вероятности 1 = 0.5, а оценка вероятности 0 равна оценке вероятности 1.
        /// </summary>
        [TestMethod]
        public void BitFrequencyCalculateMarkTableTest()
        {
            MarkTable data = new MarkTable(16);
            data.Calculate(DataFiles.File01010101_131MB);

            BitFrequencyCalculator calculator = new BitFrequencyCalculator();
            calculator.Calculate(data);

            Assert.AreEqual(calculator.FrequencyOne, 0.5);
            Assert.AreEqual(calculator.FrequencyOne, calculator.FrequencyZero);
        }

        /// <summary>
        /// Тест вызова исключения <see cref="BitFrequencyCalculatorException"/> из метода Calculate на маркировочной таблице при разных значениях смещения и размерности:
        /// 1. Рассчитывается маркировочная таблица размерностью 16 (кратно размеру файла в битах) и смещением 15. 
        /// Проверяется создание исключения.
        /// </summary>
        [TestMethod]
        public void BitFrequencyCalculateMarkTableExceptionTest()
        {
            MarkTable data = new MarkTable(16, 15);
            data.Calculate(DataFiles.File01010101_131MB);

            BitFrequencyCalculator calculator = new BitFrequencyCalculator();

            Assert.ThrowsException<BitFrequencyCalculatorException>(()=>calculator.Calculate(data));
        }
    }
}
