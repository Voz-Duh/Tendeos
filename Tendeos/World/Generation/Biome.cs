
namespace Tendeos.World.Generation
{
    public class Biome : BaseBiome
    {
        public ITile GroundTile, UndegroundTile;
        public Cave[] Caves;
        public float GroundHeight, HillsHeight;
        public float TreeChance;
        public ITile Tree;
    }
}