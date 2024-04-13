using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System.Reflection;
using Tendeos.Inventory;
using Tendeos.Inventory.Content;
using Tendeos.Modding;
using Tendeos.World;

namespace Tendeos.Content
{
    public static class Items
    {
        public static MeleeWeapon pickaxeSword;
        public static Pickaxe pickaxe;
        public static Bow bow;

        public static void Init(ContentManager content)
        {
            bow = new Bow()
            {
                Projectile = "arrow",
                ArrowOffset = (-1, 7),
                Offset = -1,
                Power = 1
            };
            pickaxe = new Pickaxe()
            {
                Power = 1,
                Radius = 1,
                SwingAngle = 65,
                SwingPerSecond = 20,
                State = 2,
                Offset = 6,
                AttackOffset = 16,
                Damage = 2,
                AttackRange = 3
            };
            /*
            pickaxeSword = new MeleeWeapon("pickaxe")
            {
                SwingAngle = 65,
                SwingPerSecond = 5,
                State = 2,
                Offset = 6,
                AttackOffset = 16,
                Damage = 1,
                AttackRange = 3
            };
            */
        }

        public static IItem Get(string value)
        {
            FieldInfo field = typeof(Items).GetField(value);
            if (field == null)
            {
                FieldInfo tileFiled = typeof(Tiles).GetField(value);
                if (tileFiled == null)
                {
                    foreach (Mod mod in Mods.Loaded.Values)
                    {
                        if (mod.Items.TryGetValue(value, out IModItem item)) return item;
                        if (mod.Tiles.TryGetValue(value, out IModTile tile)) return tile;
                    }
                    throw new KeyNotFoundException(value);
                }
                return (ITile)tileFiled.GetValue(null);
            }
            return (IItem)field.GetValue(null);
        }
    }
}
