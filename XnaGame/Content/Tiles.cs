using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Reflection;
using XnaGame.Inventory;
using XnaGame.Utils.Graphics;
using XnaGame.WorldMap;
using XnaGame.WorldMap.Content;

namespace XnaGame.Content
{
    public static class Tiles
    {
        public static ITile test;

        public static void Init(ContentManager content)
        {
            test = new AutoTile(2, Sprite.Load(content, "tiles/test"), Sprite.Load(content, "tiles/test_item"));
        }

        public static Func<T> Get<T>(string value) where T : ITile
        {
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

        private static Dictionary<string, ITile> cash = new Dictionary<string, ITile>();
    }
}
