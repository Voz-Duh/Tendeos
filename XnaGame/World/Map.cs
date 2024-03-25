using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using XnaGame.Content;
using XnaGame.Physical.Content;
using XnaGame.Utils;
using XnaGame.World.Generation;
using XnaGame.World.Liquid;

namespace XnaGame.World
{
    public class Map : IMap
    {
        private readonly WaterWorld waterWorld;
        public int Width { get; private set; }
        public int Height { get; private set; }
        private readonly Camera camera;
        private Chunk[,] chunks;
        public ITile Ignore { get; set; }

        public int ChunkSize { get; private set; }
        public float TileSize { get; private set; }
        public int FullWidth => Width * ChunkSize;
        public int FullHeight => Height * ChunkSize;

        private int widthRender, heightRender;

        public Map(int mapWidth, int mapHeight, WaterWorld waterWorld, Camera camera, int chunkSize = 8, float tileSize = 8)
        {
            Width = mapWidth;
            Height = mapHeight;
            this.camera = camera;
            ChunkSize = chunkSize;
            TileSize = tileSize;
            chunks = new Chunk[mapWidth, mapHeight];

            this.waterWorld = waterWorld;

            int i, j;
            for (i = 0; i < mapWidth; i++)
                for (j = 0; j < mapHeight; j++)
                    chunks[i, j] = new Chunk(ChunkSize);
            for (i = 0; i < mapWidth; i++)
                for (j = 0; j < mapHeight; j++)
                    chunks[i, j].Start(this, i, j);
        }

        public void CameraViewSet()
        {
            widthRender = (int)Math.Ceiling(camera.WorldViewport.X / (ChunkSize * TileSize)) + 1;
            heightRender = (int)Math.Ceiling(camera.WorldViewport.Y / (ChunkSize * TileSize)) + 1;
        }

        public void Update()
        {
            int xRender = (int)Math.Floor((camera.Position.X - camera.WorldViewport.X / 2) / (ChunkSize * TileSize)) - 3;
            int yRender = (int)Math.Floor((camera.Position.Y - camera.WorldViewport.Y / 2) / (ChunkSize * TileSize)) - 3;

            int j;
            for (int i = Math.Clamp(xRender, 0, Width - 1); i < Math.Clamp(xRender + widthRender + 3, 0, Width); i++)
                for (j = Math.Clamp(yRender, 0, Height - 1); j < Math.Clamp(yRender + heightRender + 3, 0, Height); j++)
                {
                    chunks[i, j].Update(this, i, j);
                }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int xRender = (int)Math.Floor((camera.Position.X - camera.WorldViewport.X / 2) / (ChunkSize * TileSize));
            int yRender = (int)Math.Floor((camera.Position.Y - camera.WorldViewport.Y / 2) / (ChunkSize * TileSize));

            int j;
            for (int i = Math.Clamp(xRender, 0, Width - 1); i < Math.Clamp(xRender + widthRender, 0, Width); i++)
                for (j = Math.Clamp(yRender, 0, Height - 1); j < Math.Clamp(yRender + heightRender, 0, Height); j++)
                {
                    chunks[i, j].Draw(spriteBatch, this, i, j);
                }
        }


        public float Flow(Liquid.Liquid liquid, float power, int x, int y)
        {
            if (x < 0) return power;
            else if (x >= FullWidth) return power;
            else if (y < 0) return power;
            else if (y >= FullHeight) return power;

            if (waterWorld.cells[x, y] == -1) return power;
            waterWorld.cells[x, y] = power;

            return 0;
        }
        public float Flow(Liquid.Liquid liquid, float power, (int x, int y) position) => Flow(liquid, power, position.x, position.y);

        public (int x, int y) World2Cell(float x, float y) => ((int)(x / TileSize), (int)(y / TileSize));

        public (int x, int y) World2Cell(Vec2 position) => World2Cell(position.X, position.Y);

        public Vec2 Cell2World(int x, int y) => new Vec2(x + .5f, y + .5f) * TileSize;

