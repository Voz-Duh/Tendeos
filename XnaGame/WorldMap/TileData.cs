namespace XnaGame.WorldMap
{
    public struct TileData
    {
        public byte Health { get; init; }
        private byte[] stateData;
        public byte this[int i]
        {
            get => stateData[i];
            set => stateData[i] = value;
        }
        public ITile Tile { get; init; }

        public TileData()
        {
            Health = 0;
            stateData = null;
            Tile = null;
        }

        public TileData(ITile tile) : this()
        {
            if (tile == null) return;
            Health = tile.Health;
            stateData = tile.GetData();
            Tile = tile;
        }
    }
}