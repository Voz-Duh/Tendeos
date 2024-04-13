using System;
using Tendeos.Physical.Content;
using Tendeos.World.Structures;

namespace Tendeos.World.Generation
{
    public class Biome
    {
        public string Tag { get; set; }
        public ITile GroundTile { get; set; }
        public ITile UndegroundTile { get; set; }
        public float BigCavesPower { get; set; }
        public float SmallCavesPower { get; set; }
        public Cave[] Caves { get; set; }
        public Structure[] Structures { get; set; }
        public (ITile from, ITile to)[] Grounds { get; set; }
        public float GroundHeight { get; set; }
        public float HillsHeight { get; set; }
        public float TreeChance { get; set; }
        public ITile Tree { get; set; }
        public EnemyBuilder[] Enemies { get; set; } = Array.Empty<EnemyBuilder>();
    }
}