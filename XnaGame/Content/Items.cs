using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Reflection;
using XnaGame.Inventory;
using XnaGame.Inventory.Content;
using XnaGame.PEntities.Content;
using XnaGame.Utils.Graphics;

namespace XnaGame.Content
{
    public static class Items
    {
        public static MeleeWeapon pickaxeSword;
        public static Pickaxe pickaxe;
        public static Bow bow;

        public static void Init(ContentManager content)
        {
            bow = new Bow(Sprite.Load(content, "bow").Split(5, 1, 1), Sprite.Load(content, "bow_item"))
            {
                Projectile = Entities.Get<Projectile>("projectile"),
                ArrowOffset = (-1, 7),
                Offset = -1,
                Power = 1
            };
            pickaxe = new Pickaxe(Sprite.Load(content, "pickaxe"), Sprite.Load(content, "pickaxe_item"))
            {
                Power = 1,
                Radius = 1,
                SwingAngle = 65,
                SwingPerSecond = 2,
                State = 2,
                Offset = 6,
                AttackOffset = 16,
                Damage = 2,
                AttackRange = 3
            };
            pickaxeSword = new MeleeWeapon( Sprite.Load(content, "pickaxe"), Sprite.Load(content, "pickaxe_item"))
            {
                SwingAngle = 65,
                SwingPerSecond = 5,
                State = 2,
                Offset = 6,
                AttackOffset = 16,
                Damage = 1,
                AttackRange = 3
            };
        }

        public static Func<T> Get<T>(string value) where T : IItem
        {
            FieldInfo t = typeof(Items).GetField(value);
            return () =>
            {
                if (cash.TryGetValue(value, out IItem entity))
                    return (T)entity;
                T res = (T)t.GetValue(null);
                cash.Add(value, res);
                return res;
            };
        }

        private static readonly Dictionary<string, IItem> cash = new Dictionary<string, IItem>();
    }

    public delegate IItem ItemRef();
}
