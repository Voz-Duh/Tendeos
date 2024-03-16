using System.Collections.Generic;

namespace XnaGame.Physical.Content
{
    public class EnemyData
    {
        public int current;
        private readonly Dictionary<string, object> values = new Dictionary<string, object>();

        public T Get<T>(string name) => values.TryGetValue(current + name, out object obj) ? (T)obj : default;
        public void Get<T>(out T to, string name) => to = values.TryGetValue(current + name, out object obj) ? (T)obj : default;
        public void Set(string name, object value) => values[current + name] = value;
        public void Set(params (string name, object value)[] values)
        {
            foreach (var (name, value) in values)
                this.values[current + name] = value;
        }
    }
}