        public Vec2 Cell2World((int x, int y) position) => Cell2World(position.x, position.y);

        private (int x, int y) Cell2Chunk(int x, int y) => (x / ChunkSize, y / ChunkSize);

        public void MineTile(bool top, int x, int y, float power)
        {
            if (x < 0) return;
            else if (x >= ChunkSize * Width) return;
            else if (y < 0) return;
            else if (y >= ChunkSize * Height) return;

            var chunk = Cell2Chunk(x, y);

            Chunk c = chunks[chunk.x, chunk.y];
            int lx = x - chunk.x * ChunkSize,
                ly = y - chunk.y * ChunkSize;
            if (c[top, lx, ly].Tile == null) return;
            float h = c.Mine(top, chunk.x, chunk.y, lx, ly, power, this);

            if (h > 0) return;

            if (top) waterWorld.cells[x, y] = 0;

            TileData data = GetTile(top, x + 1, y);
            data.Tile?.Changed(top, this, x + 1, y, data);

            data = GetTile(top, x - 1, y);
            data.Tile?.Changed(top, this, x - 1, y, data);

            data = GetTile(top, x, y + 1);
            data.Tile?.Changed(top, this, x, y + 1, data);

            data = GetTile(top, x, y - 1);
            data.Tile?.Changed(top, this, x, y - 1, data);
        }
        public void MineTile(bool top, (int x, int y) position, float power) => MineTile(top, position.x, position.y, power);

        public void MineTile(bool top, int x, int y, float power, float radius)
        {
            for (int X = (int)(x - MathF.Floor(radius)); X <= x + MathF.Ceiling(radius); X++)
                for (int Y = (int)(y - MathF.Floor(radius)); Y <= y + MathF.Ceiling(radius); Y++)
                {
                    if (X < 0) continue;
                    else if (X >= ChunkSize * Width) continue;
                    else if (Y < 0) continue;
                    else if (Y >= ChunkSize * Height) continue;

                    var chunk = Cell2Chunk(X, Y);

                    Chunk c = chunks[chunk.x, chunk.y];
                    int lx = X - chunk.x * ChunkSize,
                        ly = Y - chunk.y * ChunkSize;
                    if (c[top, lx, ly].Tile == null) continue;
                    float h = c.Mine(top, chunk.x, chunk.y, lx, ly, power, this);

                    if (h > 0) continue;

                    if (top) waterWorld.cells[X, Y] = 0;

                    TileData data = GetTile(top, X + 1, Y);
                    data.Tile?.Changed(top, this, X + 1, Y, data);

                    data = GetTile(top, X - 1, Y);
                    data.Tile?.Changed(top, this, X - 1, Y, data);

                    data = GetTile(top, X, Y + 1);
                    data.Tile?.Changed(top, this, X, Y + 1, data);

                    data = GetTile(top, X, Y - 1);
                    data.Tile?.Changed(top, this, X, Y - 1, data);
                }
        }
        public void MineTile(bool top, (int x, int y) position, float power, float radius) => MineTile(top, position.x, position.y, power, radius);

        public TileData GetTile(bool top, int x, int y)
        {
            if (x < 0) return default;
            else if (x >= ChunkSize * Width) return default;
            else if (y < 0) return default;
            else if (y >= ChunkSize * Height) return default;

            var chunk = Cell2Chunk(x, y);
            return chunks?[chunk.x, chunk.y]?[top, x - chunk.x * ChunkSize, y - chunk.y * ChunkSize] ?? default;
        }

        public TileData GetTile(bool top, (int x, int y) position) => GetTile(top, position.x, position.y);

