using Microsoft.Xna.Framework.Content;
using XnaGame.World.Generation;

namespace XnaGame.Content
{
    public static class Biomes
    {
        public static Biome hills, test;

        public static void Init(ContentManager content)
        {
            hills = new Biome()
            {
                UndegroundTile = Tiles.stone,
                GroundTile = Tiles.dirt,
                Grounds = new[]
                {
                    (Tiles.dirt, Tiles.grass)
                },
                GroundHeight = 60,
                HillsHeight = 10,
                TreeChance = 0.1f,
                Tree = Tiles.tree,
            };
            test = new Biome()
            {
                UndegroundTile = Tiles.test,
                GroundTile = Tiles.test,
                GroundHeight = 50,
                HillsHeight = 4
            };
        }

        public static Biome Get(string value) => (Biome)typeof(Biomes).GetField(value).GetValue(null);
    }
}
