using Microsoft.Xna.Framework.Content;
using Tendeos.Physical.Content;
using Tendeos.World.Generation;

namespace Tendeos.Content
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
                SmallCavesPower = 0.45f,
                BigCavesPower = 0.83f,
            };
            test = new Biome()
            {
                UndegroundTile = Tiles.test,
                GroundTile = Tiles.test,
                GroundHeight = 50,
                HillsHeight = 4,
                SmallCavesPower = 0.45f,
                BigCavesPower = 0.83f,
                Enemies = new EnemyBuilder[]
                {
                    Entities.dummy
                },
            };
        }

        public static Biome Get(string value) => (Biome)typeof(Biomes).GetField(value).GetValue(null);
    }
}
