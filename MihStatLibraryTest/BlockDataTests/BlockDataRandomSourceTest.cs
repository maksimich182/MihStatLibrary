using MihStatLibrary.Data;
using MihStatLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MihStatLibraryTest
{
    /// <summary>
    /// Тесты класса BlockDataRandomSource
    /// </summary>
    [TestClass]
    public class BlockDataRandomSourceTest
    {
        /// <summary>
        /// Тест получения блока данных:
        /// 1. Размер полученных данных совпадает с ожидаемым
        /// 2. TO DO (после реализации гистограммы частот): Полученные данные случайны
        /// </summary>
        [TestMethod]
        public void TestGetBlockDataRandom()
        {
            BlockDataRandomSource blockData = new BlockDataRandomSource();
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
    }
}
