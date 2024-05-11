using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Threading;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;
using Tendeos.World.Liquid;

namespace Tendeos.World.Shadows
{
    public class ShadowMatrix
    {
        private static BlendState multiplyBlend = new BlendState()
        {
            ColorBlendFunction = BlendFunction.Add,
            ColorSourceBlend = Blend.DestinationColor,
            ColorDestinationBlend = Blend.Zero,
        };

        private readonly SafeList<Light> lights = new SafeList<Light>((a, i) => a[i].available = false);

        private readonly Camera camera;
        private readonly IMap map;
        private readonly WaterWorld waterWorld;
        private Vector3[,] mask;
        private int width, height, validVertexes;
        public bool updated = false;
        public Vector3 DirectionLight;
        public int DirectionLightFrom;
        public int DirectionLightTo;
        public int DirectionLightRange;
        public float DirectionLightIntensity;
        public float DirectionLightAngle;
        public float DirectionLightShadowRange;
        public SmoothPower Smooth;
        private bool isUpscaled;
        public bool IsUpscaled
        {
            get => isUpscaled;
            set
            {
                isUpscaled = value;
                Resize();
            }
        }

        private readonly Batch batch;
        private readonly Thread thread;

        private VertexBuffer vertexBuffer;

        public ShadowMatrix(GraphicsDevice graphicsDevice, IMap map, WaterWorld waterWorld, Camera camera)
        {
            batch = new Batch(graphicsDevice);

            this.map = map;
            this.waterWorld = waterWorld;
            this.camera = camera;

            thread = new Thread(Update);

            Resize();
        }

        public uint Create(Color color, int x, int y, float intensity, float radius) =>
            lights.Add(new Light()
            {
                x = x * map.TileSize,
                y = y * map.TileSize,
                color = color.ToVector3(),
                intensity = intensity,
                radius = radius,
                available = true
            });

        public uint Create(Color color, Vec2 pos, float intensity, float radius) =>
            lights.Add(new Light()
            {
                x = pos.X,
                y = pos.Y,
                color = color.ToVector3(),
                intensity = intensity,
                radius = radius,
                available = true
            });

        public uint Create(Color color, float x, float y, float intensity, float radius) =>
            lights.Add(new Light()
            {
                x = x,
                y = y,
                color = color.ToVector3(),
                intensity = intensity,
                radius = radius,
                available = true
            });

        public void Destroy(uint lit)
        {
            lights.Destroy(lit);
        }

        public void SetPosition(uint lit, float x, float y)
        {
            lights.Mutable[lit].x = x;
            lights.Mutable[lit].y = y;
        }

        public void SetPosition(uint lit, Vec2 position) => SetPosition(lit, position.X, position.Y);

        public void SetIntensity(uint lit, float intensity) => lights.Mutable[lit].intensity = intensity;
        public void SetRadius(uint lit, float radius) => lights.Mutable[lit].radius = radius;

        public void Resize()
        {
            (width, height) = map.World2Cell(camera.WorldViewport);
            width += 4; height += 4;
            if (IsUpscaled)
            {
                width *= 2;
                height *= 2;
            }
            mask = new Vector3[width, height];
        }

