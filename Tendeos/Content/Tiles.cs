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
        public static ITile ignore, reference, test, stone, dirt, grass, tree;

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
    }
}
