using Microsoft.VisualStudio.TestPlatform.Common.Utilities;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Resources;
using MihStatLibrary;
using MihStatLibrary.Data;

namespace MihStatLibraryTest
{
    /// <summary>
    /// Тесты класса BlockDataFileSource
    /// </summary>
    [TestClass]
    public class BlockDataFileSourceTest
    {
        /// <summary>
        /// Тест получения блока данных:
        /// 1. Полученные блоком данные совпадают с данными из файла
        /// 2. Размер полученных данных совпадает с ожидаемым
        /// 3. При попытке считать данные из обработанного файла вылетает исключение
        /// </summary>
        [TestMethod]
        public void TestGetBlockDataFromFile()
        {
            FileStream fs = new FileStream(RandomFiles.FileRandom128MB, FileMode.Open);

            int indexDataFromFile = 0;
            byte[] dataFromFile = new byte[fs.Length];
            fs.Read(dataFromFile);

            fs.Seek(0, SeekOrigin.Begin);

            BlockDataFileSource blockData = new BlockDataFileSource(fs);

            int countOfBlock = (int)Math.Ceiling((double)fs.Length / Tools.SIZE_BLOCK_BYTES);
            int expectedNmBytes = 0;
            int nmRemainBytes = (int)fs.Length;
            byte[] dataFromSource;

            for (int i = 0; i < countOfBlock; i++)
            {
                expectedNmBytes = Tools.SIZE_BLOCK_BYTES <= nmRemainBytes ? Tools.SIZE_BLOCK_BYTES : nmRemainBytes;
                dataFromSource = blockData.GetBlockData(Tools.SIZE_BLOCK_BYTES);
                nmRemainBytes -= dataFromSource.Length;
                for(int j = 0; j < dataFromSource.Length; j++, indexDataFromFile++)
                {
                    Assert.AreEqual(dataFromSource[j], dataFromFile[indexDataFromFile]);
                }
                Assert.AreEqual(dataFromSource.Length, expectedNmBytes);
            }
            Assert.ThrowsException<Exception>(() => blockData.GetBlockData(Tools.SIZE_BLOCK_BYTES));
            fs.Close();
        }
    }
}