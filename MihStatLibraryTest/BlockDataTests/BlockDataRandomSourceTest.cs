using MihStatLibrary.Data;
using MihStatLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MihStatLibrary.Calculators;

namespace MihStatLibraryTest
{
    /// <summary>
    /// Тесты класса BlockDataRandomSource
    /// </summary>
    [TestClass]
    public class BlockDataRandomSourceTest
    {
        /// <summary>
        /// Тест получения случайного блока данных по размеру:
        /// 1. Размер полученных данных совпадает с ожидаемым
        /// </summary>
        [TestMethod]
        public void TestGetBlockDataRandom()
        {
            BlockDataRandomSource blockData = new BlockDataRandomSource();
            BitFrequencyCalculator probabilityCalculator = new BitFrequencyCalculator();
            byte[] data;

            int szBlockData = 40;
            data = blockData.GetBlockData(szBlockData);
            Assert.AreEqual(data.Length, szBlockData);

            szBlockData = 4650;
            data = blockData.GetBlockData(szBlockData);
            Assert.AreEqual(data.Length, szBlockData);

            szBlockData = 8866652;
            data = blockData.GetBlockData(szBlockData);
            Assert.AreEqual(data.Length, szBlockData);
        }

        /// <summary>
        /// Тест получения случайного блока данных по вероятности:
        /// 1. Генерируются случайные блоки данных разного размера с XOR-ом 3. На полученных данных рассчитываются вероятности знаков 0 и 1.
        /// Проверяется, что полученные вероятности меньше 0.6
        /// </summary>
        [TestMethod]
        public void TestGetBlockDataRandomProbability()
        {
            BlockData blockData = new BlockData(new BlockDataRandomSource(3));
            BitFrequencyCalculator probabilityCalculator = new BitFrequencyCalculator();

            int szBlockData = 40;
            blockData.GetBlockData(szBlockData);
            probabilityCalculator.Calculate(blockData);
            Assert.IsTrue(probabilityCalculator.FrequencyOne < 0.6);
            Assert.IsTrue(probabilityCalculator.FrequencyZero < 0.6);
            probabilityCalculator = new BitFrequencyCalculator();

            szBlockData = 4650;
            blockData.GetBlockData(szBlockData);
            probabilityCalculator.Calculate(blockData);
            Assert.IsTrue(probabilityCalculator.FrequencyOne < 0.6);
            Assert.IsTrue(probabilityCalculator.FrequencyZero < 0.6);
            probabilityCalculator = new BitFrequencyCalculator();

            szBlockData = 8866652;
            blockData.GetBlockData(szBlockData);
            probabilityCalculator.Calculate(blockData);
            Assert.IsTrue(probabilityCalculator.FrequencyOne < 0.6);
            Assert.IsTrue(probabilityCalculator.FrequencyZero < 0.6);
        }
    }
}
