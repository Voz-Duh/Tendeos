using System.Collections.Generic;
using System.IO;
using Tendeos.Utils.Graphics;

namespace Tendeos.Modding
{
    public class Mod
    {
        public string Name { get; internal set; }
        public string Description { get; internal set; }
        public string Tag { get; internal set; }
        public string Path { get; }
        public IModScript mainScript { get; internal set; }
        public Dictionary<string, IModScript> Scripts { get; }
        public Dictionary<string, IModTile> Tiles { get; }
        public Dictionary<string, IModItem> Items { get; }
        public Assets assets;

        public Mod(string path, SpriteBatch batch, Assets assets)
        {
            Path = path;
            Scripts = new Dictionary<string, IModScript>();
            Tiles = new Dictionary<string, IModTile>();
            Items = new Dictionary<string, IModItem>();
            this.assets = assets;

            Load(path, batch, assets, path.Length);
            foreach (var (name, script) in Scripts)
                if (assets.HasMIS(name))
                    script.Add("mis", assets.GetMIS(name));
        }

        public void AddToScripts(string name, object value)
        {
            foreach (IModScript script in Scripts.Values)
                script.Add(name, value);
        }

        private void Load(string path, SpriteBatch batch, Assets assets, int mainDirectoryPathLength)
        {
            foreach (string next in Directory.GetDirectories(path))
                Load(next, batch, assets, mainDirectoryPathLength);
            int start = Path.Length + 1;
            string name;
            foreach (string next in Directory.GetFiles(path))
            {
                string inside = next[mainDirectoryPathLength..];
                switch (System.IO.Path.GetExtension(next))
                {
                    case ".js":
                        name = Assets.ValidPath(inside[start..^3]);
                        Scripts[name] = new JSModScript(this, batch, assets, name, inside);
                        break;
                    case ".lua":
                        name = Assets.ValidPath(inside[start..^4]);
                        Scripts[name] = new LuaModScript(this, batch, assets, name, inside);
                        break;
                }
            }
        }

        public MISObject mis(string path) => assets.GetMIS($"!{Tag}:{path}");
        public Sprite sprite(string path) => assets.GetSprite($"!{Tag}:{path}");
        public IModScript script(string path) => Scripts[$"!{Tag}:{Assets.ValidPath(path)}"];
        public IModTile tile(string path) => Tiles[$"!{Tag}:{Assets.ValidPath(path)}"];
        public IModItem item(string path) => Items[$"!{Tag}:{Assets.ValidPath(path)}"];

        public static readonly ModMISValue misValue = new ModMISValue();

        public readonly struct ModMISValue
        {
            public readonly MISValue non = MISValue.Non;
            public readonly MISValue key = MISValue.Key;
            public readonly MISValue range = MISValue.Range;
            public readonly MISValue @string = MISValue.String;
            public readonly MISValue number = MISValue.Number;
            public readonly MISValue boolean = MISValue.Boolean;
            public readonly MISValue @object = MISValue.Object;

            public ModMISValue()
            {
            }
        }
    }
}