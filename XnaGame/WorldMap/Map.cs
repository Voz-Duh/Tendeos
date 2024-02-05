using XnaGame.Utils;
using System;
using XnaGame.PEntities.Content;
using XnaGame.WorldMap.Liquid;

namespace XnaGame.WorldMap
{
    /// <summary>
    /// Called for tile when raycasting tilemap.
    /// </summary>
    /// <param name="data">the tile hit by the ray @param point the point of initial intersection</param>
    /// <param name="point">the point of intersection</param>
    /// <param name="normal">the normal vector at the point of intersection</param>
    /// <returns>0 to terminate, fraction to clip the ray for closest hit, 1 to continue</returns>
    public delegate void RayCastReportTileDelegate(TileData data, Vec2 point, Vec2 normal, float fraction);

    public class Map : IMap
    {
        public const float tileSize = 8;
        public const int chunkSize = 8;

        private WaterWorld waterWorld;
        private readonly int mapWidth, mapHeight;
        public readonly Chunk[,] chunks;

        public Map(Liquid.Liquid[] liquids, int mapWidth, int mapHeight, Func<int, int, ITile> generator)
        {
            this.mapWidth = mapWidth;
            this.mapHeight = mapHeight;
            chunks = new Chunk[mapWidth, mapHeight];

            waterWorld = new WaterWorld(chunkSize * mapWidth, chunkSize * mapHeight, liquids, (x, y) => generator(x, y) == null);

            int i, j;
            for (i = 0; i < mapWidth; i++)
                for (j = 0; j < mapHeight; j++)
                    chunks[i, j] = new Chunk(i, j, generator);
            for (i = 0; i < mapWidth; i++)
                for (j = 0; j < mapHeight; j++)
                    chunks[i, j].Start(this, i, j);
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

            waterWorld.Update();
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

            waterWorld.Draw();
        }


        public float Flow(Liquid.Liquid liquid, float power, int x, int y)
        {
            if (x < 0) return power;
            else if (x >= chunkSize * mapWidth) return power;
            else if (y < 0) return power;
            else if (y >= chunkSize * mapHeight) return power;

            if (waterWorld.cells[x, y] == -1) return power;
            waterWorld.cells[x, y] = power;

            return 0;
        }
        public float Flow(Liquid.Liquid liquid, float power, (int x, int y) position) => Flow(liquid, power, position.x, position.y);

        public (int x, int y) World2Cell(float x, float y) => ((int)(x / tileSize), (int)(y / tileSize));

        public (int x, int y) World2Cell(Vec2 position) => World2Cell(position.X, position.Y);

        public Vec2 Cell2World(int x, int y) => new Vec2(x + .5f, y + .5f) * tileSize;

        public Vec2 Cell2World((int x, int y) position) => Cell2World(position.x, position.y);

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
            float h = c.Mine(chunk.x, chunk.y, lx, ly, power);

            if (h > 0) return;

            waterWorld.cells[x, y] = 0;

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
                    float h = c.Mine(chunk.x, chunk.y, lx, ly, power);

                    if (h > 0) continue;

                    waterWorld.cells[X, Y] = 0;

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

                waterWorld.cells[x, y] = -1;

                r.Tile?.Changed(this, x + 1, y, r);
                l.Tile?.Changed(this, x - 1, y, l);
                d.Tile?.Changed(this, x, y + 1, d);
                u.Tile?.Changed(this, x, y - 1, u);
                return true;
            }
            return false;
        }

        public bool TryPlaceTile(ITile tile, (int x, int y) position) => TryPlaceTile(tile, position.x, position.y);

        public bool VisitCell(RayCastReportTileDelegate callback, Vec2 start, Vec2 end, int x, int y)
        {
            var chunk = Cell2Chunk(x, y);

            Chunk c = chunks[chunk.x, chunk.y];
            TileData data = c[x - chunk.x * chunkSize, y - chunk.y * chunkSize];
            
            return false;
        }

        public bool RayCast(RayCastReportTileDelegate callback, Vec2 start, Vec2 end)
        {
            int cx = (int)MathF.Floor(start.X); // Begin/current cell coords
            int cy = (int)MathF.Floor(start.Y);
            int ex = (int)MathF.Floor(end.X); // End cell coords
            int ey = (int)MathF.Floor(end.Y);

            // Delta or direction
            float dx = end.X - start.X;
            float dy = end.Y - start.Y;

            Vec2 baseStart = start;

            while (cx < ex && cy < ey)
            {
                // find intersection "time" in x dir
                float t0 = ((int)MathF.Ceiling(start.X) - start.X) / dx;
                float t1 = ((int)MathF.Ceiling(start.Y) - start.Y) / dy;

                if (VisitCell(callback, baseStart, end, cx, cy)) return true;

                if (t0 < t1) // cross x boundary first=?
                {
                    ++cx;
                    start.X += t0 * dx;
                    start.Y += t0 * dy;
                }
                else
                {
                    ++cy;
                    start.X += t1 * dx;
                    start.Y += t1 * dy;
                }
            }
            return false;
        }

        public class Chunk
        {
            private TileData[,] tiles;

            public Chunk(int x, int y, Func<int, int, ITile> generator)
            {
                tiles = new TileData[chunkSize, chunkSize];
                int j;
                for (int i = 0; i < chunkSize; i++)
                    for (j = 0; j < chunkSize; j++)
                        tiles[i, j] = new TileData(generator(x * chunkSize + i, y * chunkSize + j));
            }

            public TileData this[int x, int y]
            {
                get => tiles[x, y];
                set => tiles[x, y] = value;
            }

            public void Start(IMap map, int x, int y)
            {
                int j;
                for (int i = 0; i < chunkSize; i++)
                    for (j = 0; j < chunkSize; j++)
                    {
                        TileData tile = tiles[i, j];
                        tile.Tile?.Start(map, x * chunkSize + i, y * chunkSize + j, tile);
                    }
            }

            public void Update(IMap map, int x, int y)
            {
                int j;
                for (int i = 0; i < chunkSize; i++)
                    for (j = 0; j < chunkSize; j++)
                    {
                        TileData tile = tiles[i, j];
                        tile.Tile?.Update(map, x * chunkSize + i, y * chunkSize + j, tile);
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

            public void Draw(IMap map, int x, int y)
            {
                int j;
                for (int i = 0; i < chunkSize; i++)
                {
                    for (j = 0; j < chunkSize; j++)
                    {
                        TileData tile = tiles[i, j];
                        tile.Tile?.Draw(map, x * chunkSize + i, y * chunkSize + j, new Vec2(x * chunkSize + i, y * chunkSize + j) * tileSize, 0, tile);
                    }
                }
            }

            public float Mine(int cx, int cy, int x, int y, float power)
            {
                float h = tiles[x, y].Health -= power;
                if (h <= 0)
                {
                    new Item((tiles[x, y].Tile, 1), new Vec2((cx * chunkSize + x + 0.5f) * tileSize, (cy * chunkSize + y + 0.5f) * tileSize));
                    tiles[x, y] = default;
                }
                return h;
            }
        }
    }
}
