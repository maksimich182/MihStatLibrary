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
    /// Тесты класса BlockData
    /// </summary>
    [TestClass]
    public class BlockDataTest
    {
        /// <summary>
        /// Тест получения блока данных из файла:
        /// 1. После получения данных массив данных проинициализирован
        /// 2. Полученные блоком данные совпадают с данными из файла
        /// 3. Размер полученных данных совпадает с ожидаемым
        /// 4. При попытке считать данные из обработанного файла вылетает исключение
        /// </summary>
        [TestMethod]
        public void TestGetBlockDataFromFile()
        {
            FileStream fs = new FileStream(DataFiles.FileRandom128MB, FileMode.Open);

            int indexDataFromFile = 0;
            byte[] dataFromFile = new byte[fs.Length];
            fs.Read(dataFromFile);

            fs.Seek(0, SeekOrigin.Begin);

            BlockData blockData = new BlockData(new BlockDataFileSource(fs));

            int countOfBlock = (int)Math.Ceiling((double)fs.Length / Tools.SIZE_BLOCK_BYTES);
            int expectedNmBytes = 0;
            int nmRemainBytes = (int)fs.Length;

            for (int i = 0; i < countOfBlock; i++)
            {
                expectedNmBytes = Tools.SIZE_BLOCK_BYTES <= nmRemainBytes ? Tools.SIZE_BLOCK_BYTES : nmRemainBytes;
                blockData!.GetBlockData(Tools.SIZE_BLOCK_BYTES);
                Assert.IsNotNull(blockData?.Data);
                nmRemainBytes -= blockData!.SzBlockData;
                for (int j = 0; j < blockData?.SzBlockData; j++, indexDataFromFile++)
                {
                    Assert.AreEqual(blockData?.Data[j], dataFromFile[indexDataFromFile]);
                }
                Assert.AreEqual(blockData?.SzBlockData, expectedNmBytes);
            }
            Assert.ThrowsException<Exception>(() => blockData?.GetBlockData(Tools.SIZE_BLOCK_BYTES));

            fs.Close();
        }

        /// <summary>
        /// Тест получения блока данных из рандомайзера:
        /// 1. После получения данных массив данных проинициализирован
        /// 2. Размер полученных данных совпадает с ожидаемым
        /// 3. TO DO (после реализации гистограммы частот): Полученные данные случайны
        /// </summary>
        [TestMethod]
        public void TestGetBlockDataRandom()
        {
            BlockData blockData = new BlockData(new BlockDataRandomSource());

            int szBlockData = 40;
            blockData.GetBlockData(szBlockData);
            Assert.IsNotNull(blockData.Data);
            Assert.AreEqual(blockData.SzBlockData, szBlockData);

            szBlockData = 4650;
            blockData.GetBlockData(szBlockData);
            Assert.IsNotNull(blockData.Data);
            Assert.AreEqual(blockData.SzBlockData, szBlockData);

            szBlockData = 8866652;
            blockData!.GetBlockData(szBlockData);
            Assert.IsNotNull(blockData.Data);
            Assert.AreEqual(blockData.SzBlockData, szBlockData);
        }
    }
}
