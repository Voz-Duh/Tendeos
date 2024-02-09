using XnaGame.Inventory;
using XnaGame.Utils;
using XnaGame.World.Shadows;

namespace XnaGame.World
{
    public interface ITile : IItem, IShadowTile
    {
        float Health { get; }
        byte Hardness { get; }
        bool Wallable { get; }
        void Changed(bool top, IMap map, int x, int y, TileData data);
        void Update(IMap map, int x, int y, TileData data);
        void Start(bool top, IMap map, int x, int y, TileData data);
        void Draw(bool top, IMap map, int x, int y, Vec2 drawPosition, float angle, TileData data);
        byte[] GetData();
    }
}
