using System;
using System.Threading.Tasks;
using XnaGame.Utils;

namespace XnaGame.World.Generation
{
    public class WorldGenerator
    {
        public string Message { private set; get; }
        public bool Done { private set; get; }

        public Biome[] biomes;
        private readonly uint seed;

        public WorldGenerator(Biome[] biomes, uint seed)
        {
            Done = false;
            this.biomes = biomes;
            this.seed = seed;
            Message = Localization.Get("generate_load");
        }

        public async Task Generate(IMap map)
        {
            URandom random = new URandom(seed);
            Message = Localization.Get("generate_biome");
            await Task.Run(() =>
            {
                int y;
                float noise;
                Biome biome;
                for (int x = 0; x < map.Width; x++)
                {
                    noise = Noise.Perlin(seed + 1, x / 30f * map.ChunkSize, 0);
                    noise = (noise + Noise.Perlin(seed + 2, x / 20f * map.ChunkSize, 0)) / 2f;
                    biome = biomes[(int)MathF.Round(Math.Clamp(noise, 0, 1) * (biomes.Length - 1))];
                    for (y = 0; y < map.Height; y++)
                        map.GetChunk(x, y).Biome = biome;
                }
            });
            Message = Localization.Get("generate_base");
            await Task.Run(() =>
            {
                int y;
                float ground;
                Biome biome;
                for (int x = 0; x < map.FullWidth; x++)
                {
                    ground = (Noise.Perlin(seed, x / 10f, 0) + Noise.Perlin(seed, x / 15f, 0)) / 2f;
                    biome = map.GetTileChunk(x, 0).Biome;
                    for (y = (int)(biome.GroundHeight - ground * biome.HillsHeight); y < map.FullHeight; y++)
                    {
                        if (y > biome.GroundHeight)
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
            Message = Localization.Get("generate_smooth");
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
                    NEXT:;
                }
            });
            Message = Localization.Get("generate_ground");
            await Task.Run(() =>
            {
                TileData data, l, r, d, u;
                int y;
                for (int x = 0; x < map.FullWidth; x++)
                    for (y = 0; y < map.FullHeight; y++)
                    {
                        data = map.GetTile(true, x, y);
                        if (data.Tile != null)
                            foreach (Biome biome in biomes)
                                if (biome.Grounds != null)
                                    foreach (var (f, t) in biome.Grounds)
                                        if (data.Tile == f)
                                        {
                                            if (URandom.SInt(5) == 0) map.SetTile(true, t, x, y);
                                            l = map.GetTile(true, x, y - 1);
                                            r = map.GetTile(true, x, y + 1);
                                            d = map.GetTile(true, x, y - 1);
                                            u = map.GetTile(true, x, y + 1);
                                            if (l.Tile == null || r.Tile == null || d.Tile == null || u.Tile == null)
                                                map.SetTile(true, t, x, y);
                                            break;
                                        }
                    }
            });
            Message = Localization.Get("generate_trees");
            await Task.Run(() =>
            {
                TileData data;
                Biome biome;
                int y;
                for (int x = 0; x < map.FullWidth; x++)
                    for (y = 0; y < map.FullHeight; y++)
                    {
                        data = map.GetTile(true, x, y);
                        if (data.Tile != null)
                        {
                            biome = map.GetChunk(x / map.ChunkSize, y / map.ChunkSize).Biome;
                            if (random.Float() < biome.TreeChance)
                                map.SetTile(true, biome.Tree, x, y - 1);
                            break;
                        }
                    }
            });
            Done = true;
        }

        public void Loaded()
        {
            Done = true;
        }
    }
}
