using System;
using System.Collections.Generic;

namespace Tendeos.Utils
{
    public class SafeList<T>
    {
        private readonly Action<T[], uint> destroy;
        private readonly Queue<uint> free = new Queue<uint>();
        private readonly T[] array;
        private uint length;

        public SafeList() => array = new T[255];
        public SafeList(uint limit) => array = new T[limit];
        public SafeList(Action<T[], uint> destroy)
        {
            array = new T[255];
            this.destroy = destroy;
        }

        public SafeList(Action<T[], uint> destroy, uint limit)
        {
            array = new T[limit];
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
    }
}
