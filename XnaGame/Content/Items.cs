using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
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
            bow = new Bow(Entities.Get<Projectile>("projectile"), (-1, 7), -1f, 1, Sprite.Load(content, "bow").Split(5, 1, 1), Sprite.Load(content, "bow_item"));
            pickaxe = new Pickaxe(1, 1, 65, 2, 2, 6, 16, 2, 3, Sprite.Load(content, "pickaxe"), Sprite.Load(content, "pickaxe_item"));
            pickaxeSword = new MeleeWeapon(65, 5, 2, 6, 16, 1f, 3, Sprite.Load(content, "pickaxe"), Sprite.Load(content, "pickaxe_item"));
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

        private static Dictionary<string, IItem> cash = new Dictionary<string, IItem>();
    }
}
