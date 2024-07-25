using System.Collections.Generic;
using System.Reflection;
using Tendeos.Modding;
using Tendeos.UI.GUIElements;
using Tendeos.Utils;
using Tendeos.World;
using Tendeos.World.Content;

namespace Tendeos.Content
{
    public static class Tiles
    {
        public static ITile ignore, reference, test, stone, dirt, grass, tree, deep_stone, chest, door;

        public static void Init()
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
            chest = new Multitile()
            {
                Hardness = 1,
                Health = 8,
                References = new[]
                {
                    (1, 0),
                    (0, 1),
                    (1, 1)
                },
                Required = new[]
                {
                    (0, 2, true, false),
                    (1, 2, true, false)
                },
                DrawOffset = new Vec2(4, 4),
                Interface = new InventoryContainer(new InventoryContainer.Style(Core.SlotButtonStyle,
                    Core.LabelWindowStyle, 5, 5))
            };
            door = new EnterableTile()
            {
                Flipable = true,
                Hardness = 1,
                Health = 8,
                References = new[]
                {
                    (0, -1),
                    (0, -2),
                },
                Required = new[]
                {
                    (0, 1, true, false),
                    (0, -3, false, false)
                },
                DrawOffset = new Vec2(0, -8),
                OpenDrawOffset = new Vec2(-4, -8),
            };
        }

        public static ITile Get(string value)
        {
            if (value == "air") return null;
            FieldInfo field = typeof(Tiles).GetField(value);
            if (field == null)
            {
                foreach (Mod mod in Mods.Loaded.Values)
                    if (mod.Tiles.TryGetValue(value, out IModTile tile))
                        return tile;
                throw new KeyNotFoundException(value);
            }

            return (ITile) field.GetValue(null);
        }

        private static Dictionary<string, ITile> __字ΑβᚠՀჰժŁŁŊҨशϠթѬძЯʬɎяŁ_UNOVERRIDABLE__all__;

        public static Dictionary<string, ITile> All
        {
            get
            {
                if (__字ΑβᚠՀჰժŁŁŊҨशϠթѬძЯʬɎяŁ_UNOVERRIDABLE__all__ == null)
                {
                    __字ΑβᚠՀჰժŁŁŊҨशϠթѬძЯʬɎяŁ_UNOVERRIDABLE__all__ = new Dictionary<string, ITile> {{"air", null}};
                    foreach (FieldInfo field in typeof(Tiles).GetFields())
                    {
                        if (field.Name != "__字ΑβᚠՀჰժŁŁŊҨशϠթѬძЯʬɎяŁ_UNREADABLE_VAR__all__" && field.Name != "reference")
                            __字ΑβᚠՀჰժŁŁŊҨशϠթѬძЯʬɎяŁ_UNOVERRIDABLE__all__.Add(field.Name, (ITile) field.GetValue(null));
                    }

                    foreach (Mod mod in Mods.Loaded.Values)
                    foreach (var (key, tile) in mod.Tiles)
                    {
                        __字ΑβᚠՀჰժŁŁŊҨशϠթѬძЯʬɎяŁ_UNOVERRIDABLE__all__.Add(key, tile);
                    }
                }

                return __字ΑβᚠՀჰժŁŁŊҨशϠթѬძЯʬɎяŁ_UNOVERRIDABLE__all__;
            }
        }
    }
}