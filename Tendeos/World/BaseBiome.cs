using System;
using Tendeos.Physical.Content;
using Tendeos.World.Structures;

namespace Tendeos.World
{
    public class BaseBiome
    {
        private static readonly EnemyBuilder[] noEnemies = Array.Empty<EnemyBuilder>();

        public string Tag { get; set; }

        public (ITile from, ITile to)[] Grounds;
        public float BigCavesPower, SmallCavesPower, GiganticCavesPower;
        public EnemyBuilder[] Enemies = noEnemies;
        public Structure[] Structures;
    }
}
