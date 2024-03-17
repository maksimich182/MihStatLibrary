using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MihStatLibrary.Data
{
    /// <summary>
    /// Интерфейс источника получения блока данных
    /// </summary>
    public interface IBlockDataSource
    {
        /// <summary>
        /// Заполняет блок данными из источника
        /// </summary>
        /// <param name="szBlock">Размер запрашиваемого блока данных в байтах</param>
        public byte[] GetBlockData(int szBlock);
    }
}
