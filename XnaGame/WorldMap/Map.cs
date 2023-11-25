using Rectangle = Microsoft.Xna.Framework.Rectangle;
using nkast.Aether.Physics2D.Dynamics;
using System.Collections.Generic;
using XnaGame.Utils;
using System;

namespace XnaGame.WorldMap
{
    public class Map : IMap
    {
        public const float tileSize = 8;
        public const int chunkSize = 8;

        private readonly int mapWidth, mapHeight;
        public readonly Chunk[,] chunks;
        private readonly Body body;

        public Map(World world, int mapWidth, int mapHeight, Func<int, int, ITile> generator)
        {
            body = new Body();
            body.Tag = this;
            world.Add(body);

            this.mapWidth = mapWidth;
            this.mapHeight = mapHeight;
            chunks = new Chunk[mapWidth, mapHeight];
            
            int i, j;
            for (i = 0; i < mapWidth; i++)
                for (j = 0; j < mapHeight; j++)
                    chunks[i, j] = new Chunk(i, j, generator);
            for (i = 0; i < mapWidth; i++)
                for (j = 0; j < mapHeight; j++)
                    chunks[i, j].Start(this, body, i, j);
        }

        public void Update()
        {
            int j;
            for (int i = 0; i < mapWidth; i++)
            {
                for (j = 0; j < mapHeight; j++)
                {
                    chunks[i, j].Update(this, i, j);
                }
            }
        }

        public void Draw()
        {
            int j;
            for (int i = 0; i < mapWidth; i++)
            {
                for (j = 0; j < mapHeight; j++)
                {
                    chunks[i, j].Draw(this, i, j);
                }
            }
        }

        public (int x, int y) World2Cell(float x, float y) => ((int)(x / tileSize), (int)(y / tileSize));

        public (int x, int y) World2Cell(FVector2 position) => World2Cell(position.X, position.Y);

        public FVector2 Cell2World(int x, int y) => new FVector2(x * tileSize, y * tileSize);

        public FVector2 Cell2World((int x, int y) position) => Cell2World(position.x, position.y);

        private (int x, int y) Cell2Chunk(int x, int y) => (x / chunkSize, y / chunkSize);

        public TileData GetTile(int x, int y)
        {
            if (x < 0) return default;
            else if (x >= chunkSize * mapWidth) return default;
            else if (y < 0) return default;
            else if (y >= chunkSize * mapHeight) return default;

            var chunk = Cell2Chunk(x, y);
            return chunks[chunk.x, chunk.y][x - chunk.x * chunkSize, y - chunk.y * chunkSize];
        }

        public TileData GetTile((int x, int y) position) => GetTile(position.x, position.y);

        public void SetTile(ITile tile, int x, int y)
        {
            if (x < 0) return;
            else if (x >= chunkSize * mapWidth) return;
            else if (y < 0) return;
            else if (y >= chunkSize * mapHeight) return;

            var chunk = Cell2Chunk(x, y);

            Chunk c = chunks[chunk.x, chunk.y];
            c[x-chunk.x*chunkSize, y-chunk.y*chunkSize] = new TileData(tile);
            tile.Start(this, x, y, c[x-chunk.x*chunkSize, y-chunk.y*chunkSize]);
            
            c.UpdateColision(body, chunk.x, chunk.y);

            TileData data = GetTile(x + 1, y);
            data.Tile?.Changed(this, x + 1, y, data);

            data = GetTile(x - 1, y);
            data.Tile?.Changed(this, x - 1, y, data);

            data = GetTile(x, y + 1);
            data.Tile?.Changed(this, x, y + 1, data);

            data = GetTile(x, y - 1);
            data.Tile?.Changed(this, x, y - 1, data);
        }

        public void SetTile(ITile tile, (int x, int y) position) => SetTile(tile, position.x, position.y);

        public bool TrySetTile(ITile tile, int x, int y)
        {
            if (GetTile(x, y).Tile == null)
            {
                SetTile(tile, x, y);
                return true;
            }
            return false;
        }
        public bool TrySetTile(ITile tile, (int x, int y) position) => TrySetTile(tile, position.x, position.y);

        public bool PlaceTile(ITile tile, int x, int y)
        {
            if (GetTile(x+1, y).Tile != null ||
                GetTile(x-1, y).Tile != null ||
                GetTile(x, y+1).Tile != null ||
                GetTile(x, y-1).Tile != null)
            {
                SetTile(tile, x, y);
                return true;
            }
            return false;
        }

