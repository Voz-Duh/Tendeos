using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System.Reflection;
using XnaGame.Utils.Graphics;
using XnaGame.World;
using XnaGame.World.Content;

namespace XnaGame.Content
{
    public static class Tiles
    {
        public static ITile ignore, test, stone;

        public static void Init(ContentManager content)
        {
            ignore = new TileTag();
            test = new AutoTile(Sprite.Load(content, "tiles/test"), Sprite.Load(content, "tiles/test_item"))
            {
                Hardness = 1,
                Health = 2
            };
            stone = new AutoTile(Sprite.Load(content, "tiles/stone"), Sprite.Load(content, "tiles/stone_item"))
            {
                ShadowIntensity = 0.1f,
                Hardness = 2,
                Health = 4
            };
        }

        public static TileRef Get<T>(string value) where T : ITile
        {
            if (value == "air") return () => null;
            FieldInfo t = typeof(Tiles).GetField(value);
            return () =>
            {
                if (cash.TryGetValue(value, out ITile entity))
                    return (T)entity;
                T res = (T)t.GetValue(null);
                cash.Add(value, res);
                return res;
            };
        }

        public static ItemRef GetItem(string value)
        {
            var @ref = Get<ITile>(value);
            return () => @ref();
        }

        private static readonly Dictionary<string, ITile> cash = new Dictionary<string, ITile>();
    }

    public delegate ITile TileRef();
}
