using XnaGame.Content;
using XnaGame.Utils;

namespace XnaGame.World
{
    public class WorldGenerator
    {
        public ITile GetTile(int x, int y)
        {
            return y > 20 && Noise.Perlin(5, x/5f, y/5f) > .5 ? Tiles.test : null;
        }

        public ITile GetWalls(int x, int y)
        {
            return y > 19 && Noise.Perlin(5, x/5f, y/5f) > .4 ? Tiles.test : null;
        }

        public bool GetWater(int x, int y)
        {
            return false;
        }
    }
}
