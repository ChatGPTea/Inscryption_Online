#if !USE_DYNAMIC_STACKS

using System;
using System.Collections;
using System.Collections.Generic;

namespace MoonSharp.Interpreter.DataStructs
{
	/// <summary>
	///     A preallocated, non-resizable, stack
	/// </summary>
	/// <typeparam name="T"></typeparam>
	internal class FastStack<T> : IList<T>
    {
        private readonly T[] m_Storage;

        public FastStack(int maxCapacity)
        {
            m_Storage = new T[maxCapacity];
        }

        public T this[int index]
        {
            get => m_Storage[index];
            set => m_Storage[index] = value;
        }

        public int Count { get; private set; }

        public T Push(T item)
        {
            m_Storage[Count++] = item;
            return item;
        }

        public void Expand(int size)
        {
            Count += size;
        }

        private void Zero(int from, int to)
        {
            Array.Clear(m_Storage, from, to - from + 1);
        }

        private void Zero(int index)
        {
            m_Storage[index] = default;
        }

        public T Peek(int idxofs = 0)
        {
            var item = m_Storage[Count - 1 - idxofs];
            return item;
        }

        public void Set(int idxofs, T item)
        {
            m_Storage[Count - 1 - idxofs] = item;
        }

        public void CropAtCount(int p)
        {
            RemoveLast(Count - p);
        }

        public void RemoveLast(int cnt = 1)
        {
            if (cnt == 1)
            {
                --Count;
                m_Storage[Count] = default;
            }
            else
            {
                var oldhead = Count;
                Count -= cnt;
                Zero(Count, oldhead);
            }
        }

        public T Pop()
        {
            --Count;
            var retval = m_Storage[Count];
            m_Storage[Count] = default;
            return retval;
        }

        public void Clear()
        {
            Array.Clear(m_Storage, 0, m_Storage.Length);
            Count = 0;
        }


        #region IList<T> Impl.

        int IList<T>.IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        void IList<T>.Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        T IList<T>.this[int index]
        {
            get => this[index];
            set => this[index] = value;
        }

        void ICollection<T>.Add(T item)
        {
            Push(item);
        }

        void ICollection<T>.Clear()
        {
            Clear();
        }

        bool ICollection<T>.Contains(T item)
        {
            throw new NotImplementedException();
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        int ICollection<T>.Count => Count;

        bool ICollection<T>.IsReadOnly => false;

        bool ICollection<T>.Remove(T item)
        {
            throw new NotImplementedException();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

#endif