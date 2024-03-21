using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Resources;
using MihStatLibrary;
using MihStatLibrary.Data;
using MihStatLibrary.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MihStatLibraryTest.MarkTableTests
{
    /// <summary>
    /// Тесты класса MarkTable
    /// </summary>
    [TestClass]
    public class MarkTableTest
    {
        /// <summary>
        /// Тест маркировочной таблицы на файле заполненном байтами со значениями от 0 до 255:
        /// 1. Тестирование на размерности 8
        ///     1.1. В таблице все значения равны 1
        ///     1.2. В таблице обсчиталось 256 последовательностей
        /// 2. Тестирование на размерности 13
        ///     2.1. В таблице обсчиталось 157 значений
        /// </summary>
        [TestMethod]
        public void MarkTableCalculateFrom0To255FileTest()
        {
            int dimension = Tools.BITS_IN_BYTE;
            MarkTable markTable = new MarkTable(dimension);
            markTable.Calculate(DataFiles.File12345678etc256B);
            foreach (var val in markTable.Table)
            {
                Assert.AreEqual(val, 1);
            }
            Assert.AreEqual(markTable.NmVectors, 256);

            markTable = new MarkTable(13);
            markTable.Calculate(DataFiles.File12345678etc256B);
            Assert.AreEqual(markTable.NmVectors, 157);
        }

        /// <summary>
        /// Тест маркировочной таблицы на файле заполненном байтами 11111111 размером 131 МБ:
        /// 1. Смещение 8, размерность 8, все значения (кроме последнего) равны 0, последнее равно размеру файла в байтах
        /// </summary>
        [TestMethod]
        public void MarkTableCalculate11111111_131MBTest()
        {
            int dimension = Tools.BITS_IN_BYTE;
            MarkTable markTable = new MarkTable(dimension);
            markTable.Calculate(DataFiles.File11111111_131MB);
            for(int i = 0; i < markTable.Table.Length - 1; i++)
            {
                Assert.AreEqual(markTable.Table[i], 0);
            }
            Assert.AreEqual(markTable.Table[markTable.Table.Length - 1], new FileInfo(DataFiles.File11111111_131MB).Length);
        }

        /// <summary>
        /// Тест маркировочной таблицы на файле заполненном байтами 01010101 размером 131 МБ (Смещение 8, размерность 8):
        /// 1. Все значения (кроме 01010101) равны 0, значение 01010101 равно размеру файла в байтах
        /// </summary>
        [TestMethod]
        public void MarkTableCalculate01010101_131MBShift8Dim8Test()
        {
            int dimension = Tools.BITS_IN_BYTE;
            long szFile = new FileInfo(DataFiles.File01010101_131MB).Length;
            MarkTable markTable = new MarkTable(dimension);
            markTable.Calculate(DataFiles.File01010101_131MB);
            for (int i = 0; i < markTable.Table.Length; i++)
            {
                if (i == 0b01010101) continue;
                Assert.AreEqual(markTable.Table[i], 0);
            }
            Assert.AreEqual(markTable.Table[0b01010101], szFile);
        }

        /// <summary>
        /// Тест маркировочной таблицы на файле заполненном байтами 01010101 (Смещение 3, размерность 5):
        /// 1. Количество посчитанных векторов равно округленному вниз значению деления размера файла в битах на смещение
        ///     минус округленное вниз деление размерности на смещение
        /// 2. Все значения (кроме 01010 и 10101) равны 0, значение 01010 равно половине количества посчитанных векторов, 
        ///     значение 10101 равно значению 01010
        /// </summary>
        [TestMethod]
        public void MarkTableCalculate01010101_131MBShift3Dim5Test()
        {
            int dimension = 5;
            int shift = 3;
            long szFile = new FileInfo(DataFiles.File01010101_131MB).Length;
            MarkTable markTable = new MarkTable(dimension, shift);
            markTable.Calculate(DataFiles.File01010101_131MB);
            Assert.AreEqual(markTable.NmVectors, (int)(Math.Floor(((double)(szFile * Tools.BITS_IN_BYTE) / shift)
                - Math.Floor((double)dimension / shift))));
            for (int i = 0; i < markTable.Table.Length; i++)
            {
                if (i == 0b01010 || i == 0b10101) continue;
                Assert.AreEqual(markTable.Table[i], 0);
            }
            Assert.AreEqual(markTable.Table[0b01010], markTable.NmVectors / 2);
            Assert.AreEqual(markTable.Table[0b10101], markTable.Table[0b01010]);
        }

        /// <summary>
        /// Тест на генерацию исключения <see cref="InvalidShiftException"/> при некорректоном значении смещения при создании маркировочной таблицы:
        /// 1. Попытка создания таблицы с размерностью 5 и смещением 7
        /// </summary>
        [TestMethod]
        public void MarkTableInvalidShiftCreateTest()
        {
            MarkTable markTable;
            Assert.ThrowsException<InvalidShiftException>(() => markTable = new MarkTable(5, 7));
        }

        /// <summary>
        /// Тест на генерацию исключения <see cref="ReduceException"/> при некорректоном использовании функции пересчета маркировочной таблицы:
        /// 1. Тесты над файлом с байтами 11110000 размером 1 МБ
        ///     1.1. Создание и обсчет таблицы размерностью 6 и смещением 3, попытка пересчитать на таблицы размерностью 7 и смещением 3
        ///     1.2. Создание и обсчет таблицы размерностью 6 и смещением 1, попытка пересчитать на таблицы размерностью 7 и смещением 3
        ///     1.3. Попытка пересчитать таблицы размерностью 6 и смещением 1 на таблицы размерностью 7 и смещением 1
        ///     1.4. Попытка пересчитать таблицы размерностью 6 и смещением 1 на таблицы размерностью 7 и смещением 1
        /// </summary>
        [TestMethod]
        public void MarkTableReduceExceptionTest()
        {
            MarkTable mtSource;
            MarkTable mtDest;

            mtSource = new MarkTable(6, 3);
            mtDest = new MarkTable(7, 3);

            mtSource.Calculate(DataFiles.File11110000_1MB);
            Assert.ThrowsException<ReduceException>(() => mtSource.Reduce(mtDest));

            mtSource = new MarkTable(6, 1);
            mtSource.Calculate(DataFiles.File11110000_1MB);
            Assert.ThrowsException<ReduceException>(() => mtSource.Reduce(mtDest));

            mtDest = new MarkTable(7, 1);
            Assert.ThrowsException<ReduceException>(() => mtSource.Reduce(mtDest));

            mtDest = new MarkTable(4, 2);
            Assert.ThrowsException<ReduceException>(() => mtSource.Reduce(mtDest));
        }

        /// <summary>
        /// Тест на пересчет маркировочной таблицы на маркировочную таблицу меньших размеров:
        /// 1. Тесты с файлом с байтами 11110000 размером 5 МБ. Пересчет таблицы на размерности от 1 до 22
        ///     и сравнения с таблицами, посчитанными на файле
        /// </summary>
        [TestMethod]
        public void MarkTableReduceCalculationTest()
        {
            int maxDimension = 22;

            MarkTable mtSource = new MarkTable(maxDimension, 1);
            mtSource.Calculate(DataFiles.File11110000_5MB);

            MarkTable mtDestReduce;
            MarkTable mtDestCalculation;

            for(int i = 1; i < maxDimension; i++)
            {
                mtDestReduce = new MarkTable(i, 1);
                mtDestCalculation = new MarkTable(i, 1);

                mtSource.Reduce(mtDestReduce);
                mtDestCalculation.Calculate(DataFiles.File11110000_5MB);

                Assert.IsTrue(mtDestCalculation.Equals(mtDestReduce));
            }
        }

        /// <summary>
        /// Тест вызова событий изменения прогресса и процесса:
        /// 1. Запускается рассчет маркировочной таблицы на файле. Проверяется, были ли вызваны события
        /// </summary>
        [TestMethod]
        public void MarkTableEventsTest()
        {
            MarkTable markTable = new MarkTable(7);
            
            bool isProgressEventHandle = false;
            bool isProcessEventHandle = false;

            markTable.ProgressChanged += (_, _) =>  isProgressEventHandle = true;
            markTable.ProcessChanged += (_, _) => isProcessEventHandle = true;

            markTable.Calculate(DataFiles.File11110000_1MB);

            Assert.IsTrue(isProgressEventHandle);
            Assert.IsTrue(isProcessEventHandle);
        }
    }
}
