using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MihStatLibrary.BlockData
{
    /// <summary>
    /// Источник получения данных из файла
    /// </summary>
    public class BlockDataFileSource : IBlockDataSource
    {
        private FileStream? _fStream;

        /// <summary>
        /// Создает экземпляр класса для получения блоков данных из файла по экземпляру <see cref="FileStream"/>
        /// </summary>
        /// <param name="fileStream">Дескриптор файла <see cref="FileStream"/></param>
        public BlockDataFileSource(FileStream fileStream)
        {
            _fStream = fileStream;
        }

        /// <summary>
        /// Получает определенное количество данных из файла
        /// </summary>
        /// <param name="szBlock">Размер получаемого блока данных в байтах</param>
        /// <returns>Блок данных</returns>
        /// <exception cref="Exception"></exception>
        public byte[] GetBlockData(int szBlock)
        {
            long szFileRemain = _fStream!.Length - _fStream.Position;
            if (szFileRemain == 0)
            {
                throw new Exception("Попытка считывания данных из обработанного файла!");
            }
            long correctSzBlock = szBlock < szFileRemain ? szBlock : szFileRemain;
            byte[] dataBlock = new byte[correctSzBlock];
            _fStream.Read(dataBlock, 0, dataBlock.Length);
            return dataBlock;
        }
    }
}
