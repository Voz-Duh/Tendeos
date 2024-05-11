using Jint.Native;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System.Reflection;
using Tendeos.Modding;
using Tendeos.World;
using Tendeos.World.Content;

namespace Tendeos.Content
{
    public static class Tiles
    {
        public static ITile ignore, reference, test, stone, dirt, grass, tree, deep_stone;

        public static void Init(ContentManager content)
        {
            ignore = new TileTag();
            reference = new ReferenceTile();
            test = new AutoTile()
            {
                Hardness = 1,
                Health = 2
            };
            stone = new AutoTile()
            {
                Hardness = 2,
                Health = 4
            };
            deep_stone = new AutoTile()
            {
                Hardness = 2,
                Health = 4
            };
            dirt = new AutoTile()
            {
                Hardness = 1,
                Health = 2
            };
            grass = new AutoTile()
            {
                Hardness = 1,
                Health = 2,
                DropTag = "dirt"
            };
            tree = new Tree("dirt")
            {
                Hardness = 1,
                Health = 2
            };
        }

        public static ITile Get(string value)
        {
            if (value == "air") return null;
            FieldInfo field = typeof(Tiles).GetField(value);
            if (field == null)
            {
                foreach (Mod mod in Mods.Loaded.Values)
                    if (mod.Tiles.TryGetValue(value, out IModTile tile)) return tile;
                throw new KeyNotFoundException(value);
            }
            return (ITile)field.GetValue(null);
        }

        private static Dictionary<string, ITile> ꥆall;
        public static Dictionary<string, ITile> All
        {
            get
            {
                if (ꥆall == null)
                {
                    ꥆall = new Dictionary<string, ITile>{{ "air", null }};
                    foreach (FieldInfo field in typeof(Tiles).GetFields())
                    {
                        if (field.Name != "ꥆall" && field.Name != "reference")
                            ꥆall.Add(field.Name, (ITile)field.GetValue(null));
                    }
                    foreach (Mod mod in Mods.Loaded.Values)
                        foreach (var (key, tile) in mod.Tiles)
                        {
                            ꥆall.Add(key, tile);
                        }
                }
                return ꥆall;
            }
        }
    }
}
