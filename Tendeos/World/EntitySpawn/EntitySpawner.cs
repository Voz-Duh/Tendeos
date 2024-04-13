using Microsoft.Xna.Framework;
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
                    if (enemy.spawnChance <= URandom.SFloat(100))
                    {
                        List<FRectangle> canSpawnIn = new List<FRectangle>();
                        for (i = cx - 5; i <= cx + 5; i++)
                            for (j = cy - 5; j <= cy + 5; j++)
                            {
                                if (i < 0 || i >= map.Width || j < 0 || j >= map.Height) continue;
                                foreach (Rectangle rect in map.GetChunk(i, j).AirQuadtree)
                                {
                                    if (enemy.size.X <= rect.Width * map.TileSize &&
                                        enemy.size.Y <= rect.Height * map.TileSize)
                                        canSpawnIn.Add(new FRectangle((rect.Location.ToVector2() + new Vector2(cx, cy) * map.ChunkSize) * map.TileSize, rect.Size.ToVector2() * map.TileSize));
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
