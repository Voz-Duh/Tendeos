using Microsoft.Xna.Framework;
using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Dynamics;
using System;
using System.Collections.Generic;
using XnaGame.PEntities;
using XnaGame.PEntities.Content;
using XnaGame.Utils;
using XnaGame.Utils.Graphics;

namespace XnaGame.WorldMap.Structures
{
    public class BlockStructure : BodyTransform, IMap, ITiledBody
    {
        private readonly int mapWidth, mapHeight;
        private readonly int chunkSize;
        public readonly Chunk[,] chunks;

        public BlockStructure(World world, int mapWidth, int mapHeight, int chunkSize, Dictionary<(int, int), ITile> tiles) : base(new Body())
        {
            body.BodyType = BodyType.Dynamic;
            body.SleepingAllowed = false;
            body.Tag = this;
            world.Add(body);

            this.mapWidth = mapWidth;
            this.mapHeight = mapHeight;
            this.chunkSize = chunkSize;
            chunks = new Chunk[mapWidth, mapHeight];

            int i, j;
            for (i = 0; i < mapWidth; i++)
                for (j = 0; j < mapHeight; j++)
                    chunks[i, j] = new Chunk(chunkSize, i, j, tiles);
            for (i = 0; i < mapWidth; i++)
                for (j = 0; j < mapHeight; j++)
                    chunks[i, j].Start(this, body, chunkSize, i, j);
            UpdateMassCenter();
        }

        public void UpdateMassCenter()
        {
            FVector2 center = FVector2.Zero;
            int fullMass = 0, j;
            for (int i = 0; i < mapWidth; i++)
                for (j = 0; j < mapHeight; j++)
                {
                    var (c, m) = chunks[i, j].GetMassCenter(chunkSize);
                    center += c + new FVector2(i * chunkSize, j * chunkSize);
                    fullMass += m;
                }
            body.LocalCenter = center / fullMass * Map.tileSize;
            body.Mass = fullMass * Map.tileSize;
        }

        public void Update()
        {
            int j;
            for (int i = 0; i < mapWidth; i++)
            {
                for (j = 0; j < mapHeight; j++)
                {
                    chunks[i, j].Update(this, this, chunkSize, i, j);
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
                    chunks[i, j].Draw(this, this, chunkSize, i, j);
                }
            }
        }

        public (int x, int y) World2Cell(float x, float y) => World2Cell(new FVector2(x, y));

        public (int x, int y) World2Cell(FVector2 position)
        {
            position = World2Local(position) / Map.tileSize;
            return ((int)position.X, (int)position.Y);
        }

        public FVector2 Cell2World(int x, int y) => Cell2World((x, y));

        public FVector2 Cell2World((int x, int y) position) => Local2World(new FVector2(position.x + .5f, position.y + .5f) * Map.tileSize);

        private (int x, int y) Cell2Chunk(int x, int y) => (x / chunkSize, y / chunkSize);

        public void MineTile(int x, int y, float power)
        {
            if (x < 0) return;
            else if (x >= chunkSize * mapWidth) return;
            else if (y < 0) return;
            else if (y >= chunkSize * mapHeight) return;

            var chunk = Cell2Chunk(x, y);

            Chunk c = chunks[chunk.x, chunk.y];
            int lx = x - chunk.x * chunkSize,
                ly = y - chunk.y * chunkSize;
            if (c[lx, ly].Tile == null) return;
            float h = c.Mine(this, chunkSize, chunk.x, chunk.y, lx, ly, power);

            if (h > 0) return;

            c.UpdateColision(body, chunkSize, chunk.x, chunk.y);
            UpdateMassCenter();

            TileData data = GetTile(x + 1, y);
            data.Tile?.Changed(this, x + 1, y, data);

            data = GetTile(x - 1, y);
            data.Tile?.Changed(this, x - 1, y, data);

            data = GetTile(x, y + 1);
            data.Tile?.Changed(this, x, y + 1, data);

            data = GetTile(x, y - 1);
            data.Tile?.Changed(this, x, y - 1, data);
        }
        public void MineTile((int x, int y) position, float power) => MineTile(position.x, position.y, power);

