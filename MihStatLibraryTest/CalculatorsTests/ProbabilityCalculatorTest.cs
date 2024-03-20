using MihStatLibrary;
using MihStatLibrary.Calculators;
using MihStatLibrary.Data;
using MihStatLibrary.Histogram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MihStatLibraryTest.CalculatorsTests
{
    /// <summary>
    /// Тесты класса ProbabilityCalculator
    /// </summary>
    [TestClass]
    public class ProbabilityCalculatorTest
    {
        /// <summary>
        /// Тест метода Calculate на блоке данных:
        /// 1. Генерируется блок данных размером 100.000.000 байт случайной последовательности с XOR-ом 3. Расчитывается вероятность знаков.
        /// На том же блоке рассчитывается вероятность бита 1 с использованием OnesCalculator и делится на количество бит в блоке. 
        /// Сравниваются полученные значения
        /// </summary>
        [TestMethod]
        public void ProbabilityCalculateBlockTest()
        {
            BlockData data = new BlockData(new BlockDataRandomSource(3));
            data.GetBlockData(Tools.SIZE_BLOCK_BYTES);
            ProbabilityCalculator calculator = new ProbabilityCalculator();
            calculator.Calculate(data);

            long nmOnes = OnesCalculator.Calculate(data);
            double probOne = (double)nmOnes / (data.SzBlockData * Tools.BITS_IN_BYTE);
            Assert.AreEqual(calculator.ProbabilityOne, probOne);

        }

        /// <summary>
        /// Тест ассинхронного метода CalculateAsync на файле:
        /// 1. Методами CalculateAync и Calculate на файле размером 131 МБ из байт 01010101 рассчитываются вероятности.
        /// Сравниваются полученные значения
        /// </summary>
        [TestMethod]
        public async Task ProbabilityAsyncCalculateFile01010101_131MBTest()
        {
            ProbabilityCalculator calculator = new ProbabilityCalculator();
            await calculator.CalculateAsync(DataFiles.File01010101_131MB);
            long nmOnes = OnesCalculator.Calculate(DataFiles.File01010101_131MB);
            double probOne = (double)nmOnes / (new FileInfo(DataFiles.File01010101_131MB).Length * Tools.BITS_IN_BYTE);
            Assert.AreEqual(calculator.ProbabilityOne, probOne);
        }

        /// <summary>
        /// Тест метода Calculate на файле:
        /// 1. Методом Calculate на файле размером 131 МБ из байт 01010101 рассчитываются вероятности.
        /// Проверяется, что вероятность 1 = 0.5, а вероятность 0 равна вероятности 1.
        /// </summary>
        [TestMethod]
        public void ProbabilityCalculateFile01010101_131MBTest()
        {
            ProbabilityCalculator calculator = new ProbabilityCalculator();
            calculator.Calculate(DataFiles.File01010101_131MB);
            Assert.AreEqual(calculator.ProbabilityOne, 0.5);
            Assert.AreEqual(calculator.ProbabilityOne, calculator.ProbabilityZero);
        }

        /// <summary>
        /// Тест метода Calculate на блоке данных:
        /// 1. Методом Calculate блоке данных размером 100.000.000 байт из файла размером 131 МБ из байт 01010101 рассчитываются вероятности.
        /// Проверяется, что вероятность 1 = 0.5, а вероятность 0 равна вероятности 1.
        /// </summary>
        [TestMethod]
        public void ProbabilityCalculateBlockDataTest()
        {
            FileStream fs = new FileStream(DataFiles.File01010101_131MB, FileMode.Open);
            BlockData data = new BlockData(new BlockDataFileSource(fs));
            ProbabilityCalculator calculator = new ProbabilityCalculator();

            data.GetBlockData(Tools.SIZE_BLOCK_BYTES);
            fs.Close();

            calculator.Calculate(data);
            Assert.AreEqual(calculator.ProbabilityOne, 0.5);
            Assert.AreEqual(calculator.ProbabilityOne, calculator.ProbabilityZero);
        }

        /// <summary>
        /// Тест метода Calculate на гистограмме частот:
        /// 1. Рассчитывается гистограмма частот размерностью 16 (кратно размеру файла в битах) и смещением 16
        /// файла размером 131 МБ из байт 01010101 рассчитываются вероятности.
        /// Проверяется, что вероятность 1 = 0.5, а вероятность 0 равна вероятности 1.
        /// </summary>
        [TestMethod]
        public void ProbabilityCalculateFreqHistogramTest()
        {
            FreqHistogram data = new FreqHistogram(16);
            data.Calculate(DataFiles.File01010101_131MB);

            ProbabilityCalculator calculator = new ProbabilityCalculator();
            calculator.Calculate(data);

            Assert.AreEqual(calculator.ProbabilityOne, 0.5);
            Assert.AreEqual(calculator.ProbabilityOne, calculator.ProbabilityZero);
        }

        /// <summary>
        /// Тест вызова исключения <see cref="ProbabilityCalculatorException"/> из метода Calculate на гистограмме частот при разных значениях смещения и размерности:
        /// 1. Рассчитывается гистограмма частот размерностью 16 (кратно размеру файла в битах) и смещением 15. 
        /// Проверяется создание исключения.
        /// </summary>
        [TestMethod]
        public void ProbabilityCalculateFreqHistogramExceptionTest()
        {
            FreqHistogram data = new FreqHistogram(16, 15);
            data.Calculate(DataFiles.File01010101_131MB);

            ProbabilityCalculator calculator = new ProbabilityCalculator();

            Assert.ThrowsException<ProbabilityCalculatorException>(()=>calculator.Calculate(data));
        }
    }
}
