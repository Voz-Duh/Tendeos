using XnaGame.Utils;

namespace XnaGame.World
{
    public interface IMap
    {
        int FullWidth { get; }
        int FullHeight { get; }
        bool TryPlaceTile(bool top, ITile tile, int x, int y);
        bool TryPlaceTile(bool top, ITile tile, (int x, int y) position);
        bool PlaceTile(bool top, ITile tile, int x, int y);
        bool PlaceTile(bool top, ITile tile, (int x, int y) position);
        bool TrySetTile(bool top, ITile tile, int x, int y);
        bool TrySetTile(bool top, ITile tile, (int x, int y) position);
        void MineTile(bool top, int x, int y, float power);
        void MineTile(bool top, (int x, int y) position, float power);
        void MineTile(bool top, int x, int y, float power, float radius);
        void MineTile(bool top, (int x, int y) position, float power, float radius);
        void SetTile(bool top, ITile tile, int x, int y);
        void SetTile(bool top, ITile tile, (int x, int y) position);
        TileData GetTile(bool top, int x, int y);
        TileData GetTile(bool top, (int x, int y) position);
        (int x, int y) World2Cell(float x, float y);
        (int x, int y) World2Cell(Vec2 position);
        Vec2 Cell2World(int x, int y);
        Vec2 Cell2World((int x, int y) position);
    }
}
