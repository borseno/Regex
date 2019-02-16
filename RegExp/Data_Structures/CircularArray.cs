using System;
using System.Collections.Generic;
using System.Collections;

namespace Data_Structures
{
    public class CircularArray<T> : ICloneable, IList<T>, IStructuralComparable, IStructuralEquatable
    {
        private T[] _data;
        private readonly int _beginningMod;

        public T this[int index]
        {
            get // 
            {
                return _data[index % _beginningMod];
            }
            set
            {
                _data[index % _beginningMod] = value;
            }
        }

        public CircularArray(int length)
        {
            _data = new T[length];
            _beginningMod = _data.Length;
        }

        public CircularArray(T[] data)
        {
            this._data = data;
            _beginningMod = data.Length;
        }

        public int GetHashCode(IEqualityComparer i)
        {
            return _data.GetHashCode();
        }

        public bool Equals(object obj, IEqualityComparer IEC)
        {
            return _data.Equals(obj);
        }

        public object Clone()
        {
            T[] newData = (T[])_data.Clone();

            return new CircularArray<T>(newData);
        }

        public void Clear()
        {
            for (int i = 0; i < _data.Length; i++)
            {
                _data[i] = default(T);
            }
        }

        public int IndexOf(T item)
        {   
            for (int i = 0; i < _data.Length; i++)
                if (_data[i].Equals(item))
                    return i;

            return -1;
        }

        public bool Contains(T item)
        {
            foreach (var i in _data)
                if (i.Equals(item))
                    return true;
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _data.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var t in _data)
            {
                yield return t;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < _data.Length; i++)
            {
                yield return _data[i];
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        void IList<T>.Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        int ICollection<T>.Count
        {
            get { throw new NotSupportedException(); }
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        int IStructuralComparable.CompareTo(object a, IComparer comparer)
        {
            throw new NotSupportedException();
        }
    }
}
