using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Threading;
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
        private Vector3[,] render, mask;
        private int width, height, x, y;
        public bool updated = false;
        public Vector3 DirectionLight { get; set; }
        public int DirectionLightFrom { get; set; }
        public int DirectionLightTo { get; set; }
        public int DirectionLightRange { get; set; }
        public float DirectionLightIntensity { get; set; }
        public float DirectionLightAngle { get; set; }
        public float DirectionLightShadowRange { get; set; }
        public SmoothPower Smooth { get; set; }

        private readonly Batch batch;
        private readonly Thread thread;

        public ShadowMatrix(GraphicsDevice graphicsDevice, IMap map, WaterWorld waterWorld, Camera camera)
        {
            batch = new Batch(graphicsDevice);

            this.map = map;
            this.waterWorld = waterWorld;
            this.camera = camera;

            thread = new Thread(Update);

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
            mask = new Vector3[width, height];
            render = new Vector3[width, height];
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

        private Vector3 Mix(Vector3 a, Vector3 b)
        {
            const float pow = 0.5f;
            return new Vector3((a.X + b.X) * pow, (a.Y + b.Y) * pow, (a.Z + b.Z) * pow);
        }

        static Vector3 Mix(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            const float pow = 0.25f;
            return new Vector3((a.X + b.X + c.X + d.X) * pow, (a.Y + b.Y + c.Y + d.Y) * pow, (a.Z + b.Z + c.Z + d.Z) * pow);
        }

        public void Draw()
        {
            batch.Begin(PrimitiveType.TriangleList, width * height * (int)Smooth, camera.GetViewMatrix());
            batch.BlendState = multiplyBlend;

            int x, y, _x, _y;

            if (updated)
                for (x = 0; x < width; x++)
                    for (y = 0; y < height; y++)
                        render[x, y] = mask[x, y];

            for (x = 0; x < width; x++)
                for (y = 0; y < height; y++)
                {
                    _x = this.x + x;
                    _y = this.y + y;
                    if (Smooth == SmoothPower.No)
                    {
                        batch.Color = new Color(render[x, y]);
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
                    else if (Smooth == SmoothPower.Blocky)
                    {
                        Vec2
                            c = map.Cell2World(_x, _y),
                            l = map.Cell2World(_x - 1, _y),
                            r = map.Cell2World(_x + 1, _y),
                            b = map.Cell2World(_x, _y - 1),
                            t = map.Cell2World(_x, _y + 1);
                        Color
                            rc = new Color(render[x == width - 1 ? width - 1 : x + 1, y]),
                            lc = new Color(render[x == 0 ? 0 : x - 1, y]),
                            bc = new Color(render[x, y == 0 ? 0 : y - 1]),
                            tc = new Color(render[x, y == height - 1 ? height - 1 : y + 1]),
                            cc = new Color(render[x, y]);

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
                        Vec2
                            c = map.Cell2World(_x, _y),
                            l = new Vec2(_x, _y + 0.5f) * map.TileSize,
                            r = new Vec2(_x + 1, _y + 0.5f) * map.TileSize,
                            b = new Vec2(_x + 0.5f, _y) * map.TileSize,
                            t = new Vec2(_x + 0.5f, _y + 1) * map.TileSize,
                            lb = new Vec2(_x, _y) * map.TileSize,
                            rb = new Vec2(_x + 1, _y) * map.TileSize,
                            lt = new Vec2(_x, _y + 1) * map.TileSize,
                            rt = new Vec2(_x + 1, _y + 1) * map.TileSize;
                        int ri = x == width - 1 ? width - 1 : x + 1,
                            li = x == 0 ? 0 : x - 1,
                            ti = y == height - 1 ? height - 1 : y + 1,
                            bi = y == 0 ? 0 : y - 1;
                        Vector3 cv = render[x, y],
                            rv = render[ri, y],
                            lv = render[li, y],
                            bv = render[x, bi],
                            tv = render[x, ti],
                            rtv = render[ri, ti],
                            ltv = render[li, ti],
                            rbv = render[ri, bi],
                            lbv = render[li, bi];
                        rtv = Mix(cv, rv, tv, rtv);
                        ltv = Mix(cv, lv, tv, ltv);
                        rbv = Mix(cv, rv, bv, rbv);
                        lbv = Mix(cv, lv, bv, lbv);
                        rv = Mix(rv, cv);
                        lv = Mix(lv, cv);
                        bv = Mix(bv, cv);
                        tv = Mix(tv, cv);
                        Color
                            cc = new Color(cv),
                            rc = new Color(rv),
                            lc = new Color(lv),
                            bc = new Color(bv),
                            tc = new Color(tv),
                            rtc = new Color(rtv),
                            ltc = new Color(ltv),
                            rbc = new Color(rbv),
                            lbc = new Color(lbv);

                        batch.Color = lc;
                        batch.Vertex(l);
                        batch.Color = tc;
                        batch.Vertex(t);
                        batch.Color = cc;
                        batch.Vertex(c);

                        batch.Color = lc;
                        batch.Vertex(l);
                        batch.Color = tc;
                        batch.Vertex(t);
                        batch.Color = ltc;
                        batch.Vertex(lt);

                        batch.Color = lc;
                        batch.Vertex(l);
                        batch.Color = bc;
                        batch.Vertex(b);
                        batch.Color = cc;
                        batch.Vertex(c);

                        batch.Color = lc;
                        batch.Vertex(l);
                        batch.Color = bc;
                        batch.Vertex(b);
                        batch.Color = lbc;
                        batch.Vertex(lb);

                        batch.Color = rc;
                        batch.Vertex(r);
                        batch.Color = tc;
                        batch.Vertex(t);
                        batch.Color = cc;
                        batch.Vertex(c);

                        batch.Color = rc;
                        batch.Vertex(r);
                        batch.Color = tc;
                        batch.Vertex(t);
                        batch.Color = rtc;
                        batch.Vertex(rt);

                        batch.Color = rc;
                        batch.Vertex(r);
                        batch.Color = bc;
                        batch.Vertex(b);
                        batch.Color = cc;
                        batch.Vertex(c);

                        batch.Color = rc;
                        batch.Vertex(r);
                        batch.Color = bc;
                        batch.Vertex(b);
                        batch.Color = rbc;
                        batch.Vertex(rb);
                    }
                }
            batch.End();

            if (updated) updated = false;
        }

        private void Update()
        {
            Thread.CurrentThread.IsBackground = true;
            int x, y, _x, _y, thisx, thisy;
            while (true)
            {
                (thisx, thisy) = map.World2Cell(camera.Position - camera.WorldViewport / 2);
                thisx -= 2; thisy -= 2;

                for (x = 0; x < width; x++)
                    for (y = 0; y < height; y++)
                        mask[x, y] = Vector3.Zero;

                if (DirectionLightIntensity > 0)
                    for (_x = 0; _x < width; _x++)
                        for (_y = 0; _y < height; _y++)
                        {
                            x = thisx + _x;
                            y = thisy + _y;
                            if (x < 0 || y < 0 || x >= map.FullWidth || y >= map.FullHeight) continue;

                            mask[_x, _y] += DirectionLight
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
                            x = _x - thisx;
                            y = _y - thisy;
                            if (x < 0 || x >= width || y < 0 || y >= height) continue;

                            mask[x, y] += p.color
                                * (1 - Ray(_x, _y, map.World2Cell(p.x, p.y), p.intensity) / p.intensity) // shadow
                                * MathF.Max(0, 1 - new Vec2(p.x / map.TileSize - _x, p.y / map.TileSize - _y).Length() / p.radius) * p.intensity; // saturation
                        }
                }

                this.x = thisx;
                this.y = thisy;
                updated = true;
                while (updated)
                {
                    Thread.Sleep(1);
                }
            }
        }

        public void Start()
        {
            thread.Start();
        }

        public enum SmoothPower : byte { No = 6, Blocky = 12, Diamondly = 24 }
    }
}