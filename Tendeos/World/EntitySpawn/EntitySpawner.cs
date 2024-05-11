using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Tendeos.Physical.Content;
using Tendeos.Utils;

namespace Tendeos.World.EntitySpawn
{
    public class EntitySpawner
    {
        private readonly ThreadLoop loop;
        private readonly IMap map;

        public EntitySpawner(IMap map)
        {
            loop = new ThreadLoop(Spawn, 100, 1000);
            this.map = map;
        }

        public void Start() => loop.Start();

        public void Spawn(float delta)
        {
            Vec2 position = Core.Player.transform.Position;
            (int x, int y) = map.World2Cell(position);
            (int cx, int cy) = map.Cell2Chunk(x, y);
            List<IChunk> chunks = new List<IChunk>();

            int i, j;
            for (i = cx - 5; i <= cx + 5; i++)
            {
                for (j = cy - 5; j <= cy + 5; j++)
                {
                    if (i < 0 || i >= map.Width || j < 0 || j >= map.Height) continue;
                    chunks.Add(map.GetChunk(i, j));
                }
            }
            EnemyBuilder enemy;
            if (chunks.Count != 0)
            {
                IChunk chunk = chunks[URandom.SInt(chunks.Count)];
                if (chunk.Biome.Enemies.Length != 0)
                {
                    enemy = chunk.Biome.Enemies[URandom.SInt(chunk.Biome.Enemies.Length)];
                    if (enemy.spawnChance >= URandom.SFloat(100))
                    {
                        (int width, int height) = map.World2Cell(enemy.size);
                        int offset = map.ChunkSize / 2 + map.ChunkSize * 5;
                        int fromx = Math.Clamp(x - offset, 0, map.FullWidth),
                            tox = Math.Clamp(x + offset, 0, map.FullWidth),
                            fromy = Math.Clamp(y - offset, 0, map.FullWidth),
                            toy = Math.Clamp(y + offset, 0, map.FullWidth);
                        List<FRectangle> canSpawnIn = new List<FRectangle>();
                        for (i = fromx; i <= tox; i++)
                        {
                            if (i + width >= map.FullWidth)
                                continue;
                            for (j = fromy; j <= toy; j++)
                            {
                                if (j + height >= map.FullHeight)
                                    continue;
                                for (x = 0; x <= width; x++)
                                    for (y = 0; y <= height; y++)
                                        if (map.GetTile(true, i + x, j + y).Tile != null)
                                            goto BAD;
                                canSpawnIn.Add(new FRectangle(i * map.TileSize, j * map.TileSize, width * map.TileSize, height * map.TileSize));
                                BAD:;
                            }
                        }
                        if (canSpawnIn.Count != 0)
                        {
                            FRectangle spawnRect = canSpawnIn[URandom.SInt(canSpawnIn.Count)];
                            enemy.Spawn(new Vector2(
                                URandom.SFloat(spawnRect.X + enemy.size.X / 2, spawnRect.X + spawnRect.Width - enemy.size.X / 2),
                                URandom.SFloat(spawnRect.Y + enemy.size.Y / 2, spawnRect.Y + spawnRect.Height - enemy.size.Y / 2)));
                        }
                    }
                }
            }
        }
    }
}