        public float Ray(int x0, int y0, (int x, int y) p, float power, bool isUpscaled)
        {
            float f = 0;

            int dx = p.x - x0;
            int dy = p.y - y0;

            float x_incr;
            float y_incr;
            p.x = Math.Abs(dx);
            p.y = Math.Abs(dy);
            if (p.y > p.x)
            {
                (x_incr, y_incr) = (dx / (float)p.y, dy < 0 ? -1 : 1);
                p.x = p.y;
            }
            else (x_incr, y_incr) = (dx < 0 ? -1 : 1, dy / (float)p.x);

            float x = x0;
            float y = y0;
            TileData tile;
            float w;

            for (p.y = 0; p.y < p.x - 1; p.y++)
            {
                y += y_incr;
                x += x_incr;
                if (isUpscaled)
                {
                    dx = (int)MathF.Round(x/2);
                    dy = (int)MathF.Round(y/2);
                }
                else
                {
                    dx = (int)MathF.Round(x);
                    dy = (int)MathF.Round(y);
                }
                if (dx < 0 || dy < 0 || dx >= map.FullWidth || dy >= map.FullHeight) return f;

                tile = map.GetTile(true, dx, dy);
                if (tile.IsReference)
                    tile = map.GetTile(true, (int)tile.GetU32(0), (int)tile.GetU32(32));
                if (tile.Tile?.ShadowAvailable ?? false)
                {
                    if (isUpscaled) f += tile.Tile.ShadowIntensity / 2;
                    else f += tile.Tile.ShadowIntensity;
                    if (f >= power) return power;
                }
                else if ((w = waterWorld.render[dx, dy] / 10) > 0)
                {
                    if (isUpscaled) f += w / 2;
                    else f += w;
                    if (f >= power) return power;
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
            batch.BlendState = multiplyBlend;
            batch.Draw(vertexBuffer, PrimitiveType.TriangleStrip, validVertexes, camera.GetViewMatrix());
        }

        private void Update()
        {
            Thread.CurrentThread.IsBackground = true;
            VertexBuffer vertexBuffer;
            VertexPositionColor[] vertexes;
            int x, y, _x, _y, thisx, thisy, i, vertexCount, z, w, u, l, width, height, sin, cos;
            SmoothPower smooth;
            bool isUpscaled;
            float d2, d1;
            Vec2 a, b, c, d, e, f, g, h, j;
            Color ac, bc, cc, dc, ec, fc, gc, hc, jc;
            Vector3 at, bt, ct, dt, et, ft, gt, ht, jt;
            Vector3[,] mask;
            Light p;
            Rectangle rectangle;
            while (true)
            {
                isUpscaled = IsUpscaled;
                (thisx, thisy) = map.World2Cell(camera.Position - camera.WorldViewport / 2);
                (width, height) = (this.width, this.height);
                mask = this.mask;
                thisx -= 2;
                thisy -= 2;
                if (IsUpscaled)
                {
                    d1 = map.TileSize / 2;
                    thisx *= 2;
                    thisy *= 2;
                }
                else d1 = map.TileSize;
                d2 = d1 / 2;

                #region COMPUTE LIGHTING
                for (x = 0; x < width; x++)
                    for (y = 0; y < height; y++)
                        mask[x, y] = Vector3.Zero;

                if (isUpscaled)
                {
                    sin = (int)(MathF.Sin(DirectionLightAngle) * DirectionLightShadowRange * 2);
                    cos = (int)(MathF.Cos(DirectionLightAngle) * DirectionLightShadowRange * 2);
                }
                else
                {
                    sin = (int)(MathF.Sin(DirectionLightAngle) * DirectionLightShadowRange);
                    cos = (int)(MathF.Cos(DirectionLightAngle) * DirectionLightShadowRange);
                }
                if (DirectionLightIntensity > 0)
                    for (_x = 0; _x < width; _x++)
                        for (_y = 0; _y < height; _y++)
                        {
                            x = thisx + _x;
                            y = thisy + _y;
                            if (x < 0 || y < 0 || x >= map.FullWidth || y >= map.FullHeight) continue;

                            if (isUpscaled)
                            {
                                mask[_x, _y] += DirectionLight
                                    * (1 - Ray(x, y, (x - sin, y + cos), DirectionLightIntensity, true) / DirectionLightIntensity) // shadow
                                    * Math.Clamp(1 - MathF.Max(DirectionLightFrom*2 - DirectionLightRange*2 - y, y - (DirectionLightTo*2 + DirectionLightRange*2)) / (DirectionLightRange*2), 0, 1) * DirectionLightIntensity; // saturation
                            }
                            else
                            {
                                mask[_x, _y] += DirectionLight
                                    * (1 - Ray(x, y, (x - sin, y + cos), DirectionLightIntensity, false) / DirectionLightIntensity) // shadow
                                    * Math.Clamp(1 - MathF.Max(DirectionLightFrom - DirectionLightRange - y, y - (DirectionLightTo + DirectionLightRange)) / DirectionLightRange, 0, 1) * DirectionLightIntensity; // saturation
                            }
                        }

                for (i = 0; i < lights.Max; i++)
                {
                    p = lights[i];
                    if (!p.available || p.intensity <= 0 || p.radius <= 0) continue;
                    rectangle = p.Generate(map);
                    if (IsUpscaled)
                    {
                        rectangle.X *= 2;
                        rectangle.Y *= 2;
                        rectangle.Width *= 2;
                        rectangle.Height *= 2;
                    }
                    for (_x = rectangle.X; _x <= rectangle.X + rectangle.Width; _x++)
                        for (_y = rectangle.Y; _y <= rectangle.Y + rectangle.Height; _y++)
                        {
                            if (IsUpscaled)
                            {
                                x = _x - thisx;
                                y = _y - thisy;
                            }
                            else
                            {
                                x = _x - thisx;
                                y = _y - thisy;
                            }
                            if (x < 0 || x >= width || y < 0 || y >= height) continue;

                            mask[x, y] += p.color
                                * (1 - Ray(_x, _y, isUpscaled ? map.World2Cell(p.x+p.x, p.y+p.y) : map.World2Cell(p.x, p.y), p.intensity, isUpscaled) / p.intensity) // shadow
                                * MathF.Max(0, 1 - new Vec2(p.x / d1 - _x, p.y / d1 - _y).Length() / (p.radius + (isUpscaled ? p.radius : 0))) * p.intensity; // saturation
                        }
                }
                #endregion

                smooth = Smooth;
                vertexCount = width * height * (int)smooth;
                (vertexBuffer, vertexes) = batch.CreateBuffer<VertexPositionColor>(vertexCount);
                i = 0;
                for (x = 0; x < width; x++)
                    for (y = 0; y < height; y++)
                    {
                        _x = thisx + x;
                        _y = thisy + y;
                        if (smooth == SmoothPower.No)
                        {
                            ac = new Color(mask[x, y]);
                            a = new Vec2(_x, _y) * d1;
                            b = new Vec2(a.X + d1, a.Y + d1);
                            c = new Vec2(a.X, b.Y);
                            d = new Vec2(b.X, a.Y);
                            vertexes[i++] = new VertexPositionColor(b, ac);
                            vertexes[i++] = new VertexPositionColor(b, ac);
                            vertexes[i++] = new VertexPositionColor(d, ac);
                            vertexes[i++] = new VertexPositionColor(c, ac);
                            vertexes[i++] = new VertexPositionColor(a, ac);
                            vertexes[i++] = new VertexPositionColor(a, ac);
                        }
                        else if (smooth == SmoothPower.Blocky)
                        {
                            b = new Vec2(_x, _y) * d1;
                            a = new Vec2(b.X + d2, b.Y + d2);
                            c = new Vec2(b.X + d1, b.Y);
                            d = new Vec2(b.X, b.Y + d1);
                            e = new Vec2(b.X + d1, b.Y + d1);
                            w = x == width - 1 ? width - 1 : x + 1;
                            z = x == 0 ? 0 : x - 1;
                            u = y == height - 1 ? height - 1 : y + 1;
                            l = y == 0 ? 0 : y - 1;
                            at = mask[x, y];
                            bt = mask[z, y];
                            ct = mask[w, y];
                            dt = mask[x, l];
                            et = mask[x, u];
                            ft = mask[z, l];
                            gt = mask[w, l];
                            ht = mask[z, u];
                            jt = mask[w, u];
                            ft = Mix(at, bt, dt, ft);
                            gt = Mix(at, ct, dt, gt);
                            ht = Mix(at, bt, et, ht);
                            jt = Mix(at, ct, et, jt);
                            ac = new Color(at);
                            bc = new Color(ft);
                            cc = new Color(gt);
                            dc = new Color(ht);
                            ec = new Color(jt);

                            vertexes[i++] = new VertexPositionColor(e, ec);
                            vertexes[i++] = new VertexPositionColor(e, ec);
                            vertexes[i++] = new VertexPositionColor(c, cc);
                            vertexes[i++] = new VertexPositionColor(a, ac);
                            vertexes[i++] = new VertexPositionColor(b, bc);
                            vertexes[i++] = new VertexPositionColor(d, dc);
                            vertexes[i++] = new VertexPositionColor(d, dc);
                            vertexes[i++] = new VertexPositionColor(a, ac);
                            vertexes[i++] = new VertexPositionColor(e, ec);
                            vertexes[i++] = new VertexPositionColor(e, ec);
                        }
                        else
                        {
                            f = new Vec2(_x, _y) * d1;
                            a = new Vec2(f.X + d2, f.Y + d2);
                            b = new Vec2(f.X, f.Y + d2);
                            c = new Vec2(f.X + d1, f.Y + d2);
                            d = new Vec2(f.X + d2, f.Y);
                            e = new Vec2(f.X + d2, f.Y + d1);
                            g = new Vec2(f.X + d1, f.Y);
                            h = new Vec2(f.X, f.Y + d1);
                            j = new Vec2(f.X + d1, f.Y + d1);
                            w = x == width - 1 ? width - 1 : x + 1;
                            z = x == 0 ? 0 : x - 1;
                            u = y == height - 1 ? height - 1 : y + 1;
                            l = y == 0 ? 0 : y - 1;
                            at = mask[x, y];
                            bt = mask[z, y];
                            ct = mask[w, y];
                            dt = mask[x, l];
                            et = mask[x, u];
                            ft = mask[z, l];
                            gt = mask[w, l];
                            ht = mask[z, u];
                            jt = mask[w, u];
                            ft = Mix(at, bt, dt, ft);
                            gt = Mix(at, ct, dt, gt);
                            ht = Mix(at, bt, et, ht);
                            jt = Mix(at, ct, et, jt);
                            bt = Mix(bt, at);
                            ct = Mix(ct, at);
                            dt = Mix(dt, at);
                            et = Mix(et, at);
                            ac = new Color(at);
                            bc = new Color(bt);
                            cc = new Color(ct);
                            dc = new Color(dt);
                            ec = new Color(et);
                            fc = new Color(ft);
                            gc = new Color(gt);
                            hc = new Color(ht);
                            jc = new Color(jt);

                            vertexes[i++] = new VertexPositionColor(g, gc);
                            vertexes[i++] = new VertexPositionColor(g, gc);
                            vertexes[i++] = new VertexPositionColor(d, dc);
                            vertexes[i++] = new VertexPositionColor(c, cc);
                            vertexes[i++] = new VertexPositionColor(a, ac);
                            vertexes[i++] = new VertexPositionColor(b, bc);
                            vertexes[i++] = new VertexPositionColor(e, ec);
                            vertexes[i++] = new VertexPositionColor(h, hc);
                            vertexes[i++] = new VertexPositionColor(j, jc);
                            vertexes[i++] = new VertexPositionColor(e, ec);
                            vertexes[i++] = new VertexPositionColor(c, cc);
                            vertexes[i++] = new VertexPositionColor(a, ac);
                            vertexes[i++] = new VertexPositionColor(b, bc);
                            vertexes[i++] = new VertexPositionColor(d, dc);
                            vertexes[i++] = new VertexPositionColor(f, fc);
                            vertexes[i++] = new VertexPositionColor(f, fc);
                        }
                    }

                vertexBuffer.SetData(vertexes);
                this.vertexBuffer = vertexBuffer;
                validVertexes = i;
                Thread.Sleep(1);
            }
        }

        public void Start()
        {
            thread.Start();
        }

        public enum SmoothPower : byte { No = 6, Blocky = 10, Diamondly = 30 }
    }
}