namespace MihStatLibrary
{
    /// <summary>
    /// Класс для получения данных блоками
    /// </summary>
    public class BlockData
    {
        private byte[]? _data;
        private FileStream? _fStream;
        private long _fullDataLength;

        /// <summary>
        /// Массив, хранящий полученный блок данных
        /// </summary>
        public byte[]? Data { get { return _data; } }

        /// <summary>
        /// Полное количество данных в байтах 
        /// </summary>
        public long FullDataLength { get { return _fullDataLength; } }

        /// <summary>
        /// Создает экземпляр класса для получения блоков данных из файла по имени
        /// </summary>
        /// <param name="fileName">Полное имя файла</param>
        public BlockData(string fileName)
        {
            _data = null;
            _fStream = new FileStream(fileName, FileMode.Open);
            _fullDataLength = _fStream.Length;
        }

        /// <summary>
        /// Создает экземпляр класса, заполненный случайной последовательностью. Случайная 
        /// последовательность получается путем XOR-а определенного количества случайных значений.
        /// </summary>
        /// <param name="szData">Размер случайной последовательности</param>
        /// <param name="nmXor">Количество XOR-ов</param>
        public BlockData(int szData, int nmXor)
        {
            _fullDataLength = szData;

            _data = new byte[_fullDataLength];
            var rnd = new Random();
            byte buffer = 0;
            for (int i = 0; i < _fullDataLength; i++)
            {
                buffer = Convert.ToByte(rnd.Next() % 256);
                for (int j = 0; j < nmXor - 1; j++)
                {
                    buffer = Convert.ToByte(buffer ^ rnd.Next(rnd.Next() % 256));
                }
                _data[i] = buffer;
            }
        }

        /// <summary>
        /// Создает экземпляр класса для получения блоков данных из файла по экземпляру <see cref="FileStream"/>
        /// </summary>
        /// <param name="fileName"></param>
        public BlockData(FileStream fileName)
        {
            _fStream = fileName;
            _fullDataLength = _fStream.Length;
        }

        /// <summary>
        /// Получение блока данных из файла
        /// </summary>
        /// <param name="szBlock">Размер блока данных</param>
        /// <exception cref="Exception"></exception>
        public void GetBlockDataFromFile(long szBlock)
        {
            long szFileRemain = (_fStream!.Length - _fStream.Position);
            if (szFileRemain == 0)
            {
                throw new Exception("Попытка считывания данных из обработанного файла!");
            }
            long correctSzBlock = szBlock < szFileRemain ? szBlock : szFileRemain;
            _data = new byte[correctSzBlock];
            _fStream.Read(_data, 0, _data.Length);
        }
    }
}
