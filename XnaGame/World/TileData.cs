using XnaGame.World.Content;

namespace XnaGame.World
{
    public struct TileData
    {
        public float Health { get; set; }
        public readonly byte[] Data { get; }
        public byte this[int i]
        {
            get => Data[i];
            set => Data[i] = value;
        }
        public bool IsReference { get; set; }
        public ITile Tile { get; init; }

        public TileData()
        {
            Health = 0;
            Tile = null;
        }

        public TileData(ITile tile) : this()
        {
            if (tile == null) return;
            if (tile is ReferenceTile)
            {
                IsReference = true;
                Data = new byte[8];
            }
            else
            {
                IsReference = false;
                Health = tile.Health;
                Data = tile.DataCount == 0 ? null : new byte[tile.DataCount];
            }
            Tile = tile;
        }
    }
}