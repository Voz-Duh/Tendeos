using Microsoft.Xna.Framework.Graphics;
using System;
using System.Threading.Tasks;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;
using Tendeos.World.Generation;
using Tendeos.World;
using Tendeos.Content;
using Tendeos.Utils.Input;

namespace Tendeos.Scenes
{
    public class WorldGeneratorScene : Scene
    {
        public class PreviewMap
        {
            private readonly Batch batch;
            public readonly PreviewChunk[,] chunks;
            public readonly int Width, Height, ChunkSize;

            public const int chunkSize = 8;
            public float TileSize;

            public PreviewMap(int width, int height, float tileSize, GraphicsDevice graphicsDevice)
            {
                batch = new Batch(graphicsDevice);

                chunks = new PreviewChunk[width, height];
                int y;
                for (int x = 0; x < width; x++)
                    for (y = 0; y < height; y++)
                    {
                        chunks[x, y] = new PreviewChunk();
                    }
                Width = width;
                Height = height;
                TileSize = tileSize;
                ChunkSize = chunkSize;
            }

            public void Draw(Camera camera)
            {
                batch.Begin(PrimitiveType.TriangleList, 508600, camera.GetViewMatrix());
                int y;
                for (int x = 0; x < Width; x++)
                    for (y = 0; y < Height; y++)
                        chunks[x, y].Draw(x, y, batch, this);
                batch.End();
            }

            public class PreviewChunk
            {
                private const byte row = 0b10000000;
                private const byte irow = 0b11111111;

                public byte row0, row1, row2, row3, row4, row5, row6, row7;

                public void Draw(int cx, int cy, Batch batch, PreviewMap map)
                {
                    byte y;
                    for (byte x = 0; x < chunkSize; x++)
                        for (y = 0; y < chunkSize; y++)
                            if (this[x, y])
                            {
                                batch.Vertex3((cx * chunkSize + x) * map.TileSize, (cy * chunkSize + y) * map.TileSize, 0);
                                batch.Vertex3((cx * chunkSize + x + 1) * map.TileSize, (cy * chunkSize + y) * map.TileSize, 0);
                                batch.Vertex3((cx * chunkSize + x) * map.TileSize, (cy * chunkSize + y + 1) * map.TileSize, 0);

                                batch.Vertex3((cx * chunkSize + x) * map.TileSize, (cy * chunkSize + y + 1) * map.TileSize, 0);
                                batch.Vertex3((cx * chunkSize + x + 1) * map.TileSize, (cy * chunkSize + y) * map.TileSize, 0);
                                batch.Vertex3((cx * chunkSize + x + 1) * map.TileSize, (cy * chunkSize + y + 1) * map.TileSize, 0);
                            }
                }

                public bool this[int x, int y]
                {
                    get
                    {
                        return y switch
                        {
                            0 => (row0 << x & row) == row,
                            1 => (row1 << x & row) == row,
                            2 => (row2 << x & row) == row,
                            3 => (row3 << x & row) == row,
                            4 => (row4 << x & row) == row,
                            5 => (row5 << x & row) == row,
                            6 => (row6 << x & row) == row,
                            _ => (row7 << x & row) == row,
                        };
                    }
                    set
                    {
                        switch (y)
                        {
                            case 0:
                                if (value) row0 |= (byte)(row >> x);
                                else row0 &= (byte)((row >> x) ^ irow);
                                break;
                            case 1:
                                if (value) row1 |= (byte)(row >> x);
                                else row1 &= (byte)((row >> x) ^ irow);
                                break;
                            case 2:
                                if (value) row2 |= (byte)(row >> x);
                                else row2 &= (byte)((row >> x) ^ irow);
                                break;
                            case 3:
                                if (value) row3 |= (byte)(row >> x);
                                else row3 &= (byte)((row >> x) ^ irow);
                                break;
                            case 4:
                                if (value) row4 |= (byte)(row >> x);
                                else row4 &= (byte)((row >> x) ^ irow);
                                break;
                            case 5:
                                if (value) row5 |= (byte)(row >> x);
                                else row5 &= (byte)((row >> x) ^ irow);
                                break;
                            case 6:
                                if (value) row6 |= (byte)(row >> x);
                                else row6 &= (byte)((row >> x) ^ irow);
                                break;
                            default:
                                if (value) row7 |= (byte)(row >> x);
                                else row7 &= (byte)((row >> x) ^ irow);
                                break;
                        }
                    }
                }
            }
        }

        private PreviewMap map;
        private Vec2 click;

        public WorldGeneratorScene(Core game) : base(game)
        {
            Game.camera.ScreenHeight = 400;
            map = new PreviewMap(40, 40, 8, Game.GraphicsDevice);
        }

        public override void Clear()
        {
            map = null;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            map.Draw(Game.camera);
        }

        public override void Init()
        {
            URandom random = new URandom(0);
            Task.Run(() =>
            {
                TileData a, b;
                PreviewMap.PreviewChunk chunk;
                Cave cave;
                float power;
                float lp;
                int y, width, height, fromx, fromy, tox, toy, lx, ly, fx, fy, tx, ty, rx, ry;
                for (int x = 0; x < map.Width; x++)
                    for (y = 0; y < map.Height; y++)
                    {
                        chunk = map.chunks[x, y];
                        if (Biomes.test.Caves == null) continue;
                        cave = Biomes.test.Caves[random.Int(Biomes.test.Caves.Length)];
                        if (random.Float() < cave.SpawnChance)
                        {
                            width = 2;
                            height = 2;
                            fromx = Math.Max(0, x - width / 2);
                            fromy = Math.Max(0, y - height / 2);
                            tox = Math.Min(map.Width - 1, fromx + width);
                            toy = Math.Min(map.Height - 1, fromy + height);
                            fx = x * map.ChunkSize + map.ChunkSize / 2;
                            fy = y * map.ChunkSize + map.ChunkSize / 2;
                            height *= map.ChunkSize / 2;
                            width *= map.ChunkSize / 2;
                            for (lx = fromx; lx <= tox; lx++)
                                for (ly = fromy; ly <= toy; ly++)
                                {
                                    chunk = map.chunks[lx, ly];
                                    power = 0;
                                    for (tx = 0; tx < map.ChunkSize; tx++)
                                        for (ty = 0; ty < map.ChunkSize; ty++)
                                        {
                                            rx = lx * map.ChunkSize + tx;
                                            ry = ly * map.ChunkSize + ty;
                                            power += lp = new Vec2((fx - rx) / (float)width, (fy - ry) / (float)height).Length();// - cave.CornersNoise.Get(seed, rx, ry) * cave.CornersPower;
                                            if (lp < 1)
                                            {
                                                chunk[tx, ty] = true;
                                            }
                                        }
                                    if (power / (map.ChunkSize * map.ChunkSize) >= 0.5)
                                    {
                                        //chunk.Biome = cave;
                                    }
                                }
                        }
                    }
            });
        }

        public override void InitGUI()
        {
        }

        public override void Update()
        {
            if (Mouse.LeftPressed) click = Mouse.GUIPosition;
            if (Mouse.LeftDown)
            {
                Game.camera.Position += click - Mouse.GUIPosition;
                click = Mouse.GUIPosition;
            }
        }
    }
}