        public void SetTile(bool top, ITile tile, int x, int y)
        {
            if (tile == Ignore) return;

            if (x < 0) return;
            else if (x >= ChunkSize * Width) return;
            else if (y < 0) return;
            else if (y >= ChunkSize * Height) return;

            var chunk = Cell2Chunk(x, y);

            Chunk c = chunks[chunk.x, chunk.y];
            TileData data = c[top, x - chunk.x * ChunkSize, y - chunk.y * ChunkSize] = new TileData(tile);
            tile?.Start(top, this, x, y, data);

            if (top) waterWorld.cells[x, y] = tile == null ? 0 : -1;

            data = GetTile(top, x + 1, y);
            data.Tile?.Changed(top, this, x + 1, y, data);

            data = GetTile(top, x - 1, y);
            data.Tile?.Changed(top, this, x - 1, y, data);

            data = GetTile(top, x, y + 1);
            data.Tile?.Changed(top, this, x, y + 1, data);

            data = GetTile(top, x, y - 1);
            data.Tile?.Changed(top, this, x, y - 1, data);
        }

        public void SetTile(bool top, ITile tile, (int x, int y) position) => SetTile(top, tile, position.x, position.y);

        public bool TrySetTile(bool top, ITile tile, int x, int y)
        {
            if (tile == Ignore) return false;

            if (x < 0) return false;
            else if (x >= ChunkSize * Width) return false;
            else if (y < 0) return false;
            else if (y >= ChunkSize * Height) return false;

            var chunk = Cell2Chunk(x, y);

            Chunk c = chunks[chunk.x, chunk.y];
            int lx = x - chunk.x * ChunkSize,
                ly = y - chunk.y * ChunkSize;
            if (c[top, lx, ly].Tile != null) return false;
            TileData data = c[top, lx, ly] = new TileData(tile);
            tile?.Start(top, this, x, y, data);

            if (top) waterWorld.cells[x, y] = tile == null ? 0 : -1;

            data = GetTile(top, x + 1, y);
            data.Tile?.Changed(top, this, x + 1, y, data);

            data = GetTile(top, x - 1, y);
            data.Tile?.Changed(top, this, x - 1, y, data);

            data = GetTile(top, x, y + 1);
            data.Tile?.Changed(top, this, x, y + 1, data);

            data = GetTile(top, x, y - 1);
            data.Tile?.Changed(top, this, x, y - 1, data);
            return true;
        }
        public bool TrySetTile(bool top, ITile tile, (int x, int y) position) => TrySetTile(top, tile, position.x, position.y);

        public bool CanSetTile(bool top, int x, int y)
        {
            if (x < 0) return false;
            else if (x >= ChunkSize * Width) return false;
            else if (y < 0) return false;
            else if (y >= ChunkSize * Height) return false;

            var chunk = Cell2Chunk(x, y);

            Chunk c = chunks[chunk.x, chunk.y];
            return c[top, x - chunk.x * ChunkSize, y - chunk.y * ChunkSize].Tile == null;
        }
        public bool CanSetTile(bool top, (int x, int y) position) => CanSetTile(top, position.x, position.y);

        public bool PlaceTile(bool top, ITile tile, int x, int y)
        {
            if (tile == Ignore) return false;

            if (x < 0) return false;
            else if (x >= ChunkSize * Width) return false;
            else if (y < 0) return false;
            else if (y >= ChunkSize * Height) return false;

            TileData
                r = GetTile(top, x + 1, y),
                l = GetTile(top, x - 1, y),
                d = GetTile(top, x, y + 1),
                u = GetTile(top, x, y - 1);
            if (r.Tile != null ||
                l.Tile != null ||
                d.Tile != null ||
                u.Tile != null ||
                GetTile(!top, x + 1, y).Tile != null ||
                GetTile(!top, x - 1, y).Tile != null ||
                GetTile(!top, x, y + 1).Tile != null ||
                GetTile(!top, x, y - 1).Tile != null ||
                GetTile(!top, x, y).Tile != null)
            {
                var chunk = Cell2Chunk(x, y);

                Chunk c = chunks[chunk.x, chunk.y];
                TileData data = c[top, x - chunk.x * ChunkSize, y - chunk.y * ChunkSize] = new TileData(tile);
                tile?.Start(top, this, x, y, data);

                if (top) waterWorld.cells[x, y] = tile == null ? 0 : -1;

                r.Tile?.Changed(top, this, x + 1, y, r);
                l.Tile?.Changed(top, this, x - 1, y, l);
                d.Tile?.Changed(top, this, x, y + 1, d);
                u.Tile?.Changed(top, this, x, y - 1, u);
                return true;
            }
            return false;
        }

