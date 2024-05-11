using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Numerics;
using Tendeos.Content;
using Tendeos.Physical.Content;
using Tendeos.Utils;
using Tendeos.World.Liquid;

namespace Tendeos.World
{
    public class Map : IMap
    {
        private static TileData NullTileReference = default;
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
                {
                    chunks[i, j].Start(this, i, j);
                    chunks[i, j].ComputeAirQuadtree(this, i, j);
                }
        }

        public void CameraViewSet()
        {
            widthRender = (int)Math.Ceiling(camera.WorldViewport.X / (ChunkSize * TileSize)) + 1;
            heightRender = (int)Math.Ceiling(camera.WorldViewport.Y / (ChunkSize * TileSize)) + 1;
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

        public (int x, int y) Cell2Chunk(int x, int y) => (x / ChunkSize, y / ChunkSize);

        public (int x, int y) Cell2Chunk((int x, int y) position) => (position.x / ChunkSize, position.y / ChunkSize);

        public (int x, int y) World2Cell(float x, float y) => ((int)(x / TileSize), (int)(y / TileSize));

        public (int x, int y) World2Cell(Vec2 position) => World2Cell(position.X, position.Y);

        public Vec2 Cell2World(int x, int y) => new Vec2(x + .5f, y + .5f) * TileSize;

        public Vec2 Cell2World((int x, int y) position) => Cell2World(position.x, position.y);

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
            if (c.GetTile(top, lx, ly).Tile == null) return;
            float h = c.Mine(top, chunk.x, chunk.y, lx, ly, power, this);

            if (h > 0) return;

            if (top)
            {
                waterWorld.cells[x, y] = 0;
                c.ComputeAirQuadtree(this, chunk.x, chunk.y);
            }

            ref TileData data = ref GetTile(top, x + 1, y);
            data.Tile?.Changed(top, this, x + 1, y, ref data);

            data = ref GetTile(top, x - 1, y);
            data.Tile?.Changed(top, this, x - 1, y, ref data);

            data = ref GetTile(top, x, y + 1);
            data.Tile?.Changed(top, this, x, y + 1, ref data);

            data = ref GetTile(top, x, y - 1);
            data.Tile?.Changed(top, this, x, y - 1, ref data);
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
                    if (c.GetTile(top, lx, ly).Tile == null) continue;
                    float h = c.Mine(top, chunk.x, chunk.y, lx, ly, power, this);

                    if (h > 0) continue;

                    if (top)
                    {
                        waterWorld.cells[X, Y] = 0;
                        c.ComputeAirQuadtree(this, chunk.x, chunk.y);
                    }

                    ref TileData data = ref GetTile(top, X + 1, Y);
                    data.Tile?.Changed(top, this, X + 1, Y, ref data);

                    data = ref GetTile(top, X - 1, Y);
                    data.Tile?.Changed(top, this, X - 1, Y, ref data);

                    data = ref GetTile(top, X, Y + 1);
                    data.Tile?.Changed(top, this, X, Y + 1, ref data);

                    data = ref GetTile(top, X, Y - 1);
                    data.Tile?.Changed(top, this, X, Y - 1, ref data);
                }
        }
        public void MineTile(bool top, (int x, int y) position, float power, float radius) => MineTile(top, position.x, position.y, power, radius);

        public ref TileData GetUnrefTile(bool top, int x, int y)
        {
            if (x < 0) return ref NullTileReference;
            else if (x >= FullWidth) return ref NullTileReference;
            else if (y < 0) return ref NullTileReference;
            else if (y >= FullHeight) return ref NullTileReference;

            var chunk = Cell2Chunk(x, y);
            if (chunks == null || chunks[chunk.x, chunk.y] == null) return ref NullTileReference;
            ref TileData data = ref chunks[chunk.x, chunk.y].GetTile(top, x - chunk.x * ChunkSize, y - chunk.y * ChunkSize);
            if (data.IsReference)
            {
                return ref GetTile(top, (int)data.GetU32(0), (int)data.GetU32(32));
            }
            return ref data;
        }

        public ref TileData GetUnrefTile(bool top, (int x, int y) position) => ref GetUnrefTile(top, position.x, position.y);

        public ref TileData GetTile(bool top, int x, int y)
        {
            if (x < 0) return ref NullTileReference;
            else if (x >= FullWidth) return ref NullTileReference;
            else if (y < 0) return ref NullTileReference;
            else if (y >= FullHeight) return ref NullTileReference;

            var chunk = Cell2Chunk(x, y);
            if (chunks == null || chunks[chunk.x, chunk.y] == null) return ref NullTileReference;
            return ref chunks[chunk.x, chunk.y].GetTile(top, x - chunk.x * ChunkSize, y - chunk.y * ChunkSize);
        }

        public ref TileData GetTile(bool top, (int x, int y) position) => ref GetTile(top, position.x, position.y);

        public void SetTile(bool top, ITile tile, int x, int y)
        {
            if (tile == Ignore) return;

            if (x < 0) return;
            else if (x >= FullWidth) return;
            else if (y < 0) return;
            else if (y >= FullHeight) return;

            var chunk = Cell2Chunk(x, y);

            Chunk c = chunks[chunk.x, chunk.y];
            ref TileData data = ref c.SetGetTile(top, x - chunk.x * ChunkSize, y - chunk.y * ChunkSize, new TileData(tile));
            tile?.Start(top, this, x, y, ref data);

            if (top)
            {
                waterWorld.cells[x, y] = tile == null ? 0 : -1;
                c.ComputeAirQuadtree(this, chunk.x, chunk.y);
            }

            data = ref GetTile(top, x + 1, y);
            data.Tile?.Changed(top, this, x + 1, y, ref data);

            data = ref GetTile(top, x - 1, y);
            data.Tile?.Changed(top, this, x - 1, y, ref data);

            data = ref GetTile(top, x, y + 1);
            data.Tile?.Changed(top, this, x, y + 1, ref data);

            data = ref GetTile(top, x, y - 1);
            data.Tile?.Changed(top, this, x, y - 1, ref data);
        }

        public void SetTile(bool top, ITile tile, (int x, int y) position) => SetTile(top, tile, position.x, position.y);

        public bool TrySetTile(bool top, ITile tile, int x, int y)
        {
            if (tile == Ignore) return false;

            if (x < 0) return false;
            else if (x >= FullWidth) return false;
            else if (y < 0) return false;
            else if (y >= FullHeight) return false;

            var chunk = Cell2Chunk(x, y);

            Chunk c = chunks[chunk.x, chunk.y];
            int lx = x - chunk.x * ChunkSize,
                ly = y - chunk.y * ChunkSize;
            if (c.GetTile(top, lx, ly).Tile != null) return false;
            ref TileData data = ref c.SetGetTile(top, lx, ly, new TileData(tile));
            tile?.Start(top, this, x, y, ref data);

            if (top)
            {
                waterWorld.cells[x, y] = tile == null ? 0 : -1;
                c.ComputeAirQuadtree(this, chunk.x, chunk.y);
            }

            data = ref GetTile(top, x + 1, y);
            data.Tile?.Changed(top, this, x + 1, y, ref data);

            data = ref GetTile(top, x - 1, y);
            data.Tile?.Changed(top, this, x - 1, y, ref data);

            data = ref GetTile(top, x, y + 1);
            data.Tile?.Changed(top, this, x, y + 1, ref data);

            data = ref GetTile(top, x, y - 1);
            data.Tile?.Changed(top, this, x, y - 1, ref data);
            return true;
        }
        public bool TrySetTile(bool top, ITile tile, (int x, int y) position) => TrySetTile(top, tile, position.x, position.y);

        public bool CanSetTile(bool top, int x, int y)
        {
            if (x < 0) return false;
            else if (x >= FullWidth) return false;
            else if (y < 0) return false;
            else if (y >= FullHeight) return false;

            var chunk = Cell2Chunk(x, y);

            Chunk c = chunks[chunk.x, chunk.y];
            return c.GetTile(top, x - chunk.x * ChunkSize, y - chunk.y * ChunkSize).Tile == null;
        }
        public bool CanSetTile(bool top, (int x, int y) position) => CanSetTile(top, position.x, position.y);

        public bool PlaceTile(bool top, ITile tile, int x, int y)
        {
            if (tile == Ignore) return false;

            if (x < 0) return false;
            else if (x >= FullWidth) return false;
            else if (y < 0) return false;
            else if (y >= FullHeight) return false;

            ref TileData
                r = ref GetTile(top, x + 1, y),
                l = ref GetTile(top, x - 1, y),
                d = ref GetTile(top, x, y + 1),
                u = ref GetTile(top, x, y - 1);
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
                ref TileData data = ref c.SetGetTile(top, x - chunk.x * ChunkSize, y - chunk.y * ChunkSize, new TileData(tile));
                tile?.Start(top, this, x, y, ref data);

                if (top)
                {
                    waterWorld.cells[x, y] = tile == null ? 0 : -1;
                    c.ComputeAirQuadtree(this, chunk.x, chunk.y);
                }

                r.Tile?.Changed(top, this, x + 1, y, ref r);
                l.Tile?.Changed(top, this, x - 1, y, ref l);
                d.Tile?.Changed(top, this, x, y + 1, ref d);
                u.Tile?.Changed(top, this, x, y - 1, ref u);
                return true;
            }
            return false;
        }

        public bool PlaceTile(bool top, ITile tile, (int x, int y) position) => PlaceTile(top, tile, position.x, position.y);

        public void SetTileData<T>(bool top, int x, int y, Func<TileData, TileData> action) where T : ITile 
        {
            ref TileData data = ref GetTile(top, x, y);
            if (data.Tile is not T) return;
            TileData tempData = action(data);
            data.Tile?.Changed(top, this, x, y, ref tempData);
            data.data = tempData.data;

            data = ref GetTile(top, x + 1, y);
            data.Tile?.Changed(top, this, x + 1, y, ref data);

            data = ref GetTile(top, x - 1, y);
            data.Tile?.Changed(top, this, x - 1, y, ref data);

            data = ref GetTile(top, x, y + 1);
            data.Tile?.Changed(top, this, x, y + 1, ref data);

            data = ref GetTile(top, x, y - 1);
            data.Tile?.Changed(top, this, x, y - 1, ref data);
        }
        
        public void SetTileData<T>(bool top, (int x, int y) position, Func<TileData, TileData> action) where T : ITile =>
            SetTileData<T>(top, position.x, position.y, action);

        public bool TryPlaceTile(bool top, ITile tile, int x, int y)
        {
            if (tile == Ignore) return false;

            if (x < 0) return false;
            else if (x >= FullWidth) return false;
            else if (y < 0) return false;
            else if (y >= FullHeight) return false;

            ref TileData
                r = ref GetTile(top, x + 1, y),
                l = ref GetTile(top, x - 1, y),
                d = ref GetTile(top, x, y + 1),
                u = ref GetTile(top, x, y - 1);
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
                if (c.GetTile(top, lx, ly).Tile != null) return false;
                ref TileData data = ref c.SetGetTile(top, lx, ly, new TileData(tile));
                tile?.Start(top, this, x, y, ref data);

                if (top)
                {
                    waterWorld.cells[x, y] = tile == null ? 0 : -1;
                    c.ComputeAirQuadtree(this, chunk.x, chunk.y);
                }

                r.Tile?.Changed(top, this, x + 1, y, ref r);
                l.Tile?.Changed(top, this, x - 1, y, ref l);
                d.Tile?.Changed(top, this, x, y + 1, ref d);
                u.Tile?.Changed(top, this, x, y - 1, ref u);
                return true;
            }
            return false;
        }

        public bool TryPlaceTile(bool top, ITile tile, (int x, int y) position) => TryPlaceTile(top, tile, position.x, position.y);

        public IChunk GetChunk(int x, int y) => chunks[x, y];

        public Rectangle? GetTileQuadtree(int x, int y)
        {
            if (x < 0) return null;
            else if (x >= FullWidth) return null;
            else if (y < 0) return null;
            else if (y >= FullHeight) return null;

            var chunk = Cell2Chunk(x, y);
            return chunks[chunk.x, chunk.y].GetTileQuadtree(this, chunk.x, chunk.y, x, y);
        }

        public IChunk GetTileChunk(int x, int y)
        {
            var chunk = Cell2Chunk(x, y);
            return chunks[chunk.x, chunk.y];
        }

        public void ToByte(ByteBuffer buffer)
        {
            List<ITile> tiles = new List<ITile>();
            ref TileData data = ref NullTileReference;
            ITile tile;
            int cx, cy, x, y;
            for (cx = 0; cx < Width; cx++)
                for (cy = 0; cy < Height; cy++)
                    for (x = 0; x < ChunkSize; x++)
                        for (y = 0; y < ChunkSize; y++)
                        {
                            data = ref chunks[cx, cy].GetTile(true, x, y);
                            if (data.IsReference) data = ref NullTileReference;
                            if (!tiles.Contains(data.Tile)) tiles.Add(data.Tile);
                            data = ref chunks[cx, cy].GetTile(false, x, y);
                            if (data.IsReference) data = ref NullTileReference;
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
                buffer.Append(tile?.Tag ?? "air");
            }
            for (cx = 0; cx < Width; cx++)
                for (cy = 0; cy < Height; cy++)
                {
                    buffer.Append(chunks[cx, cy].Biome.Tag);
                    for (x = 0; x < ChunkSize; x++)
                        for (y = 0; y < ChunkSize; y++)
                        {
                            data = chunks[cx, cy].GetTile(true, x, y);
                            buffer.Append(data.IsReference);
                            if (!data.IsReference)
                            {
                                buffer.Append(tiles.IndexOf(data.Tile));
                                if (data.Tile != null)
                                {
                                    buffer.Append(data.Health);
                                    buffer.Append(((BigInteger)data.data).ToByteArray());
                                }
                            }
                            data = chunks[cx, cy].GetTile(false, x, y);
                            buffer.Append(data.IsReference);
                            if (!data.IsReference)
                            {
                                buffer.Append(tiles.IndexOf(data.Tile));
                                if (data.Tile != null)
                                {
                                    buffer.Append(data.Health);
                                    buffer.Append(((BigInteger)data.data).ToByteArray());
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
                                chunks[cx, cy].SetTile(true, x, y, new TileData(null));
                            else
                            {
                                tile = tiles[buffer.ReadInt()];
                                chunks[cx, cy].SetTile(true, x, y, data = new TileData(tile)
                                {
                                    IsReference = false,
                                    Health = tile == null ? 0 : buffer.ReadFloat(),
                                });
                                data.data = (UInt128)new BigInteger(buffer.Read(16));
                            }
                            reference = buffer.ReadBool();
                            if (reference)
                                chunks[cx, cy].SetTile(false, x, y, new TileData(null));
                            else
                            {
                                tile = tiles[buffer.ReadInt()];
                                chunks[cx, cy].SetTile(false, x, y, data = new TileData(tile)
                                {
                                    IsReference = false,
                                    Health = tile == null ? 0 : buffer.ReadFloat(),
                                });
                                data.data = (UInt128)new BigInteger(buffer.Read(16));
                            }
                        }
                }
            for (cx = 0; cx < Width; cx++)
                for (cy = 0; cy < Height; cy++)
                    chunks[cx, cy].Loaded(this, cx, cy);
        }

        public class Chunk : IChunk
        {
            public BaseBiome Biome { get; set; }
            public List<Rectangle> AirQuadtree { get; }
            private readonly TileData[,] tiles, walls;

            public void SetTile(bool type, int x, int y, TileData tileData) => (type? tiles : walls)[x, y] = tileData;
            public ref TileData GetTile(bool type, int x, int y) => ref (type? tiles : walls)[x, y];
            public ref TileData SetGetTile(bool type, int x, int y, TileData tileData)
            {
                (type? tiles : walls)[x, y] = tileData;
                return ref (type? tiles : walls)[x, y];
            }

            public Chunk(int chunkSize)
            {
                tiles = new TileData[chunkSize, chunkSize];
                walls = new TileData[chunkSize, chunkSize];
                AirQuadtree = new List<Rectangle>();
            }

            public void Start(IMap map, int x, int y)
            {
                int j;
                for (int i = 0; i < map.ChunkSize; i++)
                    for (j = 0; j < map.ChunkSize; j++)
                    {
                        ref TileData tile = ref walls[i, j];
                        tile.Tile?.Start(false, map, x * map.ChunkSize + i, y * map.ChunkSize + j, ref tile);
                        tile = tiles[i, j];
                        tile.Tile?.Start(true, map, x * map.ChunkSize + i, y * map.ChunkSize + j, ref tile);
                    }
            }

            public void Loaded(IMap map, int x, int y)
            {
                int j;
                for (int i = 0; i < map.ChunkSize; i++)
                    for (j = 0; j < map.ChunkSize; j++)
                    {
                        ref TileData tile = ref walls[i, j];
                        tile.Tile?.Loaded(false, map, x * map.ChunkSize + i, y * map.ChunkSize + j, ref tile);
                        tile = tiles[i, j];
                        tile.Tile?.Loaded(true, map, x * map.ChunkSize + i, y * map.ChunkSize + j, ref tile);
                    }
            }

            public Rectangle? GetTileQuadtree(IMap map, int cx, int cy, int x, int y)
            {
                cx *= map.ChunkSize;
                cy *= map.ChunkSize;
                int t;
                foreach (Rectangle rectangle in AirQuadtree)
                {
                    if ((t = cx + rectangle.X) >= x && t + rectangle.Width < x &&
                        (t = cy + rectangle.Y) >= x && t + rectangle.Height < y)
                        return rectangle;
                }
                return null;
            }

            public void ComputeAirQuadtree(IMap map, int x, int y)
            {
                int j, i;

                AirQuadtree.Clear();
                Queue<Rectangle> rectangles = new Queue<Rectangle>();
                rectangles.Enqueue(new Rectangle(0, 0, map.ChunkSize, map.ChunkSize));

                int w, h;
                Rectangle rectangle;
                while (rectangles.Count != 0)
                {
                    rectangle = rectangles.Dequeue();
                    for (i = rectangle.X; i < rectangle.X + rectangle.Width; i++)
                        for (j = rectangle.Y; j < rectangle.Y + rectangle.Height; j++)
                            if (tiles[i, j].Tile != null)
                            {
                                if (rectangle.Width > 1)
                                {
                                    w = rectangle.Width / 2;
                                    h = rectangle.Height / 2;
                                    rectangles.Enqueue(new Rectangle(rectangle.X, rectangle.Y, w, h));
                                    rectangles.Enqueue(new Rectangle(rectangle.X + w, rectangle.Y, w, h));
                                    rectangles.Enqueue(new Rectangle(rectangle.X, rectangle.Y + h, w, h));
                                    rectangles.Enqueue(new Rectangle(rectangle.X + w, rectangle.Y + h, w, h));
                                }
                                goto SKIP;
                            }
                    AirQuadtree.Add(rectangle);
                    SKIP:;
                }
            }

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
                ref TileData data = ref (type ? tiles : walls)[x, y];
                if (data.IsReference)
                {
                    map.MineTile(type, (int)data.GetU32(0), (int)data.GetU32(32), power);
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
