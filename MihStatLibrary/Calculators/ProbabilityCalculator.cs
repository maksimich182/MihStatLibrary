using MihStatLibrary.Data;
using MihStatLibrary.Histogram;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MihStatLibrary.Calculators
{
    /// <summary>
    /// Класс Вычислителя вероятностей бит 1 и 0 в данных
    /// </summary>
    public class ProbabilityCalculator
    {
        private double _probabilityOne;
        private double _probabilityZero;

        /// <summary>
        /// Вероятность бита 1 в данных
        /// </summary>
        public double ProbabilityOne { get { return _probabilityOne; } }

        /// <summary>
        /// Вероятность бита 0 в данных
        /// </summary>
        public double ProbabilityZero { get { return _probabilityZero; } }

        /// <summary>
        /// Конструктор класса вычислителя вероятностей бит 1 и 0 в данных
        /// </summary>
        public ProbabilityCalculator()
        {
            _probabilityOne = 0;
            _probabilityZero = 0;
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
        /// Ассинхронная функция рассчета вероятностей 1 и 0 в файле
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
        /// Функция рассчета вероятностей 1 и 0 в файле
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
        /// Функция рассчета вероятностей 1 и 0 по гистограмме частот. СМЕЩЕНИЕ ДОЛЖНО БЫТЬ РАВНО РАЗМЕРНОСТИ! 
        /// ЕСЛИ РАЗМЕР ФАЙЛА В БИТАХ НЕ КРАТЕН РАЗМЕРНОСТИ ГИСТОГРАММЫ ЧАСТОТ, БИТЫ, НЕ ОБСЧИТАННЫЕ ГИСТОГРАММОЙ, НЕ УЧИТЫВАЮТСЯ!
        /// </summary>
        /// <param name="freqHistogram">Гистограмма частот. СМЕЩЕНИЕ ДОЛЖНО БЫТЬ РАВНО РАЗМЕРНОСТИ!</param>
        /// <exception cref="ProbabilityCalculatorException">Попытка посчитать количество единиц на гистограмме
        /// с разным значением размерности и смещения</exception>
        public void Calculate(FreqHistogram freqHistogram)
        {
            if (freqHistogram.SzShift != freqHistogram.Dimension)
                throw new ProbabilityCalculatorException("Количество единиц рассчитывается только при совпадении размерности и смещения!");

            long onesCounter = 0;
            int maxValueForNmOnesInByte = (Tools.ArNumberOne.Length <= freqHistogram.Histogram.Length)
                ? Tools.ArNumberOne.Length : freqHistogram.Histogram.Length;

            for(int i = 0; i < maxValueForNmOnesInByte; i++)
            {
                onesCounter += freqHistogram.Histogram[i] * Tools.ArNumberOne[i];
            }

            for(int i = maxValueForNmOnesInByte; i < freqHistogram.Histogram.Length; i++)
            {
                int value = i;
                int onesInValue = 0;
                while(value != 0)
                {
                    onesInValue += (value & 0x1) == 1 ? 1 : 0;
                    value >>= 1;
                }
                onesCounter += freqHistogram.Histogram[i] * onesInValue;
            }
            
            _probabilityOne += (double)onesCounter / (freqHistogram.NmVectors * freqHistogram.Dimension);
            _probabilityZero = 1 - _probabilityOne;
        }

        /// <summary>
        /// Функция рассчета вероятностей 1 и 0 на блоке данных
        /// </summary>
        /// <param name="blockData">Блок данных</param>
        public void Calculate(BlockData blockData)
        {
            long count = 0;
            foreach (var element in blockData.Data)
            {
                count += Tools.ArNumberOne[element];
            }
            _probabilityOne = (double)count / (blockData.SzBlockData * Tools.BITS_IN_BYTE);
            _probabilityZero = 1 - _probabilityOne;
        }
    }
}
