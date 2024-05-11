using Microsoft.Xna.Framework.Content;
using Tendeos.Physical.Content;
using Tendeos.World;
using Tendeos.World.Generation;
using Tendeos.World.Structures;

namespace Tendeos.Content
{
    public static class Biomes
    {
        public static Cave strangeCave;
        public static Biome hills, test;

        public static void Init(ContentManager content)
        {
            strangeCave = new Cave()
            {
                CornersPower = 0.4f,
                Height = 15..,
                ChunksWidth = 2..3,
                ChunksHeight = 2..3,
                SpawnChance = 0.1f,
                Tile = Tiles.deep_stone,
                SmallCavesPower = 0.1f,
                BigCavesPower = 0.99f,
                GiganticCavesPower = 0.1f,
            };
            hills = new Biome()
            {
                UndegroundTile = Tiles.stone,
                GroundTile = Tiles.dirt,
                Grounds = new[]
                {
                    (Tiles.dirt, Tiles.grass)
                },
                GroundHeight = 10,
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
                GroundHeight = 0,
                HillsHeight = 4,
                SmallCavesPower = 0.45f,
                BigCavesPower = 0.83f,
                GiganticCavesPower = 0.3f,
                Caves = new Cave[]
                {
                    strangeCave
                },
                Enemies = new EnemyBuilder[]
                {
                    Entities.dummy
                },
                Structures = new Structure[]
                {
                    Structures.test
                }
            };
        }

        public static BaseBiome Get(string value) => (BaseBiome)typeof(Biomes).GetField(value).GetValue(null);
    }
}
