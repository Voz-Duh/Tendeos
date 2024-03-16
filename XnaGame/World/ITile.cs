using Microsoft.Xna.Framework.Graphics;
using System;
using XnaGame.Inventory;
using XnaGame.Utils;
using XnaGame.World.Shadows;

namespace XnaGame.World
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
