using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace BigCollections
{
    /// <summary>
    /// BigArray class
    /// https://github.dev/dotnetbio/bio/blob/master/Source/Bio.Core/BigArray.cs
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BigArray<T> : IEnumerable<T>
    {
        #region Member variables
        // jagged array to hold data
        private T[][] _data;

        // maximum block size.
        private long _blockSize;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the BigArray.
        /// </summary>
        /// <param name="length">Size of the BigArray to create.</param>
        public BigArray(long length)
        {
            Allocate(length);
        }

        /// <summary>
        /// Initializes a new instance of the BigArray with elements from the specified collection.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new BigArray.</param>
        public BigArray(IEnumerable<T> collection)
        {
            AllocateFromEnumerable(collection);
        }

        /// <summary>
        /// Initializes a new instance of the BigArray with elements from the specified collection.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new BigArray.</param>
        /// <param name="collectionCount">Size of list</param>
        public BigArray(IEnumerable<T> collection, long collectionCount)
        {
            Allocate(collectionCount);
            int index = 0;
            foreach (var item in collection)
            {
                this[index++] = item;
            }
        }

        #endregion

        #region Properties
        /// <summary>
        /// Total number of elements contained within this instance.
        /// </summary>
        public long Length { get; private set; }

        /// <summary>
        /// Maximum elements can be stored in an internal block.
        /// </summary>
        public long BlockSize
        {
            get
            {
                return _blockSize;
            }
        }

        /// <summary>
        /// Number of internal blocks
        /// </summary>
        public int BlockCount => _data.Length;
        #endregion

        #region Methods
        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the
        ///     first occurrence within the entire BigArray.
        /// </summary>
        /// <param name="item">The object to locate in the BigArray. The value
        ///     can be null for reference types.</param>
        /// <returns>The zero-based index of the first occurrence of item within the entire BigArray,
        ///     if found; otherwise, –1.</returns>
        public long IndexOf(T item)
        {
            return IndexOf(item, 0, Length);
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the
        ///     first occurrence within the range of elements in the BigArray
        ///     that extends from the specified index to the last element.
        /// </summary>
        /// <param name="item">The object to locate in the BigArray. The value
        ///     can be null for reference types.</param>
        /// <param name="startIndex">The zero-based starting index of the search. 0 (zero) 
        /// is valid in an empty BigArray.</param>
        /// <returns> The zero-based index of the first occurrence of item within the range of
        ///     elements in the BigArray that extends from index to the last element, 
        ///     if found; otherwise, –1.</returns>
        public long IndexOf(T item, long startIndex)
        {
            return IndexOf(item, startIndex, Length - startIndex);
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the
        ///     first occurrence within the range of elements in the BigArray
        ///     that starts at the specified index and contains the specified number of elements.
        /// </summary>
        /// <param name="item">The object to locate in the BigArray. The value
        ///     can be null for reference types.</param>
        /// <param name="startIndex">The zero-based starting index of the search. 0 (zero) is valid in an empty
        ///     BigArray.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <returns>The zero-based index of the first occurrence of item within the range of
        ///     elements in the BigArray that starts at index and
        ///     contains count number of elements, if found; otherwise, –1.</returns>
        public long IndexOf(T item, long startIndex, long count)
        {
            long index = -1;

            if (startIndex < 0 || startIndex > Length)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }
            if (count < 0 || count > Length - startIndex)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            long blockIndex = startIndex / _blockSize;
            int start = (int)(startIndex % _blockSize);
            count += startIndex;
            for (long i = startIndex; i < count && blockIndex < _data.Length; blockIndex++)
            {
                int len = _data[blockIndex].Length;

                if (i + len > count)
                {
                    len = (int)(count - i);
                }

                index = Array.IndexOf(_data[blockIndex], item, start, len);
                start = 0;
                if (index != -1)
                {
                    index += blockIndex * _blockSize;
                    break;
                }

                i += len;
            }

            return index;
        }

        /// <summary>
        /// Sets all elements in the BigArray to zero, to false, or to null,
        ///     depending on the element type.
        /// </summary>
        public void Clear()
        {
            Clear(0, Length);
        }

        /// <summary>
        ///  Sets a range of elements in the BigArray to zero, to false, or to null,
        ///     depending on the element type.
        /// </summary>
        /// <param name="startIndex">The starting index of the range of elements to clear.</param>
        /// <param name="count">The number of elements to clear.</param>
        public void Clear(long startIndex, long count)
        {
            if (startIndex < 0 || startIndex > Length)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }

            if (count < 0 || count > Length - startIndex)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            long blockIndex = startIndex / _blockSize;
            int start = (int)(startIndex % _blockSize);
            count += startIndex;
            for (long i = startIndex; i < count && blockIndex < _data.Length; blockIndex++)
            {
                int len = _data[blockIndex].Length;
                if (i + len > count)
                {
                    len = (int)(count - i);
                }

                Array.Clear(_data[blockIndex], start, len);
                start = 0;
                i += len;
            }
        }

        /// <summary>
        /// Changes the size of the BigArray to the specified new size.
        /// </summary>
        /// <param name="newSize">The size of the new BigArray.</param>
        public void Resize(long newSize)
        {
            if (newSize == Length)
            {
                return;
            }

            int blockCount = (int)(newSize / _blockSize);
            if (newSize > blockCount * _blockSize)
            {
                blockCount++;
            }

            int previousBlockCount = _data.Length;

            int lastBlockSize = (int)(newSize - (blockCount - 1) * _blockSize);
            int previousLastBlockSize = (int)(Length - (blockCount - 1) * _blockSize);

            if (previousBlockCount != blockCount)
            {
                if (previousBlockCount < blockCount) //  Increasing size, make more.
                {
                    if (previousLastBlockSize != _blockSize)
                    {
                        Array.Resize(ref _data[previousBlockCount - 1], (int)_blockSize);
                    }

                    Array.Resize(ref _data, blockCount);
                    for (int i = previousBlockCount; i < blockCount - 1; i++)
                    {
                        _data[previousBlockCount] = new T[_blockSize];
                    }

                    _data[blockCount - 1] = new T[lastBlockSize];
                }
                else   // Reducing size - cut off blocks.
                {
                    Array.Resize(ref _data, blockCount);
                    Array.Resize(ref _data[blockCount - 1], lastBlockSize);
                }
            }
            else // resize the last block
            {
                Array.Resize(ref _data[blockCount - 1], lastBlockSize);
            }

            Length = newSize;
        }

        /// <summary>
        /// Copies a range of elements from the BigArray to a compatible one-dimensional array.
        /// </summary>
        /// <param name="index">The zero-based index in the source BigArray at
        ///     which copying begins.</param>
        /// <param name="destinationArray">The one-dimensional array that is the destination of the elements
        ///     copied from BigArray.</param>
        /// <param name="count">The number of elements to copy.</param>
        public void CopyTo(long index, T[] destinationArray, long count)
        {
            CopyTo(index, destinationArray, 0, count);
        }

        /// <summary>
        /// Copies a range of elements from the BigArray to a compatible one-dimensional array, 
        /// starting at the specified index of the destination array.
        /// </summary>
        /// <param name="index">The zero-based index in the source BigArray at
        ///     which copying begins.</param>
        /// <param name="destinationArray">The one-dimensional array that is the destination of the elements
        ///     copied from BigArray.</param>
        /// <param name="destinationIndex">The zero-based index in destinationArray at which copying begins.</param>
        /// <param name="count">The number of elements to copy.</param>
        public void CopyTo(long index, T[] destinationArray, int destinationIndex, long count)
        {
            ArgumentNullException.ThrowIfNull(destinationArray, nameof(destinationArray));

            if (index < 0 || index > Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            if (destinationIndex < 0 || destinationIndex > destinationArray.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(destinationIndex));
            }
            if (count < 0 || count > Length - index || count > destinationArray.Length - destinationIndex)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            int destIndex = destinationIndex;
            for (long i = index; i < index + count; i++)
            {
                destinationArray[destIndex++] = this[i];
            }
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index"> The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        public T this[long index]
        {
            get
            {
                return _data[(int)(index / _blockSize)][index % _blockSize];
            }

            set
            {
                _data[(int)(index / _blockSize)][index % _blockSize] = value;
            }
        }

        /// <summary>
        /// Creates a new span over a target block
        /// </summary>
        /// <param name="block">Block number</param>
        /// <returns>The span representation of the block</returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public Span<T> AsSpan(int block)
        {
            if (block >= BlockCount)
            {
                throw new IndexOutOfRangeException($"{nameof(block)} must be less than {nameof(BlockCount)}");
            }

            T[] data = _data[block];

            return data.AsSpan();
        }

        /// <summary>
        /// Cretes a new BigList from the BigArray.
        /// Use with caution with large BigArray, as this will copy all elements..
        /// </summary>
        /// <returns></returns>
        public BigList<T> ToBigList()
        {
            var list = new BigList<T>(Length);

            for (long i = 0; i < Length; i++)
            {
                list.Add(this[i]);
            }

            return list;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the BigArray.
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            return _data.SelectMany(t => t).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the BigArray.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Allocate the memory for the BigArray.
        /// </summary>
        /// <param name="length">Length of the BigArray.</param>
        private void Allocate(long length)
        {
            if (length < 0)
            {
                throw new ArgumentException("Must specify a length >= 0");
            }

            Length = length;

            //NOTE: _blockSize is optimized for 64bit process.
            if (typeof(T).GetTypeInfo().IsValueType)
            {
                int itemSize = Marshal.SizeOf(typeof(T));
                _blockSize = (int.MaxValue - 56) / itemSize;
            }
            else
            {
                // 8 bytes are required to store reference.
                int itemSize = Environment.Is64BitProcess ? 8 : 4;
                _blockSize = (int.MaxValue - 56) / itemSize - 1;
            }

            // Get the number of array elements we need
            int blockCount = (int)(length / _blockSize);
            if (length > blockCount * _blockSize)
            {
                blockCount++;
            }

            // Allocate our arrays
            _data = new T[blockCount][];

            // Allocate full blocks.
            for (int i = 0; i < blockCount - 1; i++)
            {
                _data[i] = new T[_blockSize];
            }

            // Allocate the final array element with exactly the space required.
            if (blockCount > 0)
            {
                _data[blockCount - 1] = new T[length - (blockCount - 1) * _blockSize];
            }
        }

        /// <summary>
        /// Allocate the memory for the BigArray.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new BigList.</param>
        private void AllocateFromEnumerable(IEnumerable<T> collection)
        {
            if (collection is ICollection<T> collectionObj)
            {
                int count = collectionObj.Count;
                _data = new T[1][];
                _data[0] = new T[count];
                int index = 0;
                foreach (var item in collectionObj)
                {
                    _data[0][index++] = item;
                }
                Length = count;
            }
            else
            {
                int count = collection.Count();
                _data = new T[1][];
                _data[0] = new T[count];
                int index = 0;
                using (IEnumerator<T> enumerator = collection.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        _data[0][index++] = enumerator.Current;
                    }
                }

                Length = count;
            }
        }

        #endregion

        #region Empty BigArray

        public static readonly BigArray<T> Empty = new(0);

        #endregion
    }
}