        public void MineTile(int x, int y, float power, float radius)
        {
            for (int X = (int)(x - MathF.Floor(radius)); X <= x + MathF.Ceiling(radius); X++)
                for (int Y = (int)(y - MathF.Floor(radius)); Y <= y + MathF.Ceiling(radius); Y++)
                {
                    if (X < 0) continue;
                    else if (X >= chunkSize * mapWidth) continue;
                    else if (Y < 0) continue;
                    else if (Y >= chunkSize * mapHeight) continue;

                    var chunk = Cell2Chunk(X, Y);

                    Chunk c = chunks[chunk.x, chunk.y];
                    int lx = X - chunk.x * chunkSize,
                        ly = Y - chunk.y * chunkSize;
                    if (c[lx, ly].Tile == null) continue;
                    float h = c.Mine(this, chunkSize, chunk.x, chunk.y, lx, ly, power);

                    if (h > 0) continue;

                    c.UpdateColision(body, chunkSize, chunk.x, chunk.y);
                    UpdateMassCenter();

                    TileData data = GetTile(X + 1, Y);
                    data.Tile?.Changed(this, X + 1, Y, data);

                    data = GetTile(X - 1, Y);
                    data.Tile?.Changed(this, X - 1, Y, data);

                    data = GetTile(X, Y + 1);
                    data.Tile?.Changed(this, X, Y + 1, data);

                    data = GetTile(X, Y - 1);
                    data.Tile?.Changed(this, X, Y - 1, data);
                }
        }
        public void MineTile((int x, int y) position, float power, float radius) => MineTile(position.x, position.y, power, radius);

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
            tile?.Start(this, x, y, c[x - chunk.x * chunkSize, y - chunk.y * chunkSize] = new TileData(tile));

            c.UpdateColision(body, chunkSize, chunk.x, chunk.y);
            UpdateMassCenter();

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
            if (x < 0) return false;
            else if (x >= chunkSize * mapWidth) return false;
            else if (y < 0) return false;
            else if (y >= chunkSize * mapHeight) return false;

            var chunk = Cell2Chunk(x, y);

            Chunk c = chunks[chunk.x, chunk.y];
            int lx = x - chunk.x * chunkSize,
                ly = y - chunk.y * chunkSize;
            if (c[lx, ly].Tile != null) return false;
            tile?.Start(this, x, y, c[lx, ly] = new TileData(tile));

            c.UpdateColision(body, chunkSize, chunk.x, chunk.y);
            UpdateMassCenter();

            TileData data = GetTile(x + 1, y);
            data.Tile?.Changed(this, x + 1, y, data);

            data = GetTile(x - 1, y);
            data.Tile?.Changed(this, x - 1, y, data);

            data = GetTile(x, y + 1);
            data.Tile?.Changed(this, x, y + 1, data);

            data = GetTile(x, y - 1);
            data.Tile?.Changed(this, x, y - 1, data);
            return true;
        }
        public bool TrySetTile(ITile tile, (int x, int y) position) => TrySetTile(tile, position.x, position.y);

        public bool PlaceTile(ITile tile, int x, int y)
        {
            if (x < 0) return false;
            else if (x >= chunkSize * mapWidth) return false;
            else if (y < 0) return false;
            else if (y >= chunkSize * mapHeight) return false;

            TileData
                r = GetTile(x + 1, y),
                l = GetTile(x - 1, y),
                d = GetTile(x, y + 1),
                u = GetTile(x, y - 1);
            if (r.Tile != null ||
                l.Tile != null ||
                d.Tile != null ||
                u.Tile != null)
            {
                var chunk = Cell2Chunk(x, y);

                Chunk c = chunks[chunk.x, chunk.y];
                tile?.Start(this, x, y, c[x - chunk.x * chunkSize, y - chunk.y * chunkSize] = new TileData(tile));

                c.UpdateColision(body, chunkSize, chunk.x, chunk.y);
                UpdateMassCenter();

                r.Tile?.Changed(this, x + 1, y, r);
                l.Tile?.Changed(this, x - 1, y, l);
                d.Tile?.Changed(this, x, y + 1, d);
                u.Tile?.Changed(this, x, y - 1, u);
                return true;
            }
            return false;
        }

        public bool PlaceTile(ITile tile, (int x, int y) position) => PlaceTile(tile, position.x, position.y);

        public bool TryPlaceTile(ITile tile, int x, int y)
        {
            if (x < 0) return false;
            else if (x >= chunkSize * mapWidth) return false;
            else if (y < 0) return false;
            else if (y >= chunkSize * mapHeight) return false;

            TileData
                r = GetTile(x + 1, y),
                l = GetTile(x - 1, y),
                d = GetTile(x, y + 1),
                u = GetTile(x, y - 1);
            if (r.Tile != null ||
                l.Tile != null ||
                d.Tile != null ||
                u.Tile != null)
            {
                var chunk = Cell2Chunk(x, y);

                Chunk c = chunks[chunk.x, chunk.y];
                int lx = x - chunk.x * chunkSize,
                    ly = y - chunk.y * chunkSize;
                if (c[lx, ly].Tile != null) return false;
                tile?.Start(this, x, y, c[lx, ly] = new TileData(tile));

                c.UpdateColision(body, chunkSize, chunk.x, chunk.y);
                UpdateMassCenter();

                r.Tile?.Changed(this, x + 1, y, r);
                l.Tile?.Changed(this, x - 1, y, l);
                d.Tile?.Changed(this, x, y + 1, d);
                u.Tile?.Changed(this, x, y - 1, u);
                return true;
            }
            return false;
        }

