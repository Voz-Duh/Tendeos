using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using XnaGame.Utils;
using XnaGame.Utils.Graphics;
using XnaGame.World.Liquid;

namespace XnaGame.World.Shadows
{
    public class ShadowMatrix
    {
        private static BlendState multiplyBlend = new BlendState()
        {
            ColorBlendFunction = BlendFunction.Add,
            ColorSourceBlend = Blend.DestinationColor,
            ColorDestinationBlend = Blend.Zero,
        };

        private readonly Queue<uint> free = new Queue<uint>();
        private uint length;
        private readonly Light[] lights = new Light[255];

        private readonly Camera camera;
        private readonly IMap map;
        private readonly WaterWorld waterWorld;
        private Vector3[,] tiles;
        private int width, height;
        public Vector3 DirectionLight { get; set; }
        public int DirectionLightFrom { get; set; }
        public int DirectionLightTo { get; set; }
        public int DirectionLightRange { get; set; }
        public float DirectionLightIntensity { get; set; }
        public float DirectionLightAngle { get; set; }
        public float DirectionLightShadowRange { get; set; }
        public bool Smooth { get; set; }

        private readonly Batch batch;

        public ShadowMatrix(GraphicsDevice graphicsDevice, IMap map, WaterWorld waterWorld, Camera camera)
        {
            batch = new Batch(graphicsDevice);

            this.map = map;
            this.waterWorld = waterWorld;
            this.camera = camera;

            Resize();
        }

        public uint Create(Color color, int x, int y, float intensity, float radius)
        {
            if (free.TryDequeue(out uint f))
            {
                lights[f].x = x;
                lights[f].y = y;
                lights[f].color = color.ToVector3();
                lights[f].intensity = intensity;
                lights[f].radius = radius;
                lights[f].available = true;
                return f;
            }
            else
            {
                lights[length].x = x;
                lights[length].y = y;
                lights[length].color = color.ToVector3();
                lights[length].intensity = intensity;
                lights[length].radius = radius;
                lights[length].available = true;
                length++;
                return length - 1;
            }
        }

        public void Destroy(uint lit)
        {
            lights[lit].available = false;
            free.Enqueue(lit);
        }

        public void SetPosition(uint lit, float x, float y)
        {
            lights[lit].x = x;
            lights[lit].y = y;
        }

        public void SetPosition(uint lit, Vec2 position) => SetPosition(lit, position.X, position.Y);

        public void SetIntensity(uint lit, float intensity) => lights[lit].intensity = intensity;
        public void SetRadius(uint lit, float radius) => lights[lit].radius = radius;

        public void Resize()
        {
            (width, height) = map.World2Cell(camera.WorldViewport);
            width += 4; height += 4;
            tiles = new Vector3[width, height];
        }

        public float Ray(int x0, int y0, (int, int) p, float power)
        {
            var (x1, y1) = p;
            float f = 0;

            int dx = x1 - x0;
            int dy = y1 - y0;

            float sd = new Vec2(Math.Abs(dx), Math.Abs(dy)).Length();

            float x_incr = dx / sd;
            float y_incr = dy / sd;

            float x = x0;
            float y = y0;
            TileData tile;
            float w;

            for (int i = 0; i < sd - 1; i++)
            {
                y += y_incr;
                x += x_incr;
                dx = (int)MathF.Round(x);
                dy = (int)MathF.Round(y);
                if (dx < 0 || dy < 0 || dx >= map.FullWidth || dy >= map.FullHeight) return f;
                tile = map.GetTile(true, dx, dy);
                if (tile.IsReference)
                    tile = map.GetTile(true, BitConverter.ToInt32(tile.Data), BitConverter.ToInt32(tile.Data, 4));
                if (tile.Tile?.ShadowAvailable ?? false)
                {
                    f += tile.Tile.ShadowIntensity;
                    if (f >= power) return power;
                }
                else
                {
                    w = waterWorld.render[dx, dy] / 10;
                    if (w >= 0)
                    {
                        f += w;
                        if (f >= power) return power;
                    }
                }
            }
            return f;
        }

