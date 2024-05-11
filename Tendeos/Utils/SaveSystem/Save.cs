using System.IO;
using System.Threading.Tasks;
using LZ4;

namespace Tendeos.Utils.SaveSystem
{
    public static class Save
    {
        private static object instance;
        private static string name;

        public static void Unload()
        {
            string pathToFolder = Path.Combine(Settings.AppData, "saves");
            if (!Directory.Exists(pathToFolder)) Directory.CreateDirectory(pathToFolder);
            string path = Path.Combine(pathToFolder, $"{name}.save");
            using FileStream stream = File.Open(path, FileMode.Create);
            using LZ4Stream zStream = new LZ4Stream(stream, LZ4StreamMode.Compress);
            ByteBuffer buffer = new ByteBuffer(zStream);
            buffer.Append(instance);
            zStream.Close();
            stream.Close();
        }

        public static async Task<bool> LoadAsync(string name)
        {
            Save.name = name;
            string path = Path.Combine(Settings.AppData, "saves", $"{name}.save");

            if (!File.Exists(path)) return false;
            await Task.Run(() =>
            {
#if RELEASE
                Debug.Safe(Core.Game, () =>
                {
#endif
                using FileStream stream = File.Open(path, FileMode.Open);
                using LZ4Stream zStream = new LZ4Stream(stream, LZ4StreamMode.Decompress);
                ByteBuffer buffer = new ByteBuffer(zStream);
                buffer.Read(instance);
                zStream.Close();
                stream.Close();
#if RELEASE
                });
#endif
            });
            return true;
        }

        public static bool Load(string name)
        {
            Save.name = name;
            string path = Path.Combine(Settings.AppData, "saves", $"{name}.save");

            if (!File.Exists(path)) return false;
            using FileStream stream = File.Open(path, FileMode.Open);
            using LZ4Stream zStream = new LZ4Stream(stream, LZ4StreamMode.Decompress);
            ByteBuffer buffer = new ByteBuffer(zStream);
            buffer.Read(instance);
            zStream.Close();
            stream.Close();
            return true;
        }

        public static void Create(string name)
        {
            Save.name = name;
        }

        public static void SetInstance(object saveInstance) => instance = saveInstance;
    }
}
