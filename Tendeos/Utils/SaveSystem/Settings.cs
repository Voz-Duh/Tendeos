using LZ4;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Tendeos.World.Shadows;

namespace Tendeos.Utils.SaveSystem
{
    public static class Settings
    {
        public static string AppData { get; } =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Tendeos");

        private static readonly Dictionary<(Type, string), object> data = new();
        
        private static event Action SetDefault;

        public static string GetString(string key) => (string) data[(Type.String, key)];
        public static int GetInt(string key) => (int) data[(Type.Int, key)];
        public static float GetFloat(string key) => (float) data[(Type.Float, key)];
        public static bool GetBool(string key) => (bool) data[(Type.Bool, key)];

        public static void Set(Type type, string key, object to) => data[(type, key)] = to;

        public static void Default(Type type, string key, object to) => SetDefault += () => data[(type, key)] = to;

        public static async Task SaveAsync() => await Task.Run(Save);
        public static async Task LoadAsync() => await Task.Run(Load);

        public static void Save()
        {
            using Stream stream = File.Open(Path.Combine(AppData, ".settings"), FileMode.Create);
            using LZ4Stream zStream = new LZ4Stream(stream, LZ4StreamMode.Compress);
            ByteBuffer buffer = new ByteBuffer(zStream);
            buffer.Append(data.Count);
            foreach (var item in data)
            {
                buffer.Append((byte) item.Key.Item1);
                buffer.Append(item.Key.Item2);
                switch (item.Key.Item1)
                {
                    case Type.String:
                        buffer.Append((string) item.Value);
                        break;
                    case Type.Int:
                        buffer.Append((int) item.Value);
                        break;
                    case Type.Float:
                        buffer.Append((float) item.Value);
                        break;
                    case Type.Bool:
                        buffer.Append((bool) item.Value);
                        break;
                }
            }

            zStream.Close();
            stream.Close();
        }

        public static void Load()
        {
            string path = Path.Combine(AppData, ".settings");
            data.Clear();
            
            SetDefault();

            if (!File.Exists(path))
            {
                Save();
                return;
            }

            using Stream stream = File.Open(path, FileMode.Open);
            using LZ4Stream zStream = new LZ4Stream(stream, LZ4StreamMode.Decompress);
            ByteBuffer buffer = new ByteBuffer(zStream);
            string name;
            object obj = null;
            int length = buffer.ReadInt();
            for (int i = 0; i < length; i++)
            {
                Type type = (Type) buffer.ReadByte();
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

                data[(type, name)] = obj;
            }

            zStream.Close();
            stream.Close();
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