        public bool PlaceTile(bool top, ITile tile, (int x, int y) position) => PlaceTile(top, tile, position.x, position.y);

        public bool TryPlaceTile(bool top, ITile tile, int x, int y)
        {
            if (tile == Ignore) return false;

            if (x < 0) return false;
            else if (x >= ChunkSize * Width) return false;
            else if (y < 0) return false;
            else if (y >= ChunkSize * Height) return false;

            TileData
                r = GetTile(top, x + 1, y),
                l = GetTile(top, x - 1, y),
                d = GetTile(top, x, y + 1),
                u = GetTile(top, x, y - 1);
            if (r.Tile != null ||
                l.Tile != null ||
                d.Tile != null ||
                u.Tile != null ||
                GetTile(!top, x + 1, y).Tile != null ||
                GetTile(!top, x - 1, y).Tile != null ||
                GetTile(!top, x, y + 1).Tile != null ||
                GetTile(!top, x, y - 1).Tile != null ||
                GetTile(!top, x, y).Tile != null)
            {
                var chunk = Cell2Chunk(x, y);

                Chunk c = chunks[chunk.x, chunk.y];
                int lx = x - chunk.x * ChunkSize,
                    ly = y - chunk.y * ChunkSize;
                if (c[top, lx, ly].Tile != null) return false;
                TileData data = c[top, lx, ly] = new TileData(tile);
                tile?.Start(top, this, x, y, data);

                if (top) waterWorld.cells[x, y] = tile == null ? 0 : -1;

                r.Tile?.Changed(top, this, x + 1, y, r);
                l.Tile?.Changed(top, this, x - 1, y, l);
                d.Tile?.Changed(top, this, x, y + 1, d);
                u.Tile?.Changed(top, this, x, y - 1, u);
                return true;
            }
            return false;
        }

        public bool TryPlaceTile(bool top, ITile tile, (int x, int y) position) => TryPlaceTile(top, tile, position.x, position.y);

        public IChunk GetChunk(int x, int y) => chunks[x, y];

        public IChunk GetTileChunk(int x, int y)
        {
            var chunk = Cell2Chunk(x, y);
            return chunks[chunk.x, chunk.y];
        }

        public void ToByte(ByteBuffer buffer)
        {
            List<ITile> tiles = new List<ITile>();
            TileData data;
            ITile tile;
            int cx, cy, x, y;
            for (cx = 0; cx < Width; cx++)
                for (cy = 0; cy < Height; cy++)
                    for (x = 0; x < ChunkSize; x++)
                        for (y = 0; y < ChunkSize; y++)
                        {
                            data = chunks[cx, cy][true, x, y];
                            if (data.IsReference) data = default;
                            if (!tiles.Contains(data.Tile)) tiles.Add(data.Tile);
                            data = chunks[cx, cy][false, x, y];
                            if (data.IsReference) data = default;
                            if (!tiles.Contains(data.Tile)) tiles.Add(data.Tile);
                        }
            buffer.Append(Width);
            buffer.Append(Height);
            buffer.Append(TileSize);
            buffer.Append(ChunkSize);
            buffer.Append(tiles.Count);
            for (int i = 0; i < tiles.Count; i++)
            {
                tile = tiles[i];
                buffer.Append(tile?.Name ?? "air");
            }
            for (cx = 0; cx < Width; cx++)
                for (cy = 0; cy < Height; cy++)
                {
                    buffer.Append(chunks[cx, cy].Biome.Name);
                    for (x = 0; x < ChunkSize; x++)
                        for (y = 0; y < ChunkSize; y++)
                        {
                            data = chunks[cx, cy][true, x, y];
                            buffer.Append(data.IsReference);
                            if (!data.IsReference)
                            {
                                buffer.Append(tiles.IndexOf(data.Tile));
                                if (data.Tile != null)
                                {
                                    buffer.Append(data.Health);
                                    for (int i = 0; i < data.Tile.DataCount; i++) buffer.Append(data[0]);
                                }
                            }
                            data = chunks[cx, cy][false, x, y];
                            buffer.Append(data.IsReference);
                            if (!data.IsReference)
                            {
                                buffer.Append(tiles.IndexOf(data.Tile));
                                if (data.Tile != null)
                                {
                                    buffer.Append(data.Health);
                                    for (int i = 0; i < data.Tile.DataCount; i++) buffer.Append(data[0]);
                                }
                            }
                        }
                }
        }

