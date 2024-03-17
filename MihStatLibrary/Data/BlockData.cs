namespace MihStatLibrary.Data
{
    /// <summary>
    /// Класс для получения данных блоками
    /// </summary>
    public class BlockData
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
    }
}
