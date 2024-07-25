using System;
using Tendeos.Inventory;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;
using Tendeos.World.Shadows;

namespace Tendeos.World
{
    public interface ITile : IItem, IShadowTile, IUseable
    {
        bool Collision { get; }
        bool Multitile { get; }
        float Health { get; }
        byte Hardness { get; }
        IItem Drop { get; }
        Range DropCount { get; }
        ITileInterface Interface { get; set; }

        void Changed(bool top, IMap map, int x, int y, ref TileData data);
        void Start(bool top, IMap map, int x, int y, ref TileData data);
        void Loaded(bool top, IMap map, int x, int y, ref TileData data);
        void Destroy(bool top, IMap map, int x, int y, TileData data);
        void Draw(SpriteBatch spriteBatch, bool top, IMap map, int x, int y, Vec2 drawPosition, TileData data);
    }
}