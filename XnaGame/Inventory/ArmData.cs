using System.Collections.Generic;

namespace XnaGame.Inventory
{
    public class ArmData
    {
        private readonly Dictionary<string, object> values = new Dictionary<string, object>();

        public T Get<T>(string name) => values.TryGetValue(name, out object obj) ? (T)obj : default;
        public void Get<T>(out T to, string name) => to = values.TryGetValue(name, out object obj) ? (T)obj : default;
        public void Set(string name, object value) => values.Add(name, value);
        public void Set(params (string name, object value)[] values)
        {
            foreach (var (name, value) in values)
                this.values.Add(name, value);
        }

        public void Clear() => values.Clear();
    }
}
