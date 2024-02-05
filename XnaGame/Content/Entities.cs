using Microsoft.Xna.Framework.Content;
using XnaGame.Utils.Graphics;
using XnaGame.PEntities.Content;
using XnaGame.Utils;
using System;
using System.Reflection;
using System.Collections.Generic;
using XnaGame.PEntities;
using XnaGame.PEntities.Content.EnemyComponents;
using XnaGame.WorldMap;

namespace XnaGame.Content
{
    public static class Entities
    {
        public static Projectile projectile;
        public static Enemy dummy;

        public static void Init(ContentManager content)
        {
            projectile = new Projectile(Vec2.Zero, 0, 200, 1, Sprite.Load(content, "arrow"));
            dummy = new Enemy(10, 25, new Vec2(10, 20),
                new GroundComponent(Sprite.Load(content, "dummy"), 5, 15, 5),
                new LootDropComponent(75, 3..6, Tiles.Get<ITile>("test")));
        }

        public static Func<T> Get<T>(string value) where T : Entity
        {
            FieldInfo t = typeof(Entities).GetField(value);
            return () =>
            {
                if (cash.TryGetValue(value, out Entity entity))
                    return (T)entity;
                T res = (T)t.GetValue(null);
                cash.Add(value, res);
                return res;
            };
        }

        private static Dictionary<string, Entity> cash = new Dictionary<string, Entity>();
    }
}
