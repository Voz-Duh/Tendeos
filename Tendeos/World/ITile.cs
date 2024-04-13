using Microsoft.Xna.Framework.Graphics;
using System;
using Tendeos.Inventory;
using Tendeos.Utils;
using Tendeos.World.Shadows;

namespace Tendeos.World
{
    public interface ITile : IItem, IShadowTile
    {
        bool Collision { get; }
        float Health { get; }
        byte Hardness { get; }
        int DataCount { get; }
        IItem Drop { get; }
        Range DropCount { get; }

        void Changed(bool top, IMap map, int x, int y, TileData data);
        void Update(IMap map, int x, int y, TileData data);
        void Start(bool top, IMap map, int x, int y, TileData data);
        void Loaded(bool top, IMap map, int x, int y, TileData data);
        void Destroy(bool top, IMap map, int x, int y, TileData data);
        void Draw(SpriteBatch spriteBatch, bool top, IMap map, int x, int y, Vec2 drawPosition, TileData data);
    }
}