        public void FromByte(ByteBuffer buffer)
        {
            Width = buffer.ReadInt();
            Height = buffer.ReadInt();
            TileSize = buffer.ReadFloat();
            ChunkSize = buffer.ReadInt();
            int cx, cy, x, y;
            cy = buffer.ReadInt();
            ITile[] tiles = new ITile[cy];
            for (cx = 0; cx < cy; cx++)
                tiles[cx] = Tiles.Get(buffer.ReadString());
            bool reference;
            TileData data;
            ITile tile;
            chunks = new Chunk[Width, Height];
            for (cx = 0; cx < Width; cx++)
                for (cy = 0; cy < Height; cy++)
                {
                    Chunk chunk = new Chunk(ChunkSize) { Biome = Biomes.Get(buffer.ReadString()) };
                    chunks[cx, cy] = chunk;
                    for (x = 0; x < ChunkSize; x++)
                        for (y = 0; y < ChunkSize; y++)
                        {
                            reference = buffer.ReadBool();
                            if (reference)
                                chunks[cx, cy][true, x, y] = new TileData(null);
                            else
                            {
                                tile = tiles[buffer.ReadInt()];
                                chunks[cx, cy][true, x, y] = data = new TileData(tile)
                                {
                                    IsReference = false,
                                    Health = tile == null ? 0 : buffer.ReadFloat(),
                                };
                                if (data.Tile != null) for (int i = 0; i < data.Tile.DataCount; i++) data[i] = buffer.ReadByte();
                            }
                            reference = buffer.ReadBool();
                            if (reference)
                                chunks[cx, cy][false, x, y] = new TileData(null);
                            else
                            {
                                tile = tiles[buffer.ReadInt()];
                                chunks[cx, cy][false, x, y] = data = new TileData(tile)
                                {
                                    IsReference = false,
                                    Health = tile == null ? 0 : buffer.ReadFloat(),
                                };
                                if (data.Tile != null) for (int i = 0; i < data.Tile.DataCount; i++) data[i] = buffer.ReadByte();
                            }
                        }
                }
            for (cx = 0; cx < Width; cx++)
                for (cy = 0; cy < Height; cy++)
                    chunks[cx, cy].Loaded(this, cx, cy);
        }

        public class Chunk : IChunk
        {
            public Biome Biome { get; set; }
            private readonly TileData[,] tiles, walls;

            public TileData this[bool type, int x, int y]
            {
                get => (type ? tiles : walls)[x, y];
                set => (type ? tiles : walls)[x, y] = value;
            }

            public Chunk(int chunkSize)
            {
                tiles = new TileData[chunkSize, chunkSize];
                walls = new TileData[chunkSize, chunkSize];
            }

            public void Start(IMap map, int x, int y)
            {
                int j;
                for (int i = 0; i < map.ChunkSize; i++)
                    for (j = 0; j < map.ChunkSize; j++)
                    {
                        TileData tile = walls[i, j];
                        tile.Tile?.Start(false, map, x * map.ChunkSize + i, y * map.ChunkSize + j, tile);
                        tile = tiles[i, j];
                        tile.Tile?.Start(true, map, x * map.ChunkSize + i, y * map.ChunkSize + j, tile);
                    }
            }