        public bool TryPlaceTile(ITile tile, (int x, int y) position) => TryPlaceTile(tile, position.x, position.y);

        public void AddForce(int x, int y, FVector2 force, ForceType type, bool local = true)
        {
            if (local) force = body.GetWorldVector(force);
            if (type == ForceType.Pushing) force *= Time.Delta;
            body.ApplyForce(force, Cell2World(x, y));
        }

        public class Chunk
        {
            public Fixture[] fixtures;
            private TileData[,] tiles;

            public Chunk(int chunkSize, int x, int y, Dictionary<(int, int), ITile> tiles)
            {
                this.tiles = new TileData[chunkSize, chunkSize];
                int j;
                for (int i = 0; i < chunkSize; i++)
                    for (j = 0; j < chunkSize; j++)
                    {
                        int tx = x * chunkSize + i,
                            ty = y * chunkSize + j;
                        this.tiles[i, j] = tiles.ContainsKey((tx, ty)) ? new TileData(tiles[(tx, ty)]) : default;
                    }
                fixtures = new Fixture[0];
            }

            public TileData this[int x, int y]
            {
                get => tiles[x, y];
                set => tiles[x, y] = value;
            }

            public void Start(IMap map, Body body, int chunkSize, int x, int y)
            {
                int j;
                for (int i = 0; i < chunkSize; i++)
                    for (j = 0; j < chunkSize; j++)
                    {
                        TileData tile = tiles[i, j];
                        tile.Tile?.Start(map, x * chunkSize + i, y * chunkSize + j, tile);
                    }
                UpdateColision(body, chunkSize, x, y);
            }

            public void Update(ITiledBody body, IMap map, int chunkSize, int x, int y)
            {
                int j;
                for (int i = 0; i < chunkSize; i++)
                    for (j = 0; j < chunkSize; j++)
                    {
                        TileData tile = tiles[i, j];
                        tile.Tile?.Update(body, map, x * chunkSize + i, y * chunkSize + j, tile);
                    }
            }

            public void UpdateColision(Body body, int chunkSize, int x, int y)
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

                foreach (var fixture in fixtures) body.Remove(fixture);

                fixtures = new Fixture[result.Count];
                while (result.Count != 0)
                {
                    Rectangle rectangle = result.Dequeue();
                    fixtures[result.Count] = body.CreateRectangle(
                            rectangle.Width * Map.tileSize, rectangle.Height * Map.tileSize,
                            1,
                            new FVector2(
                                x * chunkSize + rectangle.X + rectangle.Width / 2f,
                                y * chunkSize + rectangle.Y + rectangle.Height / 2f
                                ) * Map.tileSize);
                }
            }

            public void Draw(ITransform transform, IMap map, int chunkSize, int x, int y)
            {
                int j;
                for (int i = 0; i < chunkSize; i++)
                {
                    for (j = 0; j < chunkSize; j++)
                    {
                        TileData tile = tiles[i, j];
                        tile.Tile?.Draw(map, x * chunkSize + i, y * chunkSize + j, transform.Local2World(new FVector2(x * chunkSize + i, y * chunkSize + j) * Map.tileSize), transform.Local2World(0), tile);
                    }
                }
            }

            public float Mine(ITransform transform, int chunkSize, int cx, int cy, int x, int y, float power)
            {
                float h = tiles[x, y].Health -= power;
                if (h <= 0)
                {
                    new Item((tiles[x, y].Tile, 1), transform.Local2World(new FVector2((cx * chunkSize + x + 0.5f) * Map.tileSize, (cy * chunkSize + y + 0.5f) * Map.tileSize)));
                    tiles[x, y] = default;
                }
                return h;
            }

            public (FVector2, int) GetMassCenter(int chunkSize)
            {
                FVector2 mass = FVector2.Zero;
                int fullMass = 0, j;
                for (int i = 0; i < chunkSize; i++)
                    for (j = 0; j < chunkSize; j++)
                        if (tiles[i, j].Tile != null)
                        {
                            int m = tiles[i, j].Tile.Mass;
                            fullMass += m;
                            mass += new FVector2(i, j) * m;
                        }
                return (mass, fullMass);
            }
        }
    }
}
