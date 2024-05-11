using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Tendeos.World
{
    public interface IChunk
    {
        BaseBiome Biome { get; set; }
        List<Rectangle> AirQuadtree { get; }
        void SetTile(bool type, int x, int y, TileData tileData);
        ref TileData GetTile(bool type, int x, int y);
        ref TileData SetGetTile(bool type, int x, int y, TileData tileData);
        Rectangle? GetTileQuadtree(IMap map, int cx, int cy, int x, int y);
    }
}