namespace MihStatLibrary.Data
{
    /// <summary>
    /// Класс для получения данных блоками
    /// </summary>
    public class BlockData : ICloneable
    {
        private byte[]? _data;
        private int _szBLockData;
        private readonly IBlockDataSource _blockDataSource;

        /// <summary>
        /// Размер данных в блоке
        /// </summary>
        public int SzBlockData { get { return _szBLockData; } }

        /// <summary>
        /// Массив, хранящий полученный блок данных
        /// </summary>
        public byte[]? Data { get { return _data; } }

        /// <summary>
        /// Создавет экземпляр класса
        /// </summary>
        /// <param name="blockDataSource">Источник данных</param>
        public BlockData(IBlockDataSource blockDataSource)
        {
            _blockDataSource = blockDataSource;
        }

        /// <summary>
        /// Заполняет блок данными из источника
        /// </summary>
        /// <param name="szBlock">Размер запрашиваемого количества данных</param>
        public void GetBlockData(int szBlock)
        {
            _data = _blockDataSource.GetBlockData(szBlock);
            _szBLockData = _data.Length;
        }

        /// <summary>
        /// Функция клонирования блока данных
        /// </summary>
        /// <returns>Клон блока данных</returns>
        public object Clone()
        {
            BlockData newBlockData = new BlockData(_blockDataSource);
            newBlockData._szBLockData = this._szBLockData;
            newBlockData._data = new byte[newBlockData._szBLockData];
            for (int i = 0; i < _szBLockData; i++)
            {
                newBlockData._data[i] = this._data[i];
            }
            return newBlockData;
        }
    }
}
