using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
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
        public Dictionary<string, MISObject> Objects { get; }
        public Dictionary<string, IModScript> Scripts { get; }
        public Dictionary<string, IModTile> Tiles { get; }
        public Dictionary<string, IModItem> Items { get; }

        public Mod(string path, SpriteBatch batch, ContentManager content)
        {
            Path = path;
            Objects = new Dictionary<string, MISObject>();
            Scripts = new Dictionary<string, IModScript>();
            Tiles = new Dictionary<string, IModTile>();
            Items = new Dictionary<string, IModItem>();
            Load(path, batch, content);
            foreach (var (name, script) in Scripts)
                if (Objects.TryGetValue(name, out MISObject mis))
                    script.Add("mis", mis);
        }

        public void AddToScripts(string name, object value)
        {
            foreach (IModScript script in Scripts.Values)
                script.Add(name, value);
        }

        private void Load(string path, SpriteBatch batch, ContentManager content)
        {
            foreach (string next in Directory.GetDirectories(path))
                Load(next, batch, content);
            int start = Path.Length + 1;
            string name;
            foreach (string next in Directory.GetFiles(path))
            {
                switch (System.IO.Path.GetExtension(next))
                {
                    case ".mis":
                        Objects[next[start..^4]] = MIS.Generate(next);
                        break;
                    case ".js":
                        name = next[start..^3];
                        Scripts[name] = new JSModScript(this, batch, content, name, next);
                        break;
                    case ".lua":
                        name = next[start..^3];
                        Scripts[next[start..^4]] = new LuaModScript(this, batch, content, name, next);
                        break;
                }
            }
        }

        public MISObject mis(string path) => Objects[path];
        public IModScript script(string path) => Scripts[path];
        public IModTile tile(string path) => Tiles[path];
        public IModItem item(string path) => Items[path];

        public static readonly ModOrigin origin = new ModOrigin();

        public readonly struct ModOrigin
        {
            public readonly Origin one = Origin.One;
            public readonly Origin zero = Origin.Zero;
            public readonly Origin center = Origin.Center;
            public ModOrigin() { }
        }

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
            public ModMISValue() { }
        }
    }
}