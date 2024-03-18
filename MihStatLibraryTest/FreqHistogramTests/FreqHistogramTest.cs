using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Resources;
using MihStatLibrary;
using MihStatLibrary.Data;
using MihStatLibrary.Histogram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MihStatLibraryTest.FreqHistogramTests
{
    /// <summary>
    /// Тесты класса FreqHistogram
    /// </summary>
    [TestClass]
    public class FreqHistogramTest
    {
        /// <summary>
        /// Тест гистограммы частот на файле заполненном байтами со значениями от 0 до 255:
        /// 1. Тестирование на размерности 8
        ///     1.1. В гистограмме все значения равны 1
        ///     1.2. В гистограмме обсчиталось 256 последовательностей
        /// 2. Тестирование на размерности 13
        ///     2.1. В гистограмме обсчиталось 157 значений
        /// </summary>
        [TestMethod]
        public void FreqHistogremCalculateFrom0To255FileTest()
        {
            int dimension = Tools.BITS_IN_BYTE;
            FreqHistogram fq = new FreqHistogram(dimension);
            fq.Calculate(DataFiles.File12345678etc256B);
            foreach (var frequence in fq.Histogram)
            {
                Assert.AreEqual(frequence, 1);
            }
            Assert.AreEqual(fq.NmVectors, 256);

            fq = new FreqHistogram(13);
            fq.Calculate(DataFiles.File12345678etc256B);
            Assert.AreEqual(fq.NmVectors, 157);
        }

        /// <summary>
        /// Тест гистограммы на файле заполненном байтами 11111111:
        /// 1. Смещение 8, размерность 8, все значения (кроме последнего) равны 0, последнее равно размеру файла в байтах
        /// </summary>
        [TestMethod]
        public void FreqHistogramCalculate11111111_131MBTest()
        {
            int dimension = Tools.BITS_IN_BYTE;
            FreqHistogram fq = new FreqHistogram(dimension);
            fq.Calculate(DataFiles.File11111111_131MB);
            for(int i = 0; i < fq.Histogram.Length - 1; i++)
            {
                Assert.AreEqual(fq.Histogram[i], 0);
            }
            Assert.AreEqual(fq.Histogram[fq.Histogram.Length - 1], new FileInfo(DataFiles.File11111111_131MB).Length);
        }

        /// <summary>
        /// Тест гистограммы на файле заполненном байтами 01010101 (Смещение 8, размерность 8):
        /// 1. Все значения (кроме 01010101) равны 0, значение 01010101 равно размеру файла в байтах
        /// </summary>
        [TestMethod]
        public void FreqHistogramCalculate01010101_131MBShift8Dim8Test()
        {
            int dimension = Tools.BITS_IN_BYTE;
            long szFile = new FileInfo(DataFiles.File01010101_131MB).Length;
            FreqHistogram fq = new FreqHistogram(dimension);
            fq.Calculate(DataFiles.File01010101_131MB);
            for (int i = 0; i < fq.Histogram.Length; i++)
            {
                if (i == 0b01010101) continue;
                Assert.AreEqual(fq.Histogram[i], 0);
            }
            Assert.AreEqual(fq.Histogram[0b01010101], szFile);
        }

        /// <summary>
        /// Тест гистограммы на файле заполненном байтами 01010101 (Смещение 3, размерность 5):
        /// 1. Количество посчитанных векторов равно округленному вниз значению деления размера файла в битах на смещение
        ///     минус округленное вниз деление размерности на смещение
        /// 2. Все значения (кроме 01010 и 10101) равны 0, значение 01010 равно половине количества посчитанных векторов, 
        ///     значение 10101 равно значению 01010
        /// </summary>
        [TestMethod]
        public void FreqHistogramCalculate01010101_131MBShift3Dim5Test()
        {
            int dimension = 5;
            int shift = 3;
            long szFile = new FileInfo(DataFiles.File01010101_131MB).Length;
            FreqHistogram fq = new FreqHistogram(dimension, shift);
            fq.Calculate(DataFiles.File01010101_131MB);
            Assert.AreEqual(fq.NmVectors, (int)(Math.Floor(((double)(szFile * Tools.BITS_IN_BYTE) / shift)
                - Math.Floor((double)dimension / shift))));
            for (int i = 0; i < fq.Histogram.Length; i++)
            {
                if (i == 0b01010 || i == 0b10101) continue;
                Assert.AreEqual(fq.Histogram[i], 0);
            }
            Assert.AreEqual(fq.Histogram[0b01010], fq.NmVectors / 2);
            Assert.AreEqual(fq.Histogram[0b10101], fq.Histogram[0b01010]);
        }

        /// <summary>
        /// Тест на генерацию исключения <see cref="InvalidShiftException"/> при некорректоном значении смещения при создании гистограммы:
        /// 1. Попытка создания гистограммы с размерностью 5 и смещением 7
        /// </summary>
        [TestMethod]
        public void FreqHistogramInvalidShiftCreateTest()
        {
            FreqHistogram fq;
            Assert.ThrowsException<InvalidShiftException>(() => fq = new FreqHistogram(5, 7));
        }

        /// <summary>
        /// Тест на генерацию исключения <see cref="ReduceException"/> при некорректоном использовании функции пересчета гистограммы:
        /// 1. Тесты над файлом с байтами 11110000 размером 1 МБ
        ///     1.1. Создание и обсчет матрицы размерностью 6 и смещением 3, попытка пересчитать на матрицу размерностью 7 и смещением 3
        ///     1.2. Создание и обсчет матрицы размерностью 6 и смещением 1, попытка пересчитать на матрицу размерностью 7 и смещением 3
        ///     1.3. Попытка пересчитать матрицу размерностью 6 и смещением 1 на матрицу размерностью 7 и смещением 1
        ///     1.4. Попытка пересчитать матрицу размерностью 6 и смещением 1 на матрицу размерностью 7 и смещением 1
        /// </summary>
        [TestMethod]
        public void FreqHistogramReduceExceptionTest()
        {
            FreqHistogram fqSource;
            FreqHistogram fqDest;

            fqSource = new FreqHistogram(6, 3);
            fqDest = new FreqHistogram(7, 3);

            fqSource.Calculate(DataFiles.File11110000_1MB);
            Assert.ThrowsException<ReduceException>(() => fqSource.Reduce(fqDest));

            fqSource = new FreqHistogram(6, 1);
            fqSource.Calculate(DataFiles.File11110000_1MB);
            Assert.ThrowsException<ReduceException>(() => fqSource.Reduce(fqDest));

            fqDest = new FreqHistogram(7, 1);
            Assert.ThrowsException<ReduceException>(() => fqSource.Reduce(fqDest));

            fqDest = new FreqHistogram(4, 2);
            Assert.ThrowsException<ReduceException>(() => fqSource.Reduce(fqDest));
        }

        /// <summary>
        /// Тест на пересчет гистограммы на гистограммы меньших размеров:
        /// 1. Тесты с файлом с байтами 11110000 размером 5 МБ. Пересчет гистограммы на размерности от 1 до 22
        ///     и сравнения с гистограммами, посчитанными на файле
        /// </summary>
        [TestMethod]
        public void FreqHistogramReduceCalculationTest()
        {
            int maxDimension = 22;

            FreqHistogram fqSource = new FreqHistogram(maxDimension, 1);
            fqSource.Calculate(DataFiles.File11110000_5MB);

            FreqHistogram fqDestReduce;
            FreqHistogram fqDestCalculation;

            for(int i = 1; i < maxDimension; i++)
            {
                fqDestReduce = new FreqHistogram(i, 1);
                fqDestCalculation = new FreqHistogram(i, 1);

                fqSource.Reduce(fqDestReduce);
                fqDestCalculation.Calculate(DataFiles.File11110000_5MB);

                Assert.IsTrue(fqDestCalculation.Equals(fqDestReduce));
            }
        }

        /// <summary>
        /// Тест вызова событий изменения прогресса и процесса:
        /// 1. Запускается рассчет гистограммы на файле. Проверяется, были ли вызваны события
        /// </summary>
        [TestMethod]
        public void FreqHistogramEventsTest()
        {
            FreqHistogram fqSource = new FreqHistogram(7);
            
            bool isProgressEventHandle = false;
            bool isProcessEventHandle = false;

            fqSource.ProgressChanged += (_, _) =>  isProgressEventHandle = true;
            fqSource.ProcessChanged += (_, _) => isProcessEventHandle = true;

            fqSource.Calculate(DataFiles.File11110000_1MB);

            Assert.IsTrue(isProgressEventHandle);
            Assert.IsTrue(isProcessEventHandle);
        }
    }
}