        public bool PlaceTile(ITile tile, (int x, int y) position) => PlaceTile(tile, position.x, position.y);

        public bool TryPlaceTile(ITile tile, int x, int y)
        {
            if (GetTile(x, y).Tile == null &&
                (GetTile(x + 1, y).Tile != null ||
                 GetTile(x - 1, y).Tile != null ||
                 GetTile(x, y + 1).Tile != null ||
                 GetTile(x, y - 1).Tile != null))
            {
                SetTile(tile, x, y);
                return true;
            }
            return false;
        }

        public bool TryPlaceTile(ITile tile, (int x, int y) position) => TryPlaceTile(tile, position.x, position.y);

        public class Chunk
        {
            public Fixture[] fixtures;
            private TileData[,] tiles;

            public Chunk(int x, int y, Func<int, int, ITile> generator)
            {
                tiles = new TileData[chunkSize, chunkSize];
                int j;
                for (int i = 0; i < chunkSize; i++)
                    for (j = 0; j < chunkSize; j++)
                        tiles[i, j] = new TileData(generator(x*chunkSize+i, y*chunkSize+j));
                fixtures = new Fixture[0];
            }

            public TileData this[int x, int y]
            {
                get => tiles[x, y];
                set => tiles[x, y] = value;
            }

            public void Start(IMap map, Body body, int x, int y)
            {
                int j;
                for (int i = 0; i < chunkSize; i++)
                    for (j = 0; j < chunkSize; j++)
                    {
                        TileData tile = tiles[i, j];
                        tile.Tile?.Start(map, x * chunkSize + i, y * chunkSize + j, tile);
                    }
                UpdateColision(body, x, y);
            }

            public void Update(IMap map, int x, int y)
            {
                int j;
                for (int i = 0; i < chunkSize; i++)
                    for (j = 0; j < chunkSize; j++)
                    {
                        TileData tile = tiles[i, j];
                        tile.Tile?.Update(map, x*chunkSize+i, y*chunkSize+j, tile);
                    }
            }

            public void UpdateColision(Body body, int x, int y)
            {
                Queue<Rectangle> rectangles = new Queue<Rectangle>();
                rectangles.Enqueue(new Rectangle(0, 0, chunkSize, chunkSize));
                Queue<Rectangle> result = new Queue<Rectangle>();
                
                int j, i;
                while (rectangles.Count != 0)
                {
                    Rectangle rectangle = rectangles.Dequeue();
                    for (i = rectangle.X; i < rectangle.X + rectangle.Width; i++)
                        for (j = rectangle.Y; j < rectangle.Y + rectangle.Height; j++)
                            if (tiles[i, j].Tile == null)
                            {
                                if (rectangle.Width > 1)
                                {
                                    int w = rectangle.Width / 2,
                                        h = rectangle.Height / 2;
                                    rectangles.Enqueue(new Rectangle(rectangle.X, rectangle.Y, w, h));
                                    rectangles.Enqueue(new Rectangle(rectangle.X + w, rectangle.Y, w, h));
                                    rectangles.Enqueue(new Rectangle(rectangle.X, rectangle.Y + h, w, h));
                                    rectangles.Enqueue(new Rectangle(rectangle.X + w, rectangle.Y + h, w, h));
                                }
                                goto SKIP;
                            }
                    result.Enqueue(rectangle);
                    SKIP:;
                }

                fixtures = new Fixture[result.Count];
                while (result.Count != 0)
                {
                    Rectangle rectangle = result.Dequeue();
                    fixtures[result.Count] = body.CreateRectangle(
                            rectangle.Width * tileSize, rectangle.Height * tileSize,
                            1,
                            new FVector2(
                                x * chunkSize + rectangle.X + rectangle.Width / 2f,
                                y * chunkSize + rectangle.Y + rectangle.Height / 2f
                                ) * tileSize);
                }
            }

            public void Draw(IMap map, int x, int y)
            {
                int j;
                for (int i = 0; i < chunkSize; i++)
                {
                    for (j = 0; j < chunkSize; j++)
                    {
                        TileData tile = tiles[i, j];
                        tile.Tile?.Draw(map, x*chunkSize+i, y*chunkSize+j, new FVector2(x*chunkSize+i, y*chunkSize+j)*tileSize, tile);
                    }
                }
            }
        }
    }
}
