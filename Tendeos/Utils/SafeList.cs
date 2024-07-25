using System;
using System.Collections;
using System.Collections.Generic;

namespace Tendeos.Utils
{
    public class SafeList<T> : IEnumerable<T>
    {
        private readonly Action<T[], uint> destroy;
        private readonly Queue<uint> free = new();
        private readonly T[] array;
        private uint length;

        public SafeList() => array = new T[Limit = 400000];
        public SafeList(uint limit) => array = new T[Limit = limit];

        public SafeList(Action<T[], uint> destroy)
        {
            array = new T[Limit = 400000];
            this.destroy = destroy;
        }

        public SafeList(Action<T[], uint> destroy, uint limit)
        {
            array = new T[Limit = limit];
            this.destroy = destroy;
        }

        public T[] Mutable => array;

        public T this[uint index]
        {
            get => array[index];
            set => array[index] = value;
        }

        public T this[int index]
        {
            get => array[index];
            set => array[index] = value;
        }

        public T this[byte index]
        {
            get => array[index];
            set => array[index] = value;
        }

        public T this[ushort index]
        {
            get => array[index];
            set => array[index] = value;
        }

        public T this[short index]
        {
            get => array[index];
            set => array[index] = value;
        }

        public T this[long index]
        {
            get => array[index];
            set => array[index] = value;
        }

        public T this[ulong index]
        {
            get => array[index];
            set => array[index] = value;
        }

        public uint Max => length;

        public uint Limit { get; }

        public uint Add(T value)
        {
            uint index = free.TryDequeue(out uint f) ? f : length++;
            array[index] = value;
            return index;
        }

        public void Destroy(uint index)
        {
            destroy?.Invoke(array, index);
            free.Enqueue(index);
        }

        public uint Alloc() => free.TryDequeue(out uint f) ? f : length++;

        public void Free(uint index) => free.Enqueue(index);

        public void Clear()
        {
            free.Clear();
            for (uint i = 0; i < length; i++)
            {
                if (destroy == null) array[i] = default;
                else destroy.Invoke(array, i);
            }

            length = 0;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (uint i = 0; i < Max; i++)
                if (!free.Contains(i))
                    yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerable<(uint, T)> IndexEnumerable
        {
            get
            {
                for (uint i = 0; i < Max; i++)
                    if (!free.Contains(i))
                        yield return (i, this[i]);
            }
        }
    }
}