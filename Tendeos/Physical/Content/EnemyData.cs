using System.Collections.Generic;
using Tendeos.Utils;

namespace Tendeos.Physical.Content
{
    public class EnemyData
    {
        public int Current;
        private readonly Dictionary<(int, string), object> values = new();

        public T Get<T>(string name) => values.TryGetValue((Current, name), out object obj) ? (T) obj : default;

        public void Get<T>(out T to, string name) =>
            to = values.TryGetValue((Current, name), out object obj) ? (T) obj : default;

        public void Set(string name, object value) => values[(Current, name)] = value;

        public void Set(params (string name, object value)[] values)
        {
            foreach (var (name, value) in values)
                this.values[(Current, name)] = value;
        }

        public void ToByte(ByteBuffer buffer)
        {
            buffer.Append(values.Count);
            foreach (var ((i, name), value) in values)
            {
                buffer.Append(i).Append(name).AppendUndefined(value);
            }
        }
    }
}