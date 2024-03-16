using System.IO;
using System.Threading.Tasks;

namespace XnaGame.Utils.SaveSystem
{
    public static class Save
    {
        private static object instance;
        private static string name;

        public static void Unload()
        {
            if (!Directory.Exists(Settings.AppData)) Directory.CreateDirectory(Settings.AppData);
            string path = Path.Combine(Settings.AppData, $"{name}.save");
            using FileStream stream = File.Open(path, FileMode.Create);
            ByteBuffer buffer = new ByteBuffer(stream);
            buffer.Append(instance, instance.GetType());
            stream.Close();
        }

        public static async Task<bool> LoadAsync(string name, object saveInstance)
        {
            Save.name = name;
            string path = Path.Combine(Settings.AppData, $"{name}.save");

            if (!File.Exists(path)) return false;
            await Task.Run(() =>
            {
#if RELEASE
                Debug.Safe(Core.Instance, () =>
                {
#endif
                    using FileStream stream = File.Open(path, FileMode.Open);
                    ByteBuffer buffer = new ByteBuffer(stream);
                    buffer.Read(instance, instance.GetType());
                    stream.Close();
#if RELEASE
                });
#endif
            });
            return true;
        }

        public static async Task<bool> Load(string name, object saveInstance)
        {
            Save.name = name;
            string path = Path.Combine(Settings.AppData, $"{name}.save");

            if (!File.Exists(path)) return false;
            using FileStream stream = File.Open(path, FileMode.Open);
            ByteBuffer buffer = new ByteBuffer(stream);
            buffer.Read(instance, instance.GetType());
            stream.Close();
            return true;
        }

        public static void Create(string name, object saveInstance)
        {
            Save.name = name;
            instance = saveInstance;
        }

        public static void SetInstance(object saveInstance) => instance = saveInstance;
    }
}
