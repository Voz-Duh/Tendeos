using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tendeos.Utils;
using Tendeos.World.Content;
using Tendeos.World.Structures;

namespace Tendeos.World.Generation
{
    public class WorldGenerator
    {
        public string Message { private set; get; }
        public bool Done { private set; get; }

        private readonly uint seed;
        private readonly Biome[] biomes;
        private readonly Noise smallCaveNoise;
        private readonly Noise bigCaveNoise;
        private readonly Noise giganticCaveNoise;
        private readonly int groundHeight;

        public WorldGenerator(Noise smallCaveNoise, Noise bigCaveNoise, Noise giganticCaveNoise, int groundHeight,
            Biome[] biomes, uint seed)
        {
            Done = false;
            this.smallCaveNoise = smallCaveNoise;
            this.bigCaveNoise = bigCaveNoise;
            this.giganticCaveNoise = giganticCaveNoise;
            this.groundHeight = groundHeight;
            this.biomes = biomes;
            this.seed = seed;
            Message = Localization.Translate("generate_load");
        }

        public async Task Generate(IMap map)
        {
            URandom random = new URandom(seed);
            Message = Localization.Translate("generate_biome");
            await Task.Run(() =>
            {
                int y;
                float noise;
                Biome biome;
                for (int x = 0; x < map.Width; x++)
                {
                    noise = Perlin.Noise2D(seed + 1, x / 10f * map.ChunkSize, 0);
                    noise = (noise + Perlin.Noise2D(seed + 2, x / 20f * map.ChunkSize, 0)) / 2f;
                    biome = biomes[(int) MathF.Round(Math.Clamp(noise, 0, 1) * (biomes.Length - 1))];
                    for (y = 0; y < map.Height; y++)
                        map.GetChunk(x, y).Biome = biome;
                }
            });
            Message = Localization.Translate("generate_base");
            await Task.Run(() =>
            {
                int y;
                float ground;
                Biome biome;
                for (int x = 0; x < map.FullWidth; x++)
                {
                    ground = (Perlin.Noise2D(seed, x / 10f, 0) + Perlin.Noise2D(seed, x / 15f, 0)) / 2f;
                    biome = (Biome) map.GetTileChunk(x, 0).Biome;
                    for (y = (int) (groundHeight - biome.GroundHeight - ground * biome.HillsHeight);
                         y < map.FullHeight;
                         y++)
                    {
                        if (y > groundHeight)
                        {
                            map.SetTile(false, biome.UndegroundTile, x, y);
                            map.SetTile(true, biome.UndegroundTile, x, y);
                        }
                        else
                        {
                            map.SetTile(false, biome.GroundTile, x, y);
                            map.SetTile(true, biome.GroundTile, x, y);
                        }
                    }
                }
            });
            Message = Localization.Translate("generate_smooth");
            await Task.Run(() =>
            {
                int[] heigth = new int[10];
                int tmp = 0;
                int y, i;
                int diff;
                TileData data;
                for (int x = 0; x < map.FullWidth; x++)
                {
                    for (y = 0; y < map.FullHeight; y++)
                    {
                        data = map.GetTile(true, x, y);
                        if (data.Tile != null)
                        {
                            tmp = 0;
                            if (x == 0)
                                for (i = 0; i < heigth.Length; i++)
                                    tmp += heigth[i] = y;
                            else
                            {
                                for (i = 1; i < heigth.Length; i++) tmp += heigth[i - 1] = heigth[i];
                                tmp += heigth[^1] = y;
                            }

                            diff = y - (tmp / heigth.Length);
                            if (diff > 0)
                                for (i = 1; i < diff; i++)
                                {
                                    map.SetTile(false, data.Tile, x, y - i);
                                    map.SetTile(true, data.Tile, x, y - i);
                                }
                            else
                            {
                                for (i = 0; i < -diff; i++)
                                {
                                    map.SetTile(false, null, x, y + i);
                                    map.SetTile(true, null, x, y + i);
                                }
                            }

                            goto NEXT;
                        }
                    }

                    NEXT: ;
                }
            });
            Message = Localization.Translate("generate_special_caves");
            await Task.Run(() =>
            {
                IChunk chunk;
                Cave cave;
                float power;
                float lp;
                int y, width, height, fromx, fromy, tox, toy, lx, ly, fx, fy, tx, ty, rx, ry;
                for (int x = 0; x < map.Width; x++)
                for (y = 0; y < map.Height; y++)
                {
                    chunk = map.GetChunk(x, y);
                    if (chunk.Biome is Biome biome)
                    {
                        if (biome.Caves == null) continue;
                        cave = biome.Caves[random.Int(biome.Caves.Length)];
                        if (y < (cave.Height.Start.IsFromEnd ? 0 : cave.Height.Start.Value) ||
                            y > (cave.Height.End.IsFromEnd ? map.Height - 1 : cave.Height.End.Value))
                            continue;
                        if (random.Float() < cave.SpawnChance)
                        {
                            width = cave.ChunksWidth.Random();
                            height = cave.ChunksHeight.Random();
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
                                chunk = map.GetChunk(lx, ly);
                                power = 0;
                                for (tx = 0; tx < map.ChunkSize; tx++)
                                for (ty = 0; ty < map.ChunkSize; ty++)
                                {
                                    rx = lx * map.ChunkSize + tx;
                                    ry = ly * map.ChunkSize + ty;
                                    power += lp =
                                        new Vec2((fx - rx) / (float) width, (fy - ry) / (float) height).Length() -
                                        cave.CornersNoise.Get(seed, rx, ry) * cave.CornersPower;
                                    if (lp < 1)
                                    {
                                        map.SetTile(false, cave.Tile, rx, ry);
                                        map.SetTile(true, cave.Tile, rx, ry);
                                    }
                                }

                                if (power / (map.ChunkSize * map.ChunkSize) >= 0.5)
                                {
                                    chunk.Biome = cave;
                                }
                            }
                        }
                    }
                }
            });
            const float caveSquare = 121;
            Message = Localization.Translate("generate_small_caves");
            await Task.Run(() =>
            {
                TileData data;
                float p;
                int y, z, w, i, j, l;
                for (int x = 0; x < map.FullWidth; x++)
                for (y = 0; y < map.FullHeight; y++)
                {
                    data = map.GetTile(true, x, y);
                    if (data.Tile != null)
                    {
                        p = 0;
                        j = y - 5;
                        i = y + 5;
                        l = x + 5;
                        for (z = x - 5; z <= l; z++)
                        for (w = j; w <= i; w++)
                            p += map.GetTileChunk(Math.Clamp(z, 0, map.FullWidth - 1),
                                Math.Clamp(w, 0, map.FullHeight - 1)).Biome.SmallCavesPower;

                        if (smallCaveNoise.Get(seed, x, y) >= p / caveSquare)
                            map.SetTile(true, null, x, y);
                    }
                }
            });
            Message = Localization.Translate("generate_big_caves");
            await Task.Run(() =>
            {
                TileData data;
                float p;
                int y, z, w, i, j, l;
                for (int x = 0; x < map.FullWidth; x++)
                for (y = 0; y < map.FullHeight; y++)
                {
                    data = map.GetTile(true, x, y);
                    if (data.Tile != null)
                    {
                        p = 0;
                        j = y - 5;
                        i = y + 5;
                        l = x + 5;
                        for (z = x - 5; z <= l; z++)
                        for (w = j; w <= i; w++)
                            p += map.GetTileChunk(Math.Clamp(z, 0, map.FullWidth - 1),
                                Math.Clamp(w, 0, map.FullHeight - 1)).Biome.BigCavesPower;

                        if (bigCaveNoise.Get(seed, x, y) <= p / caveSquare)
                            map.SetTile(true, null, x, y);
                    }
                }
            });
            Message = Localization.Translate("generate_gigantic_caves");
            await Task.Run(() =>
            {
                TileData data;
                float p;
                int y, z, w, i, j, l;
                for (int x = 0; x < map.FullWidth; x++)
                for (y = 0; y < map.FullHeight; y++)
                {
                    data = map.GetTile(true, x, y);
                    if (data.Tile != null)
                    {
                        p = 0;
                        j = y - 5;
                        i = y + 5;
                        l = x + 5;
                        for (z = x - 5; z <= l; z++)
                        for (w = j; w <= i; w++)
                            p += map.GetTileChunk(Math.Clamp(z, 0, map.FullWidth - 1),
                                Math.Clamp(w, 0, map.FullHeight - 1)).Biome.GiganticCavesPower;

                        if (giganticCaveNoise.Get(seed, x, y) <= p / caveSquare)
                            map.SetTile(true, null, x, y);
                    }
                }
            });
            Message = Localization.Translate("generate_ground");
            await Task.Run(() =>
            {
                BaseBiome biome;
                TileData data, l, r, d, u;
                int y;
                for (int x = 0; x < map.FullWidth; x++)
                for (y = 0; y < map.FullHeight; y++)
                {
                    data = map.GetTile(true, x, y);
                    biome = map.GetTileChunk(x, y).Biome;
                    if (data.Tile != null)
                        if (biome.Grounds != null)
                            foreach (var (f, t) in biome.Grounds)
                                if (data.Tile == f)
                                {
                                    if (URandom.SInt(5) == 0) map.SetTile(true, t, x, y);
                                    l = map.GetTile(true, x, y - 1);
                                    r = map.GetTile(true, x, y + 1);
                                    d = map.GetTile(true, x - 1, y);
                                    u = map.GetTile(true, x + 1, y);
                                    if (l.Tile == null || r.Tile == null || d.Tile == null || u.Tile == null)
                                        map.SetTile(true, t, x, y);
                                    break;
                                }
                }
            });
            Message = Localization.Translate("generate_trees");
            await Task.Run(() =>
            {
                TileData data;
                int y;
                for (int x = 0; x < map.FullWidth; x++)
                for (y = 0; y < map.FullHeight; y++)
                {
                    data = map.GetTile(true, x, y);
                    if (data.Tile != null)
                    {
                        if (map.GetChunk(x / map.ChunkSize, y / map.ChunkSize).Biome is Biome biome)
                            if (random.Float() < biome.TreeChance)
                                map.SetTile(true, biome.Tree, x, y - 1);
                        break;
                    }
                }
            });
            Message = Localization.Translate("generate_small_smooth_ground");
            await Task.Run(() => Smooth(map, true, random));
            Message = Localization.Translate("generate_small_smooth_walls");
            await Task.Run(() => Smooth(map, false, random));
            Message = Localization.Translate("generate_structures");
            await Task.Run(() =>
            {
                Rectangle[] quadtree = {new Rectangle(0, 0, map.FullWidth, map.FullHeight)};
                Structure structure;
                BaseBiome biome;
                Rectangle spawn;
                int x, y;

                for (int i = 0; i < 50; i++)
                {
                    spawn = quadtree[URandom.SInt(Math.Min(5, quadtree.Length - 1))];
                    x = spawn.X + URandom.SInt(spawn.Width);
                    y = spawn.Y + URandom.SInt(spawn.Height);
                    biome = map.GetTileChunk(x, y).Biome;
                    if (biome.Structures != null)
                    {
                        structure = biome.Structures[URandom.SInt(biome.Structures.Length - 1)];
                        x -= structure.Width;
                        y -= structure.Height;
                        structure.Spawn(map, x, y);
                        RemoveRectangle(
                            new Rectangle(x, y, structure.Width + structure.Width / 2,
                                structure.Height + structure.Height / 2), ref quadtree);
                        Array.Sort(quadtree,
                            (a, b) => Math.Max(a.Height, a.Width) > Math.Max(b.Height, b.Width) ? -1 : 1);
                    }
                }
            });
            Done = true;
        }

