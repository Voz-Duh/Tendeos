using Microsoft.Xna.Framework.Content;
using XnaGame.Utils.Graphics;
using XnaGame.PEntities.Content;
using XnaGame.Utils;
using System.Reflection;
using System.Collections.Generic;
using XnaGame.PEntities;
using XnaGame.PEntities.Content.EnemyComponents;

namespace XnaGame.Content
{
    public static class Entities
    {
        public static Projectile projectile;
        public static Enemy dummy;

        public static void Init(ContentManager content)
        {
            projectile = new Projectile(Sprite.Load(content, "arrow"))
            {
                Speed = 200,
                Damage = 1
            };
            dummy = new Enemy(10, 25, new Vec2(10, 20),
                new GroundComponent(Sprite.Load(content, "dummy"))
                {
                    Acceleration = 5,
                    MaxSpeed = 15,
                    Drag = 5
                },
                new LootDropComponent(chance: 75, 3..6, Tiles.GetItem("test")));
        }

        public static EntityRef<T> Get<T>(string value) where T : Entity
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

        private static readonly Dictionary<string, Entity> cash = new Dictionary<string, Entity>();
    }

    public delegate T EntityRef<T>() where T : Entity;
}
