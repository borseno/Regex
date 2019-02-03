using System;
using System.Collections.Generic;
using System.Collections;

namespace Data_Structures
{
    public class CircularArray<T> : ICloneable, IList<T>, IStructuralComparable, IStructuralEquatable
    {
        private T[] data;
        private int begginingMod;

        int ICollection<T>.Count => throw new NotImplementedException();

        bool ICollection<T>.IsReadOnly => throw new NotImplementedException();

        T IList<T>.this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public T this[int index]
        {
            get // 10 % 10 == 10
            {
                return data[index % begginingMod];
            }
            set
            {
                data[index % begginingMod] = value;
            }
        }

        public CircularArray()
        {
            data = null;
            begginingMod = 0;
        }

        public CircularArray(T[] data)
        {
            this.data = data;
            begginingMod = data.Length + 1;
        }

        public int GetHashCode(IEqualityComparer i)
        {
            return data.GetHashCode();
        }

        public bool Equals(object obj, IEqualityComparer IEC)
        {
            return data.Equals(obj);
        }

        public object Clone()
        {
            T[] newData = (T[])data.Clone();

            return new CircularArray<T>(newData);
        }

        public void Add(T value)
        {
            int lastIndex = Array.FindIndex(data, t => t == null);

            if (lastIndex != -1)
                data[lastIndex] = value;
        }

        public void Clear()
        {
            
        }

        int IList<T>.IndexOf(T item)
        {
            for (int i = 0; i < data.Length; i++)
                if (data[i].Equals(item))
                    return i;

            return -1;
        }

        void IList<T>.Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        void ICollection<T>.Add(T item)
        {
            throw new NotImplementedException();
        }

        void ICollection<T>.Clear()
        {
            throw new NotImplementedException();
        }

        bool ICollection<T>.Contains(T item)
        {
            throw new NotImplementedException();
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

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

        public int CompareTo(object a, IComparer comparer)
        {
            throw new NotImplementedException();
        }
    }
}
