using Microsoft.Xna.Framework.Graphics;
using System;
using Tendeos.Inventory;
using Tendeos.UI.GUIElements;
using Tendeos.Utils;
using Tendeos.World.Shadows;

namespace Tendeos.World
{
    public interface ITile : IItem, IShadowTile
    {
        bool Collision { get; }
        float Health { get; }
        byte Hardness { get; }
        IItem Drop { get; }
        Range DropCount { get; }
        object RealInterface { get; set; }
        TileInterface Interface { get; set; }

        public void SetCraftMenu(CraftMenu craftMenu)
        {
            unsafe
            {
                RealInterface = craftMenu;
                Interface = TileInterface.CraftMenu;
            }
        }
        public void SetInventory(Inventory.Inventory inventory)
        {
            unsafe
            {
                RealInterface = inventory;
                Interface = TileInterface.Inventory;
            }
        }
        void Changed(bool top, IMap map, int x, int y, ref TileData data);
        void Start(bool top, IMap map, int x, int y, ref TileData data);
        void Loaded(bool top, IMap map, int x, int y, ref TileData data);
        void Destroy(bool top, IMap map, int x, int y, TileData data);
        void Draw(SpriteBatch spriteBatch, bool top, IMap map, int x, int y, Vec2 drawPosition, TileData data);
    }
}
