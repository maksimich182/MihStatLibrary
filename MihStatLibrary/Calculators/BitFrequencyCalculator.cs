using MihStatLibrary.Data;
using MihStatLibrary.Tables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MihStatLibrary.Calculators
{
    /// <summary>
    /// Класс вычислителя оценки вероятностей бит 1 и 0 в данных
    /// </summary>
    public class BitFrequencyCalculator
    {
        private double _frequencyOne;
        private double _frequencyZero;

        /// <summary>
        /// Оценка вероятности бита 1 в данных
        /// </summary>
        public double FrequencyOne { get { return _frequencyOne; } }

        /// <summary>
        /// Оценка вероятности 0 в данных
        /// </summary>
        public double FrequencyZero { get { return _frequencyZero; } }

        /// <summary>
        /// Конструктор класса вычислителя вероятностей бит 1 и 0 в данных
        /// </summary>
        public BitFrequencyCalculator()
        {
            _frequencyOne = 0;
            _frequencyZero = 0;
        }

        ///// <summary>
        ///// Многопоточная функция рассчета вероятностей 1 и 0 в файле
        ///// </summary>
        ///// <param name="fileName">Имя файла</param>
        //public async void CalculateAsync(string fileName)
        //{
        //    FileStream fsData = new FileStream(fileName, FileMode.Open);

        //    List<Thread> lstThreads = new List<Thread>();
        //    double iNmBlocks = Math.Ceiling((double)fsData.Length / Tools.SIZE_BLOCK_BYTES);
        //    for (int i = 0; i < iNmBlocks; i++)
        //    {
        //        Thread thCalcBlock = new Thread(() => Calculate(fsData, Tools.SIZE_BLOCK_BYTES));
        //        thCalcBlock.Name = "thr" + i.ToString();
        //        lstThreads.Add(thCalcBlock);
        //        thCalcBlock.Start();
        //    }

        //    foreach (var element in lstThreads)
        //    {
        //        element.Join();
        //    }

        //    fsData.Close();
        //}

        /// <summary>
        /// Ассинхронная функция рассчета оценки вероятностей 1 и 0 в файле
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        public async Task CalculateAsync(string fileName)
        {
            FileStream fsData = new FileStream(fileName, FileMode.Open);
            

            List<Task> tasks = new List<Task>();
            double iNmBlocks = Math.Ceiling((double)fsData.Length / Tools.SIZE_BLOCK_BYTES);
            for (int i = 0; i < iNmBlocks; i++)
            {
                BlockData blockData = new BlockData(new BlockDataFileSource(fsData));
                blockData.GetBlockData(Tools.SIZE_BLOCK_BYTES);
                Task calculateBlock = new Task(() => Calculate(blockData));
                tasks.Add(calculateBlock);
                calculateBlock.Start();
            }

            foreach (var element in tasks)
            {
                await element;
            }

            fsData.Close();
        }

        /// <summary>
        /// Функция рассчета оценки вероятностей 1 и 0 в файле
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        public void Calculate(string fileName)
        {
            FileStream fsData = new FileStream(fileName, FileMode.Open);
            BlockData blockData = new BlockData(new BlockDataFileSource(fsData));
            double iNmBlocks = Math.Ceiling((double)fsData.Length / Tools.SIZE_BLOCK_BYTES);
            for (int i = 0; i < iNmBlocks; i++)
            {
                blockData.GetBlockData(Tools.SIZE_BLOCK_BYTES);
                Calculate(blockData);
            }

            fsData.Close();
        }

        /// <summary>
        /// Функция рассчета оценки вероятностей 1 и 0 по маркировочной таблице. СМЕЩЕНИЕ ДОЛЖНО БЫТЬ РАВНО РАЗМЕРНОСТИ! 
        /// ЕСЛИ РАЗМЕР ФАЙЛА В БИТАХ НЕ КРАТЕН РАЗМЕРНОСТИ МАРКИРОВОЧНОЙ ТАБЛИЦЫ, БИТЫ, НЕ ОБСЧИТАННЫЕ ТАБЛИЦЕЙ, НЕ УЧИТЫВАЮТСЯ!
        /// </summary>
        /// <param name="markTable">Маркировочная таблица. СМЕЩЕНИЕ ДОЛЖНО БЫТЬ РАВНО РАЗМЕРНОСТИ!</param>
        /// <exception cref="BitFrequencyCalculatorException">Попытка посчитать количество единиц на таблице
        /// с разным значением размерности и смещения</exception>
        public void Calculate(MarkTable markTable)
        {
            if (markTable.SzShift != markTable.Dimension)
                throw new BitFrequencyCalculatorException("Количество единиц рассчитывается только при совпадении размерности и смещения!");

            long onesCounter = 0;
            int maxValueForNmOnesInByte = (Tools.ArNumberOne.Length <= markTable.Table.Length)
                ? Tools.ArNumberOne.Length : markTable.Table.Length;

            for(int i = 0; i < maxValueForNmOnesInByte; i++)
            {
                onesCounter += markTable.Table[i] * Tools.ArNumberOne[i];
            }

            for(int i = maxValueForNmOnesInByte; i < markTable.Table.Length; i++)
            {
                int value = i;
                int onesInValue = 0;
                while(value != 0)
                {
                    onesInValue += (value & 0x1) == 1 ? 1 : 0;
                    value >>= 1;
                }
                onesCounter += markTable.Table[i] * onesInValue;
            }
            
            _frequencyOne += (double)onesCounter / (markTable.NmVectors * markTable.Dimension);
            _frequencyZero = 1 - _frequencyOne;
        }

        /// <summary>
        /// Функция рассчета оценки вероятностей 1 и 0 на блоке данных
        /// </summary>
        /// <param name="blockData">Блок данных</param>
        public void Calculate(BlockData blockData)
        {
            long count = 0;
            foreach (var element in blockData.Data)
            {
                count += Tools.ArNumberOne[element];
            }
            _frequencyOne = (double)count / (blockData.SzBlockData * Tools.BITS_IN_BYTE);
            _frequencyZero = 1 - _frequencyOne;
        }
    }
}