            public void Loaded(IMap map, int x, int y)
            {
                int j;
                for (int i = 0; i < map.ChunkSize; i++)
                    for (j = 0; j < map.ChunkSize; j++)
                    {
                        TileData tile = walls[i, j];
                        tile.Tile?.Loaded(false, map, x * map.ChunkSize + i, y * map.ChunkSize + j, tile);
                        tile = tiles[i, j];
                        tile.Tile?.Loaded(true, map, x * map.ChunkSize + i, y * map.ChunkSize + j, tile);
                    }
            }

            public void Update(IMap map, int x, int y)
            {
                int j;
                for (int i = 0; i < map.ChunkSize; i++)
                    for (j = 0; j < map.ChunkSize; j++)
                    {
                        TileData tile = tiles[i, j];
                        if (!tile.IsReference)
                            tile.Tile?.Update(map, x * map.ChunkSize + i, y * map.ChunkSize + j, tile);
                    }
            }
            /*
            public void UpdateColision(Body body, int x, int y)
            {
                int j, i;

                foreach (var fixture in fixtures) body.Remove(fixture);

                Queue<Rectangle> rectangles = new Queue<Rectangle>();
                rectangles.Enqueue(new Rectangle(0, 0, chunkSize, chunkSize));
                Queue<Rectangle> result = new Queue<Rectangle>();

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
                    Fixture fixture = body.CreateRectangle(
                            rectangle.Width * tileSize, rectangle.Height * tileSize,
                            1,
                            new FVector2(
                                x * chunkSize + rectangle.X + rectangle.Width / 2f,
                                y * chunkSize + rectangle.Y + rectangle.Height / 2f
                                ) * tileSize);
                    fixture.CollisionCategories = Category.Cat1;
                    fixture.Friction = 0;
                    fixtures[result.Count] = fixture;
                }
            }
            */

            public void Draw(SpriteBatch spriteBatch, IMap map, int x, int y)
            {
                int j;
                TileData tile;
                for (int i = 0; i < map.ChunkSize; i++)
                    for (j = 0; j < map.ChunkSize; j++)
                    {
                        tile = walls[i, j];
                        if (!tile.IsReference)
                            tile.Tile?.Draw(spriteBatch, false, map, x * map.ChunkSize + i, y * map.ChunkSize + j, new Vec2(x * map.ChunkSize + i + 0.5f, y * map.ChunkSize + j + 0.5f) * map.TileSize, tile);
                        tile = tiles[i, j];
                        if (!tile.IsReference)
                            tile.Tile?.Draw(spriteBatch, true, map, x * map.ChunkSize + i, y * map.ChunkSize + j, new Vec2(x * map.ChunkSize + i + 0.5f, y * map.ChunkSize + j + 0.5f) * map.TileSize, tile);
                    }
            }

            public float Mine(bool type, int cx, int cy, int x, int y, float power, IMap map)
            {
                TileData data;
                if ((data = (type ? tiles : walls)[x, y]).IsReference)
                {
                    map.MineTile(type, BitConverter.ToInt32(data.Data), BitConverter.ToInt32(data.Data, 4), power);
                    return 1;
                }
                float h = (type ? tiles : walls)[x, y].Health -= power;
                if (h <= 0)
                {
                    ITile tile = (type ? tiles : walls)[x, y].Tile;
                    tile.Destroy(type, map, cx * map.ChunkSize + x, cy * map.ChunkSize + y, data);
                    if (tile.Drop == null) return h;
                    int i = URandom.SInt(tile.DropCount.Start.Value, tile.DropCount.End.Value);
                    if (i > 0) new Item((tile.Drop, i), new Vec2(cx * map.ChunkSize + x + 0.5f, cy * map.ChunkSize + y + 0.5f) * map.TileSize);
                    (type ? tiles : walls)[x, y] = default;
                }
                return h;
            }
        }
    }
}
