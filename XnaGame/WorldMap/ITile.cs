using nkast.Aether.Physics2D.Dynamics;
using XnaGame.Inventory;
using XnaGame.Utils;
using XnaGame.WorldMap.Structures;

namespace XnaGame.WorldMap
{
    public interface ITile : IItem
    {
        float Health { get; }
        byte Hardness { get; }
        int Mass { get; }
        void Changed(IMap map, int x, int y, TileData data);
        void Update(ITiledBody body, IMap map, int x, int y, TileData data);
        void Start(IMap map, int x, int y, TileData data);
        void Draw(IMap map, int x, int y, FVector2 drawPosition, float angle, TileData data);
        byte[] GetData();
    }
}
