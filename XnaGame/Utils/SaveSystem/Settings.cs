using System;
using System.Collections.Generic;
using System.IO;

namespace XnaGame.Utils.SaveSystem
{
    public static class Settings
    {
        public static string AppData { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Tendeos");
        private static readonly Dictionary<(Type, string), object> data = new Dictionary<(Type, string), object>();

        public static string GetString(string key) => (string)data[(Type.String, key)];
        public static int GetInt(string key) => (int)data[(Type.Int, key)];
        public static float GetFloat(string key) => (float)data[(Type.Float, key)];
        public static bool GetBool(string key) => (bool)data[(Type.Bool, key)];

        public static void Set(Type type, string key, object to) => data[(type, key)] = to;

        public static void Save()
        {
            using Stream stream = File.Open(Path.Combine(AppData, ".settings"), FileMode.Create);
            ByteBuffer buffer = new ByteBuffer(stream);
            buffer.Append(data.Count);
            foreach (var item in data)
            {
                buffer.Append((byte)item.Key.Item1);
                buffer.Append(item.Key.Item2);
                switch (item.Key.Item1)
                {
                    case Type.String:
                        buffer.Append((string)item.Value);
                        break;
                    case Type.Int:
                        buffer.Append((int)item.Value);
                        break;
                    case Type.Float:
                        buffer.Append((float)item.Value);
                        break;
                    case Type.Bool:
                        buffer.Append((bool)item.Value);
                        break;
                }
            }
            stream.Close();
        }

        public static void Load()
        {
            string path = Path.Combine(AppData, ".settings");
            data.Clear();

            if (!File.Exists(path))
            {
                Default();
                Save();
                return;
            }

            using Stream stream = File.Open(path, FileMode.Open);
            ByteBuffer buffer = new ByteBuffer(stream);
            string name;
            object obj = null;
            int length = buffer.ReadInt();
            for (int i = 0; i < length; i++)
            {
                Type type = (Type)buffer.ReadByte();
                name = buffer.ReadString();
                switch (type)
                {
                    case Type.String:
                        obj = buffer.ReadString();
                        break;
                    case Type.Int:
                        obj = buffer.ReadInt();
                        break;
                    case Type.Float:
                        obj = buffer.ReadFloat();
                        break;
                    case Type.Bool:
                        obj = buffer.ReadBool();
                        break;
                }

                data.Add((type, name), obj);
            }
            stream.Close();
        }

        private static void Default()
        {
            Set(Type.String, "language", "en");
        }

        public enum Type : byte
        {
            String,
            Int,
            Float,
            Bool
        }
    }
}
