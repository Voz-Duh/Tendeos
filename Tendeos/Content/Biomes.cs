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

        public static void Init()
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
                    Entities.zombie_0, Entities.zombie_1, Entities.zombie_2, Entities.zombie_3, Entities.zombie_4, Entities.zombie_5, Entities.zombie_6, Entities.zombie_7, Entities.zombie_8, Entities.zombie_9, Entities.zombie_10, Entities.zombie_11, Entities.zombie_12, Entities.zombie_13, Entities.zombie_14, Entities.zombie_15, Entities.zombie_16, Entities.zombie_17, Entities.zombie_18, Entities.zombie_19, Entities.zombie_20, Entities.zombie_21, Entities.zombie_22, Entities.zombie_23, Entities.zombie_24, Entities.zombie_25, Entities.zombie_26, Entities.zombie_27, Entities.zombie_28, Entities.zombie_29, Entities.zombie_30, Entities.zombie_31, Entities.zombie_32,
                },
                Structures = new Structure[]
                {
                    Structures.test
                }
            };
        }

        public static BaseBiome Get(string value) => (BaseBiome) typeof(Biomes).GetField(value).GetValue(null);
    }
}