using Microsoft.Xna.Framework.Content;
using XnaGame.World;
using XnaGame.World.Content;

namespace XnaGame.Content
{
    public static class Tiles
    {
        public static ITile ignore, reference, test, stone, dirt, grass, tree;

        public static void Init(ContentManager content)
        {
            ignore = new TileTag();
            reference = new ReferenceTile();
            test = new AutoTile()
            {
                Hardness = 1,
                Health = 2
            };
            stone = new AutoTile()
            {
                Hardness = 2,
                Health = 4
            };
            dirt = new AutoTile()
            {
                Hardness = 1,
                Health = 2
            };
            grass = new AutoTile()
            {
                Hardness = 1,
                Health = 2,
                DropTag = "dirt"
            };
            tree = new Tree("dirt")
            {
                Hardness = 1,
                Health = 2
            };
        }

        public static ITile Get(string value) => value == "air" ? default : (ITile)typeof(Tiles).GetField(value).GetValue(null);
    }
}
