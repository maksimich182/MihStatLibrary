using MihStatLibrary.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MihStatLibrary.Histogram
{
    /// <summary>
    /// Класс истограммы частот
    /// </summary>
    public class FreqHistogram
    {
        //TODO: Сделать обработку недостаточного объема памяти

        private int _dimension;  //TODO: сделать проверку на < 64, так как памяти вряд ли хватит на такой объем гистограммы
        private int _szShift;
        private long _mask;
        private long[] _histogram;
        private BigInteger _remain;
        private int _szRemain;
        private long _nmVectors;

        /// <summary>
        /// Гистограмма частот
        /// </summary>
        public long[] Histogram { get { return _histogram; } }

        /// <summary>
        /// Размерность гистограммы частот
        /// </summary>
        public int Dimension { get { return _dimension; } }

        /// <summary>
        /// Количество учтенных последовательностей
        /// </summary>
        public long NmVectors { get { return _nmVectors; } }


        /// <summary>
        /// Конструктор класса гистограммы частот без зацепления
        /// </summary>
        /// <param name="dimension">Размерность векторов гистограммы</param>
        public FreqHistogram(int dimension)
        {
            _dimension = dimension;
            _szShift = _dimension;
            _mask = (1 << _dimension) - 1;
            _histogram = new long[1 << _dimension];
            _remain = 0;
            _szRemain = 0;
            _nmVectors = 0;
        }

        /// <summary>
        /// Конструктор класса гистограммы частот с зацеплением
        /// </summary>
        /// <param name="dimension">Размерность векторов гистограммы</param>
        /// <param name="szShift">Размер смещения</param>
        /// <exception cref="InvalidShiftException">Попытка задать смещение больше размерности</exception>
        public FreqHistogram(int dimension, int szShift) : this(dimension)
        {
            if (szShift > dimension)
                throw new InvalidShiftException("Смещение должно быть меньше размерности!");
            _szShift = szShift;
        }

        /// <summary>
        /// Подсчет гистограммы частот по блоку данных
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
                _getDataForCalculate(data, ref dataBuffer, ref szBuffer, ref countReadElements);
                _calculateData(ref dataBuffer, ref szBuffer);
            }

            _remain = dataBuffer;
            _szRemain = szBuffer;
        }

        /// <summary>
        /// Обнуление гистограммы частот
        /// </summary>
        public void Clear()
        {
            Array.Clear(_histogram, 0, _histogram.Length);
            _remain = 0;
            _szRemain = 0;
            _nmVectors = 0;
        }

        /// <summary>
        /// Подсчет гистограммы частот по данным файла
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        public void Calculate(string fileName)
        {
            FileStream dataStream = new FileStream(fileName, FileMode.Open);
            BlockData blockData = new BlockData(new BlockDataFileSource(dataStream));

            double nmBlocks = Math.Ceiling((double)dataStream.Length / Tools.SIZE_BLOCK_BYTES);
            for (int i = 0; i < nmBlocks; i++)
            {
                blockData.GetBlockData(Tools.SIZE_BLOCK_BYTES);
                Calculate(blockData);
                Console.WriteLine($"Гистограмма частот: {i + 1} из {nmBlocks}"); //TODO event
            }
            dataStream.Close();

        }

        /// <summary>
        /// Функция пересчета гистограммы частот в гистограмму с меньшей размерностью векторов. ФУНКЦИЯ РАБОТАЕТ ТОЛЬКО СО СМЕЩЕНИЕМ 1!
        /// </summary>
        /// <param name="destHistogram">Гистограмма, куда будут записаны пересчитанные значения. смещение должно быть равным 1!</param>
        /// <exception cref="ReduceException">Попытка пересчитать гистограмму на большую размерность или смещения гистограм не равны 1</exception>
        public void Reduce(FreqHistogram destHistogram)
        {
            if (destHistogram.Dimension > this.Dimension)
                throw new ReduceException("Размерность итоговой гистограммы больше размерности исходной!");

            if (destHistogram._szShift != 1 || this._szShift != 1)
                throw new ReduceException("Значения смещений исходной и итоговой гистограммы должны совпадать!");

            destHistogram._histogram = new long[1 << destHistogram._dimension];

            int maskOffset = this._dimension - destHistogram._dimension;
            long reduceMask = destHistogram._mask << maskOffset;
            for (int i = 0; i < this._histogram.Length; i++)
            {
                destHistogram._histogram[(i & reduceMask) >> maskOffset] += this._histogram[i];
            }

            destHistogram._nmVectors = this._nmVectors;

            if (this._szRemain != 0)
            {
                destHistogram._remain = this._remain;
                destHistogram._szRemain = this._szRemain;
                destHistogram._calculateData(ref destHistogram._remain, ref destHistogram._szRemain);
            }
        }

        /// <summary>
        /// Функция создания гистограммы частот на основании гистограммы частот большей размерности. ФУНКЦИЯ РАБОТАЕТ ТОЛЬКО СО СМЕЩЕНИЕМ 1! 
        /// </summary>
        /// <param name="pDestDimension">Размерность новой гистограммы</param>
        /// <returns>Новая гистограмма с пересчитанными значениями и сдвигом 1</returns>
        public FreqHistogram Reduce(int pDestDimension)
        {
            FreqHistogram resultHistogram = new FreqHistogram(pDestDimension, this._szShift);
            this.Reduce(resultHistogram);

            return resultHistogram;
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
                _histogram[(long)((dataBuffer & (_mask << offsetMask)) >> offsetMask)]++;
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
        /// Глубокое сравнение двух экземпляров <see cref="FreqHistogram"/>
        /// </summary>
        /// <param name="obj">Экземпляр для сравнения</param>
        /// <returns>Результат сравнения</returns>
        public override bool Equals(object? obj)
        {
            var other = obj as FreqHistogram;

            if(other == null)
                return false;

            if (_dimension != other._dimension || _szShift != other._szShift || _mask != other._mask
                || _remain != other._remain || _szRemain != other._szRemain || _nmVectors != other._nmVectors)
                return false;

            for(int i = 0; i < _histogram.Length; i++)
            {
                if (_histogram[i] != other._histogram[i]) return false;
            }

            return true;
        }
    }
}
