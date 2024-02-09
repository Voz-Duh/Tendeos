using XnaGame.Utils;
using System;
using XnaGame.PEntities.Content;
using XnaGame.World.Liquid;

namespace XnaGame.World
{
    public class Map : IMap
    {
        public const float tileSize = 8;
        public const int chunkSize = 8;

        private WaterWorld waterWorld;
        private readonly int mapWidth, mapHeight;
        public readonly Chunk[,] chunks;
        public ITile ignore;

        public int FullWidth => mapWidth * chunkSize;
        public int FullHeight => mapHeight * chunkSize;

        public Map(int mapWidth, int mapHeight, WorldGenerator generator)
        {
            this.mapWidth = mapWidth;
            this.mapHeight = mapHeight;
            chunks = new Chunk[mapWidth, mapHeight];

            waterWorld = new WaterWorld(chunkSize * mapWidth, chunkSize * mapHeight, generator);

            int i, j;
            for (i = 0; i < mapWidth; i++)
                for (j = 0; j < mapHeight; j++)
                    chunks[i, j] = new Chunk(i, j, generator, waterWorld);
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

        public void MineTile(bool top, int x, int y, float power)
        {
            if (x < 0) return;
            else if (x >= chunkSize * mapWidth) return;
            else if (y < 0) return;
            else if (y >= chunkSize * mapHeight) return;

            var chunk = Cell2Chunk(x, y);

            Chunk c = chunks[chunk.x, chunk.y];
            int lx = x - chunk.x * chunkSize,
                ly = y - chunk.y * chunkSize;
            if (c[top, lx, ly].Tile == null) return;
            float h = c.Mine(top, chunk.x, chunk.y, lx, ly, power);

            if (h > 0) return;

            waterWorld.cells[x, y] = 0;

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
                    else if (X >= chunkSize * mapWidth) continue;
                    else if (Y < 0) continue;
                    else if (Y >= chunkSize * mapHeight) continue;

                    var chunk = Cell2Chunk(X, Y);

                    Chunk c = chunks[chunk.x, chunk.y];
                    int lx = X - chunk.x * chunkSize,
                        ly = Y - chunk.y * chunkSize;
                    if (c[top, lx, ly].Tile == null) continue;
                    float h = c.Mine(top, chunk.x, chunk.y, lx, ly, power);

                    if (h > 0) continue;

                    waterWorld.cells[X, Y] = 0;

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
            else if (x >= chunkSize * mapWidth) return default;
            else if (y < 0) return default;
            else if (y >= chunkSize * mapHeight) return default;

            var chunk = Cell2Chunk(x, y);
            return chunks[chunk.x, chunk.y][top, x - chunk.x * chunkSize, y - chunk.y * chunkSize];
        }

        public TileData GetTile(bool top, (int x, int y) position) => GetTile(top, position.x, position.y);

        public void SetTile(bool top, ITile tile, int x, int y)
        {
            if (tile == ignore) return;

            if (x < 0) return;
            else if (x >= chunkSize * mapWidth) return;
            else if (y < 0) return;
            else if (y >= chunkSize * mapHeight) return;

            var chunk = Cell2Chunk(x, y);

            Chunk c = chunks[chunk.x, chunk.y];
            TileData data = c[top, x - chunk.x * chunkSize, y - chunk.y * chunkSize] = new TileData(tile);
            tile?.Start(top, this, x, y, data);

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
            if (tile == ignore) return false;

            if (x < 0) return false;
            else if (x >= chunkSize * mapWidth) return false;
            else if (y < 0) return false;
            else if (y >= chunkSize * mapHeight) return false;

            var chunk = Cell2Chunk(x, y);

            Chunk c = chunks[chunk.x, chunk.y];
            int lx = x - chunk.x * chunkSize,
                ly = y - chunk.y * chunkSize;
            if (c[top, lx, ly].Tile != null) return false;
            TileData data = c[top, lx, ly] = new TileData(tile);
            tile?.Start(top, this, x, y, data);

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

        public bool PlaceTile(bool top, ITile tile, int x, int y)
        {
            if (tile == ignore) return false;

            if (x < 0) return false;
            else if (x >= chunkSize * mapWidth) return false;
            else if (y < 0) return false;
            else if (y >= chunkSize * mapHeight) return false;

            TileData
                r = GetTile(top, x + 1, y),
                l = GetTile(top, x - 1, y),
                d = GetTile(top, x, y + 1),
                u = GetTile(top, x, y - 1);
            if (r.Tile != null ||
                l.Tile != null ||
                d.Tile != null ||
                u.Tile != null ||
                GetTile(!top, x+1, y).Tile != null ||
                GetTile(!top, x-1, y).Tile != null ||
                GetTile(!top, x, y+1).Tile != null ||
                GetTile(!top, x, y-1).Tile != null ||
                GetTile(!top, x, y).Tile != null)
            {
                var chunk = Cell2Chunk(x, y);

                Chunk c = chunks[chunk.x, chunk.y];
                TileData data = c[top, x - chunk.x * chunkSize, y - chunk.y * chunkSize] = new TileData(tile);
                tile?.Start(top, this, x, y, data);

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
            if (tile == ignore) return false;

            if (x < 0) return false;
            else if (x >= chunkSize * mapWidth) return false;
            else if (y < 0) return false;
            else if (y >= chunkSize * mapHeight) return false;

            TileData
                r = GetTile(top, x + 1, y),
                l = GetTile(top, x - 1, y),
                d = GetTile(top, x, y + 1),
                u = GetTile(top, x, y - 1);
            if (r.Tile != null ||
                l.Tile != null ||
                d.Tile != null ||
                u.Tile != null ||
                GetTile(!top, x+1, y).Tile != null ||
                GetTile(!top, x-1, y).Tile != null ||
                GetTile(!top, x, y+1).Tile != null ||
                GetTile(!top, x, y-1).Tile != null ||
                GetTile(!top, x, y).Tile != null)
            {
                var chunk = Cell2Chunk(x, y);

                Chunk c = chunks[chunk.x, chunk.y];
                int lx = x - chunk.x * chunkSize,
                    ly = y - chunk.y * chunkSize;
                if (c[top, lx, ly].Tile != null) return false;
                TileData data = c[top, lx, ly] = new TileData(tile);
                tile?.Start(top, this, x, y, data);

                waterWorld.cells[x, y] = -1;

                r.Tile?.Changed(top, this, x + 1, y, r);
                l.Tile?.Changed(top, this, x - 1, y, l);
                d.Tile?.Changed(top, this, x, y + 1, d);
                u.Tile?.Changed(top, this, x, y - 1, u);
                return true;
            }
            return false;
        }

        public bool TryPlaceTile(bool top, ITile tile, (int x, int y) position) => TryPlaceTile(top, tile, position.x, position.y);

        public class Chunk
        {
            private TileData[,] tiles, walls;

            public Chunk(int x, int y, WorldGenerator generator, WaterWorld waterWorld)
            {
                tiles = new TileData[chunkSize, chunkSize];
                walls = new TileData[chunkSize, chunkSize];
                int j;
                for (int i = 0; i < chunkSize; i++)
                    for (j = 0; j < chunkSize; j++)
                    {
                        int X = x * chunkSize + i, Y = y * chunkSize + j;
                        ITile tile = generator.GetTile(X, Y);
                        tiles[i, j] = new TileData(tile);
                        walls[i, j] = new TileData(generator.GetWalls(X, Y));
                        if (tile != null) waterWorld.cells[X, Y] = waterWorld.render[X, Y] = -1;
                    }
            }

            public TileData this[bool type, int x, int y]
            {
                get => (type ? tiles : walls)[x, y];
                set => (type ? tiles : walls)[x, y] = value;
            }

            public void Start(IMap map, int x, int y)
            {
                int j;
                for (int i = 0; i < chunkSize; i++)
                    for (j = 0; j < chunkSize; j++)
                    {
                        TileData tile = walls[i, j];
                        tile.Tile?.Start(false, map, x * chunkSize + i, y * chunkSize + j, tile);
                        tile = tiles[i, j];
                        tile.Tile?.Start(true, map, x * chunkSize + i, y * chunkSize + j, tile);
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
                        TileData tile = walls[i, j];
                        tile.Tile?.Draw(false, map, x * chunkSize + i, y * chunkSize + j, new Vec2(x * chunkSize + i, y * chunkSize + j) * tileSize, 0, tile);
                        tile = tiles[i, j];
                        tile.Tile?.Draw(true, map, x * chunkSize + i, y * chunkSize + j, new Vec2(x * chunkSize + i, y * chunkSize + j) * tileSize, 0, tile);
                    }
                }
            }

            public float Mine(bool type, int cx, int cy, int x, int y, float power)
            {
                float h = (type ? tiles : walls)[x, y].Health -= power;
                if (h <= 0)
                {
                    new Item(((type ? tiles : walls)[x, y].Tile, 1), new Vec2((cx * chunkSize + x + 0.5f) * tileSize, (cy * chunkSize + y + 0.5f) * tileSize));
                    (type ? tiles : walls)[x, y] = default;
                }
                return h;
            }
        }
    }
}
