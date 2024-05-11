using System;
using Tendeos.Utils;

namespace Tendeos.World.Generation
{
    public class Cave : BaseBiome
    {
        private static readonly Noise defaultCornersNoise = Noise.CPerlin(0.3f, 10);

        public ITile Tile;
        public float SpawnChance;
        public float CornersPower;
        public Noise CornersNoise = defaultCornersNoise;
        public Range ChunksWidth, ChunksHeight, Height;
    }
}