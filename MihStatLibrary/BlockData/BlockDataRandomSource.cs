using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MihStatLibrary.BlockData
{
    /// <summary>
    /// Источник получения случайных данных. Случайная последовательность получается путем XOR-а определенного количества случайных значений.
    /// </summary>
    public class BlockDataRandomSource : IBlockDataSource
    {
        private readonly Random _rnd;
        private readonly int _nmXor;

        /// <summary>
        /// Конструктор, создающий элемент класса <see cref="BlockDataRandomSource"/>
        /// </summary>
        /// <param name="nmXor">Количество XOR-ов значений из <see cref="Random"/> для получения одного байта случайной последовательности</param>
        public BlockDataRandomSource(int nmXor = 0)
        {
            _rnd = new Random();
            _nmXor = nmXor;
        }

        /// <summary>
        /// Создает массив случайных байт, полученных путем XOR-а определенного количества байт из <see cref="Random"/>
        /// </summary>
        /// <param name="szBlock">Размер возвращаемого блока данных</param>
        /// <returns>Блок данных</returns>
        public byte[] GetBlockData(int szBlock)
        {
            byte[] dataBlock = new byte[szBlock];
            byte buffer = 0;
            for (int i = 0; i < szBlock; i++)
            {
                buffer = Convert.ToByte(_rnd.Next() % 256);
                for (int j = 0; j < _nmXor - 1; j++)
                {
                    buffer = Convert.ToByte(buffer ^ (_rnd.Next() % 256));
                }
                dataBlock[i] = buffer;
            }

            return dataBlock;
        }
    }
}
