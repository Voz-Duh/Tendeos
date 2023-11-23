using XnaGame.Utils;

namespace XnaGame.WorldMap
{
    public interface IMap
    {
        bool TryPlaceTile(ITile tile, int x, int y);
        bool TryPlaceTile(ITile tile, (int x, int y) position);
        bool PlaceTile(ITile tile, int x, int y);
        bool PlaceTile(ITile tile, (int x, int y) position);
        bool TrySetTile(ITile tile, int x, int y);
        bool TrySetTile(ITile tile, (int x, int y) position);
        void SetTile(ITile tile, int x, int y);
        void SetTile(ITile tile, (int x, int y) position);
        TileData GetTile(int x, int y);
        TileData GetTile((int x, int y) position);
        (int x, int y) World2Cell(float x, float y);
        (int x, int y) World2Cell(FVector2 position);
        FVector2 Cell2World(int x, int y);
        FVector2 Cell2World((int x, int y) position);
    }
}