        public void Draw()
        {
            var (selfx, selfy) = map.World2Cell(camera.Position - camera.WorldViewport / 2);
            selfx -= 2; selfy -= 2;

            int x, y, _x, _y;

            for (x = 0; x < width; x++)
                for (y = 0; y < height; y++)
                    tiles[x, y] = Vector3.Zero;

            if (DirectionLightIntensity > 0)
                for (_x = 0; _x < width; _x++)
                    for (_y = 0; _y < height; _y++)
                    {
                        x = selfx + _x;
                        y = selfy + _y;
                        if (x < 0 || y < 0 || x >= map.FullWidth || y >= map.FullHeight) continue;

                        tiles[_x, _y] += DirectionLight
                            * (1 - Ray(x, y, (x - (int)(MathF.Sin(DirectionLightAngle) * DirectionLightShadowRange), y + (int)(MathF.Cos(DirectionLightAngle) * DirectionLightShadowRange)), DirectionLightIntensity) / DirectionLightIntensity) // shadow
                            * Math.Clamp(1 - MathF.Max(DirectionLightFrom - DirectionLightRange - y, y - (DirectionLightTo + DirectionLightRange)) / DirectionLightRange, 0, 1) * DirectionLightIntensity; // saturation
                    }

            foreach (Light p in lights)
            {
                if (!p.available || p.intensity <= 0 || p.radius <= 0) continue;
                p.Generate(map);
                for (_x = p.rectangle.X; _x <= p.rectangle.X + p.rectangle.Width; _x++)
                    for (_y = p.rectangle.Y; _y <= p.rectangle.Y + p.rectangle.Height; _y++)
                    {
                        x = _x - selfx;
                        y = _y - selfy;
                        if (x < 0 || x >= width || y < 0 || y >= height) continue;

                        tiles[x, y] += p.color
                            * (1 - Ray(_x, _y, map.World2Cell(p.x, p.y), p.intensity) / p.intensity) // shadow
                            * MathF.Max(0, 1 - new Vec2(p.x / map.TileSize - _x, p.y / map.TileSize - _y).Length() / p.radius) * p.intensity; // saturation
                    }
            }
            batch.Begin(PrimitiveType.TriangleList, width * height * (Smooth ? 12 : 6), camera.GetViewMatrix());
            batch.BlendState = multiplyBlend;
            for (x = 0; x < width; x++)
                for (y = 0; y < height; y++)
                {
                    _x = selfx + x;
                    _y = selfy + y;
                    if (Smooth)
                    {
                        Vec2
                            c = map.Cell2World(_x, _y),
                            l = map.Cell2World(_x - 1, _y),
                            r = map.Cell2World(_x + 1, _y),
                            b = map.Cell2World(_x, _y - 1),
                            t = map.Cell2World(_x, _y + 1);
                        Color
                            rc = new Color(tiles[x == width - 1 ? width - 1 : x + 1, y]),
                            lc = new Color(tiles[x == 0 ? 0 : x - 1, y]),
                            bc = new Color(tiles[x, y == 0 ? 0 : y - 1]),
                            tc = new Color(tiles[x, y == height - 1 ? height - 1 : y + 1]),
                            cc = new Color(tiles[x, y]);

                        batch.Color = lc;
                        batch.Vertex(l);
                        batch.Color = tc;
                        batch.Vertex(t);
                        batch.Color = cc;
                        batch.Vertex(c);

                        batch.Color = lc;
                        batch.Vertex(l);
                        batch.Color = bc;
                        batch.Vertex(b);
                        batch.Color = cc;
                        batch.Vertex(c);

                        batch.Color = rc;
                        batch.Vertex(r);
                        batch.Color = tc;
                        batch.Vertex(t);
                        batch.Color = cc;
                        batch.Vertex(c);

                        batch.Color = rc;
                        batch.Vertex(r);
                        batch.Color = bc;
                        batch.Vertex(b);
                        batch.Color = cc;
                        batch.Vertex(c);
                    }
                    else
                    {
                        batch.Color = new Color(tiles[x, y]);
                        Vec2
                            m = new Vec2(_x, _y) * map.TileSize,
                            p = new Vec2(_x + 1, _y + 1) * map.TileSize;
                        batch.Vertex(m);
                        batch.Vertex3(m.X, p.Y, 0);
                        batch.Vertex3(p.X, m.Y, 0);
                        batch.Vertex3(p.X, m.Y, 0);
                        batch.Vertex3(m.X, p.Y, 0);
                        batch.Vertex(p);
                    }
                }
            batch.End();
        }
    }
}