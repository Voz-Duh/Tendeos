using XnaGame.World.Structures;

namespace XnaGame.World.Generation
{
    public class Biome
    {
        public string Name { get; set; }
        public ITile GroundTile { get; set; }
        public ITile UndegroundTile { get; set; }
        public Cave CustomCave { get; set; }
        public Structure[] Structures { get; set; }
        public (ITile from, ITile to)[] Grounds { get; set; }
        public float GroundHeight { get; set; }
        public float HillsHeight { get; set; }
        public float TreeChance { get; set; }
        public ITile Tree { get; set; }
    }
}