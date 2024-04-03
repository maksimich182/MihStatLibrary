using MihStatLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MihStatLibrary.Tables
{
    /// <summary>
    /// Класс маркировочной таблицы
    /// </summary>
    public class MarkTable
    {
        //TODO: Сделать обработку недостаточного объема памяти

        private int _dimension;  //TODO: сделать проверку на < 64, так как памяти вряд ли хватит на такой объем таблицы
        private int _szShift;
        private long _mask;
        private long[] _table;
        private BigInteger _remain;
        private int _szRemain;
        private long _nmVectors;
        private object _locker;

        /// <summary>
        /// Событие изменения процесса обсчитывания
        /// </summary>
        public event EventHandler<string>? ProcessChanged = null;

        /// <summary>
        /// Событие изменения прогресса обсчитывания
        /// </summary>
        public event EventHandler<int>? ProgressChanged = null;

        /// <summary>
        /// Маркировочная таблица
        /// </summary>
        public long[] Table { get { return _table; } }

        /// <summary>
        /// Размерность маркировочной таблицы
        /// </summary>
        public int Dimension { get { return _dimension; } }

        /// <summary>
        /// Количество учтенных последовательностей
        /// </summary>
        public long NmVectors { get { return _nmVectors; } }

        /// <summary>
        /// Смещение маркировочной таблицы
        /// </summary>
        public int SzShift { get { return _szShift; } }


        /// <summary>
        /// Конструктор класса маркировочной таблицы без зацепления
        /// </summary>
        /// <param name="dimension">Размерность векторов маркировочной таблицы</param>
        public MarkTable(int dimension)
        {
            _dimension = dimension;
            _szShift = _dimension;
            _mask = (1 << _dimension) - 1;
            _table = new long[1 << _dimension];
            _remain = 0;
            _szRemain = 0;
            _nmVectors = 0;
            _locker = new object();
        }

        /// <summary>
        /// Конструктор класса маркировочной таблицы с зацеплением
        /// </summary>
        /// <param name="dimension">Размерность векторов маркировочной таблицы</param>
        /// <param name="szShift">Размер смещения</param>
        /// <exception cref="InvalidShiftException">Попытка задать смещение больше размерности</exception>
        public MarkTable(int dimension, int szShift) : this(dimension)
        {
            if (szShift > dimension)
                throw new InvalidShiftException("Смещение должно быть меньше размерности!");
            _szShift = szShift;
        }

        /// <summary>
        /// Подсчет маркировочной таблицы по блоку данных
        /// </summary>
        /// <param name="data">Блок данных</param>
        public void Calculate(BlockData data)
        {
            BigInteger dataBuffer = 0;
            int szBuffer = 0;

            int countReadElements = 0;

            if (_szRemain != 0)
            {
                dataBuffer = _remain;
                szBuffer = _szRemain;
            }

            while (countReadElements < data.SzBlockData)
            {
                lock (_locker)
                {
                    _getDataForCalculate(data, ref dataBuffer, ref szBuffer, ref countReadElements);
                    _calculateData(ref dataBuffer, ref szBuffer);
                }
                
                this.ProgressChanged?.Invoke(this, Tools.GetPercent(countReadElements, data.SzBlockData));
            }

            _remain = dataBuffer;
            _szRemain = szBuffer;
        }

        /// <summary>
        /// Обнуление маркировочной таблицы
        /// </summary>
        public void Clear()
        {
            Array.Clear(_table, 0, _table.Length);
            _remain = 0;
            _szRemain = 0;
            _nmVectors = 0;
        }

        /// <summary>
        /// Подсчет маркировочной таблицы по данным файла
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        /// <param name="token">Токен отмены рассчета маркировочной таблицы. При срабатывании выполнение завершается перед получением очередного болока данных</param>
        public void Calculate(string fileName, CancellationToken? token = null)
        {
            FileStream dataStream = new FileStream(fileName, FileMode.Open);
            BlockData blockData = new BlockData(new BlockDataFileSource(dataStream));

            double nmBlocks = Math.Ceiling((double)dataStream.Length / Tools.SIZE_BLOCK_BYTES);
            for (int i = 0; i < nmBlocks; i++)
            {
                if (token?.IsCancellationRequested ?? false)
                {
                    dataStream.Close();
                    return;
                }

                blockData.GetBlockData(Tools.SIZE_BLOCK_BYTES);
                Calculate(blockData);
                this.ProcessChanged?.Invoke(this, $"Маркировочная таблица: {i + 1} из {nmBlocks}");
            }
            dataStream.Close();
        }

        /// <summary>
        /// Подсчет маркировочной таблицы по данным файла ToDo
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        //public async Task CalculateAsync(string fileName, CancellationToken? token = null)
        //{
        //    FileStream dataStream = new FileStream(fileName, FileMode.Open);
        //    BlockData blockData = new BlockData(new BlockDataFileSource(dataStream));

        //    int szBlock = (int)(Math.Ceiling((double)(Tools.SIZE_BLOCK_BYTES * Tools.BITS_IN_BYTE) / (_dimension * Tools.BITS_IN_BYTE))
        //        * (_dimension * Tools.BITS_IN_BYTE)) / Tools.BITS_IN_BYTE;
        //    double nmBlocks = Math.Ceiling((double)dataStream.Length / szBlock);

        //    List<Task> tasks = new List<Task>();
        //    for (int i = 0; i < nmBlocks - 1; i++)
        //    {
        //        Task task = new Task(() =>
        //        {
        //            blockData.GetBlockData(szBlock);
        //            BlockData blockDataTask = (BlockData)blockData.Clone();
        //            Calculate(blockDataTask);
        //        }, token ?? CancellationToken.None);
        //        tasks.Add(task);
        //        task.Start();
        //        this.ProcessChanged?.Invoke(this, $"Маркировочная таблица: {i + 1} из {nmBlocks}"); //TODO
        //    }
        //    await Task.WhenAll(tasks.ToArray());

        //    blockData.GetBlockData(szBlock);
        //    Calculate(blockData);

        //    dataStream.Close();
        //}

        /// <summary>
        /// Функция пересчета маркировочной таблицы в таблицу с меньшей размерностью векторов. ФУНКЦИЯ РАБОТАЕТ ТОЛЬКО СО СМЕЩЕНИЕМ 1!
        /// </summary>
        /// <param name="destTable">маркировочная таблица, куда будут записаны пересчитанные значения. смещение должно быть равным 1!</param>
        /// <exception cref="ReduceException">Попытка пересчитать маркировочную таблицу на большую размерность или смещения таблиц не равны 1</exception>
        public void Reduce(MarkTable destTable)
        {
            if (destTable.Dimension > this.Dimension)
                throw new ReduceException("Размерность итоговой таблицы больше размерности исходной!");

            if (destTable._szShift != 1 || this._szShift != 1)
                throw new ReduceException("Значения смещений исходной и итоговой таблиц должны совпадать!");

            destTable._table = new long[1 << destTable._dimension];

            int maskOffset = this._dimension - destTable._dimension;
            long reduceMask = destTable._mask << maskOffset;
            for (int i = 0; i < this._table.Length; i++)
            {
                destTable._table[(i & reduceMask) >> maskOffset] += this._table[i];
            }

            destTable._nmVectors = this._nmVectors;

            if (this._szRemain != 0)
            {
                destTable._remain = this._remain;
                destTable._szRemain = this._szRemain;
                destTable._calculateData(ref destTable._remain, ref destTable._szRemain);
            }
        }

        /// <summary>
        /// Функция создания маркировочной таблицы на основании маркировочной таблицы большей размерности. ФУНКЦИЯ РАБОТАЕТ ТОЛЬКО СО СМЕЩЕНИЕМ 1! 
        /// </summary>
        /// <param name="destDimension">Размерность новой маркировочной таблицы</param>
        /// <returns>Новая маркировочная таблица с пересчитанными значениями и смещением 1</returns>
        public MarkTable Reduce(int destDimension)
        {
            MarkTable resultTable = new MarkTable(destDimension, this._szShift);
            this.Reduce(resultTable);

            return resultTable;
        }

        /// <summary>
        /// Обсчет порции данных
        /// </summary>
        /// <param name="dataBuffer">Буффер для порции данных</param>
        /// <param name="szBuffer">Количество бит данных в буффере</param>
        private void _calculateData(ref BigInteger dataBuffer, ref int szBuffer)
        {
            int offsetMask = 0;
            long maskRemain = 0;
            while (szBuffer >= _dimension)
            {
                //_histogram[(long)dataBuffer & _mask]++;
                offsetMask = szBuffer - _dimension;
                maskRemain = (1 << (offsetMask + (_dimension - _szShift))) - 1;
                _table[(long)((dataBuffer & (_mask << offsetMask)) >> offsetMask)]++;
                _nmVectors++;
                //dataBuffer >>= _szShift;
                dataBuffer &= maskRemain;
                szBuffer -= _szShift;
            }
        }

        /// <summary>
        /// Получение новой порции данных
        /// </summary>
        /// <param name="blockData">Обсчитываемый блок данных</param>
        /// <param name="dataBuffer">Буффер для порции данных. Содержит в том числе данные после прошлых вызовов</param>
        /// <param name="szBuffer">Количество бит данных в буффере</param>
        /// <param name="countReadElements">Количество считанных из блока данных байт</param>
        private void _getDataForCalculate(BlockData blockData, ref BigInteger dataBuffer, ref int szBuffer, ref int countReadElements)
        {
            while (szBuffer < _dimension)
            {
                //dataBuffer |= (BigInteger)blockData.Data[countReadElements++] << szBuffer;
                dataBuffer = (dataBuffer << Tools.BITS_IN_BYTE) | (BigInteger)blockData.Data[countReadElements++];
                szBuffer += Tools.BITS_IN_BYTE;
                if (countReadElements == blockData.Data.Length)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Глубокое сравнение двух экземпляров <see cref="MarkTable"/>
        /// </summary>
        /// <param name="obj">Экземпляр для сравнения</param>
        /// <returns>Результат сравнения</returns>
        public override bool Equals(object? obj)
        {
            var other = obj as MarkTable;

            if (other == null)
                return false;

            if (_dimension != other._dimension || _szShift != other._szShift || _mask != other._mask
                || _remain != other._remain || _szRemain != other._szRemain || _nmVectors != other._nmVectors)
                return false;

            for (int i = 0; i < _table.Length; i++)
            {
                if (_table[i] != other._table[i]) return false;
            }

            return true;
        }

        /// <summary>
        /// Вычисление hash объекта <see cref="MarkTable"/>
        /// </summary>
        /// <returns>Hash объекта <see cref="MarkTable"/></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
