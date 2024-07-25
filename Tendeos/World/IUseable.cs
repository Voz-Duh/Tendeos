using Tendeos.Physical.Content;

namespace Tendeos.World
{
    public interface IUseable
    {
        void Use(IMap map, ref TileData data, Player player);
        void Unuse(IMap map, ref TileData data, Player player);
    }
}