        private static void RemoveRectangle(Rectangle rect, ref Rectangle[] quads)
        {
            List<Rectangle> nquadtree = new List<Rectangle>();
            Queue<Rectangle> quadtree = new Queue<Rectangle>(quads);
            Rectangle quad;
            while (quadtree.Count > 0)
            {
                quad = quadtree.Dequeue();

                if (rect.X + rect.Width >= quad.X &&
                    rect.Y + rect.Height >= quad.Y &&
                    rect.X <= quad.X + quad.Width &&
                    rect.Y <= quad.Y + quad.Height)
                {
                    if (Math.Min(quad.Width, quad.Height) > 2)
                    {
                        int w = quad.Width / 2;
                        int h = quad.Height / 2;
                        quadtree.Enqueue(new Rectangle(quad.X, quad.Y, w, h));
                        quadtree.Enqueue(new Rectangle(quad.X, quad.Y + h, w, h));
                        quadtree.Enqueue(new Rectangle(quad.X + w, quad.Y, w, h));
                        quadtree.Enqueue(new Rectangle(quad.X + w, quad.Y + h, w, h));
                    }
                }
                else
                {
                    nquadtree.Add(quad);
                }
            }

            quads = nquadtree.ToArray();
        }

        public void Smooth(IMap map, bool top, URandom random)
        {
            TileData data, left, right, up, down, leftup, leftdown, rightup, rightdown;
            int y;
            for (int x = 0; x < map.FullWidth; x++)
            for (y = 0; y < map.FullHeight; y++)
            {
                data = map.GetTile(top, x, y);
                if (data.Tile is AutoTile)
                {
                    left = map.GetTile(top, x - 1, y);
                    right = map.GetTile(top, x + 1, y);
                    up = map.GetTile(top, x, y - 1);
                    down = map.GetTile(top, x, y + 1);
                    leftup = map.GetTile(top, x - 1, y - 1);
                    leftdown = map.GetTile(top, x - 1, y + 1);
                    rightup = map.GetTile(top, x + 1, y - 1);
                    rightdown = map.GetTile(top, x + 1, y + 1);

                    if (((leftup.Tile != null ? rightup.Tile == null : rightup.Tile != null) &&
                         (leftdown.Tile != null ? rightdown.Tile == null : rightdown.Tile != null) ||
                         (leftup.Tile != null ? leftup.Tile == null : leftup.Tile != null) &&
                         (rightup.Tile != null ? rightdown.Tile == null : rightdown.Tile != null)) &&
                        (up.Tile == null || down.Tile == null))
                    {
                        map.SetTileData<AutoTile>(top, x, y, data =>
                        {
                            data.HasTriangleCollision = true;
                            return data;
                        });
                    }
                    else
                    {
                        if (up.Tile == null && down.Tile != null &&
                            (left.Tile != null && right.Tile != null || left.Tile != null
                                ? right.Tile == null
                                : right.Tile != null))
                        {
                            map.SetTileData<AutoTile>(top, x, y, data =>
                            {
                                //data.HasTriangleCollision = random.Int(10) > 5;
                                return data.SetU6(0, AutoTile.Down);
                            });
                        }
                        else if (down.Tile == null && up.Tile != null &&
                                 (left.Tile != null && right.Tile != null || left.Tile != null
                                     ? right.Tile == null
                                     : right.Tile != null))
                        {
                            map.SetTileData<AutoTile>(top, x, y, data =>
                            {
                                //data.HasTriangleCollision = random.Int(10) > 5;
                                return data.SetU6(0, AutoTile.Up);
                            });
                        }
                    }
                }
            }
        }

        public void Loaded()
        {
            Done = true;
        }
    }
}