using Microsoft.Xna.Framework;
using System;
using XnaGame.Utils;
using XnaGame.Utils.Graphics;

namespace XnaGame.World.Liquid
{
    public class WaterWorld
    {
        public const float tileSize = Map.tileSize;

        public float[,] cells;
        public readonly float[,] render;
        public readonly int width;
        public readonly int height;

        private float timer = 0;
        private const float time = .01f;

        public WaterWorld(int width, int height, WorldGenerator generator)
        {
            cells = new float[width, height];
            render = new float[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    render[i, j] = cells[i, j] = generator.GetWater(i, j) ? 1 : 0;
                }
            }
            this.width = width;
            this.height = height;
        }
        public bool CanFlow(float other)
        {
            if (other == -1) return false;
            if (other >= 1) return false;

            return true;
        }

        public bool CanPush(float other)
        {
            if (other == -1) return false;
            if (other >= 0.5) return false;

            return true;
        }

        public float Move(float pop, float r, ref float other)
        {
            float sum = r + pop;
            if (sum > 1)
            {
                other += 1 - r;
                return sum - 1;
            }
            other += pop;
            return 0;
        }

        public void Move(ref float from, ref float other)
        {
            float sum = other + from;
            if (sum > 1)
            {
                other = 1;
                from = sum - 1;
            }
            other += from;
        }

        public void Update()
        {
            timer += Time.Delta;
            while (timer >= time)
            {
                Tick();
                timer -= time;
            }
        }

        public void Tick()
        {
            float[,] mask = new float[width, height];
            
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    float self = cells[i, j];
                    if (self == -1)
                    {
                        mask[i, j] = -1;
                        continue;
                    }
                    if (self == 0) continue;
                    bool
                        b_l = false,
                        b_r = false,
                        b_d = false,
                        b_u = false;
                    float l = i == 0 ? -1 : cells[i - 1, j];
                    float r = i == width - 1 ? -1 : cells[i + 1, j];
                    float d = j == height - 1 ? -1 : cells[i, j + 1];
                    float u = j == 0 ? -1 : cells[i, j - 1];

                    b_d = CanFlow(d);

                    if (!CanPush(d))
                    {
                        b_l = CanFlow(l);
                        if (b_l && l > self) b_l = false;
                        b_r = CanFlow(r);
                        if (b_r && r > self) b_r = false;
                    }

                    if (!b_d && !b_l && !b_r)
                    {
                        b_u = CanPush(u);
                    }

                    int c = 1;
                    if (b_l) c++;
                    if (b_r) c++;
                    if (b_d) c++;
                    if (b_u) c++;
                    float sum = self / c;

                    self = sum;
                    if (b_l) self += Move(sum, l, ref mask[i - 1, j]);
                    if (b_r) self += Move(sum, r, ref mask[i + 1, j]);
                    if (b_d) self += Move(sum, d, ref mask[i, j + 1]);
                    if (b_u) self += Move(sum, u, ref mask[i, j - 1]);

                    mask[i, j] += self;
                }
            }
            
            for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                {
                    if (cells[i, j] == -1) render[i, j] = -1;
                    else render[i, j] = MathHelper.Lerp(render[i, j], cells[i, j], .04f);
                }

            cells = mask;
        }

        private readonly Batch batch = new Batch(SDraw.spriteBatch.GraphicsDevice);
        public void Draw()
        {
            batch.Begin(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, (width * height) * 6, SDraw.Matrix);
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    float s = render[i, j];
                    if (s <= 0.1) continue;

                    bool cl = i != 0;
                    bool cr = i != width - 1;
                    bool cu = j != 0;
                    bool cd = j != height - 1;
                    float l = cl ? render[i - 1, j] : -1;
                    float r = cr ? render[i + 1, j] : -1;
                    float d = cd ? render[i, j + 1] : -1;
                    float u = cu ? render[i, j - 1] : -1;
                    float lu = cl && cu ? render[i - 1, j - 1] : -1;
                    float ld = cl && cd ? render[i - 1, j + 1] : -1;
                    float ru = cr && cu ? render[i + 1, j - 1] : -1;
                    float rd = cr && cd ? render[i + 1, j + 1] : -1;

                    if (d == -1) d = s;
                    if (r == -1) r = s;
                    if (l == -1) l = s;
                    if (u == -1) u = s;
                    if (lu == -1) lu = s;
                    if (ru == -1) ru = s;
                    if (ld == -1) ld = s;
                    if (rd == -1) rd = s;

                    bool
                        bl = l > 0.1,
                        br = r > 0.1,
                        bu = u > 0.1,
                        bd = d > 0.1,
                        brd = rd > 0.1,
                        bru = ru > 0.1,
                        bld = ld > 0.1,
                        blu = lu > 0.1;

                    bool has = false;
                    batch.Color = Color.Aqua;
                    for (int m = 0; m < march.Length; m++)
                    {
                        (bool lu, bool u, bool ru, bool l, bool r, bool ld, bool d, bool rd,
                            (Vec2 a, Vec2 b, Vec2 c, byte at, byte bt, byte ct, Vec2 ac, Vec2 bc, Vec2 cc)[] triangles) marching = march[m];
                        if ((marching.l ? bl : !bl) && (marching.r ? br : !br) && (marching.d ? bd : !bd) && (marching.u ? bu : !bu) &&
                            (marching.rd ? brd : !brd) && (marching.ru ? bru : !bru) && (marching.ld ? bld : !bld) && (marching.lu ? blu : !blu))
                        {
                            foreach (var (a, b, c, at, bt, ct, ac, bc, cc) in marching.triangles)
                            {
                                float
                                    ap = GetByType(at, false, false, false, s, l, r, d, u, lu, ld, ru, rd),
                                    bp = GetByType(bt, false, false, false, s, l, r, d, u, lu, ld, ru, rd),
                                    cp = GetByType(ct, false, false, false, s, l, r, d, u, lu, ld, ru, rd);
                                batch.Vertex((new Vec2(i, j) + ac + a * ap) * tileSize);
                                batch.Vertex((new Vec2(i, j) + bc + b * bp) * tileSize);
                                batch.Vertex((new Vec2(i, j) + cc + c * cp) * tileSize);
                            }
                            has = true;
                            break;
                        }
                        if ((marching.r ? bl : !bl) && (marching.l ? br : !br) && (marching.u ? bd : !bd) && (marching.d ? bu : !bu) &&
                            (marching.lu ? brd : !brd) && (marching.ld ? bru : !bru) && (marching.ru ? bld : !bld) && (marching.rd ? blu : !blu))
                        {
                            foreach (var (a, b, c, at, bt, ct, ac, bc, cc) in marching.triangles)
                            {
                                float
                                    ap = GetByType(at, true, true, false, s, l, r, d, u, lu, ld, ru, rd),
                                    bp = GetByType(bt, true, true, false, s, l, r, d, u, lu, ld, ru, rd),
                                    cp = GetByType(ct, true, true, false, s, l, r, d, u, lu, ld, ru, rd);
                                batch.Vertex((new Vec2(i, j) + IC(ac) - a * ap) * tileSize);
                                batch.Vertex((new Vec2(i, j) + IC(bc) - b * bp) * tileSize);
                                batch.Vertex((new Vec2(i, j) + IC(cc) - c * cp) * tileSize);
                            }
                            has = true;
                            break;
                        }
                        if ((marching.u ? bl : !bl) && (marching.d ? br : !br) && (marching.l ? bd : !bd) && (marching.r ? bu : !bu) &&
                            (marching.ld ? brd : !brd) && (marching.rd ? bru : !bru) && (marching.lu ? bld : !bld) && (marching.ru ? blu : !blu))
                        {
                            foreach (var (a, b, c, at, bt, ct, ac, bc, cc) in marching.triangles)
                            {
                                float
                                    ap = GetByType(at, false, true, true, s, l, r, d, u, lu, ld, ru, rd),
                                    bp = GetByType(bt, false, true, true, s, l, r, d, u, lu, ld, ru, rd),
                                    cp = GetByType(ct, false, true, true, s, l, r, d, u, lu, ld, ru, rd);
                                batch.Vertex((new Vec2(i, j) + ICX(IC(new Vec2(ac.Y, ac.X))) - IX(new Vec2(a.Y, a.X)) * ap) * tileSize);
                                batch.Vertex((new Vec2(i, j) + ICX(IC(new Vec2(bc.Y, bc.X))) - IX(new Vec2(b.Y, b.X)) * bp) * tileSize);
                                batch.Vertex((new Vec2(i, j) + ICX(IC(new Vec2(cc.Y, cc.X))) - IX(new Vec2(c.Y, c.X)) * cp) * tileSize);
                            }
                            has = true;
                            break;
                        }
                        if ((marching.d ? bl : !bl) && (marching.u ? br : !br) && (marching.r ? bd : !bd) && (marching.l ? bu : !bu) &&
                            (marching.ru ? brd : !brd) && (marching.lu ? bru : !bru) && (marching.rd ? bld : !bld) && (marching.ld ? blu : !blu))
                        {
                            foreach (var (a, b, c, at, bt, ct, ac, bc, cc) in marching.triangles)
                            {
                                float
                                    ap = GetByType(at, true, false, true, s, l, r, d, u, lu, ld, ru, rd),
                                    bp = GetByType(bt, true, false, true, s, l, r, d, u, lu, ld, ru, rd),
                                    cp = GetByType(ct, true, false, true, s, l, r, d, u, lu, ld, ru, rd);
                                batch.Vertex((new Vec2(i, j) + ICX(new Vec2(ac.Y, ac.X)) + IX(new Vec2(a.Y, a.X)) * ap) * tileSize);
                                batch.Vertex((new Vec2(i, j) + ICX(new Vec2(bc.Y, bc.X)) + IX(new Vec2(b.Y, b.X)) * bp) * tileSize);
                                batch.Vertex((new Vec2(i, j) + ICX(new Vec2(cc.Y, cc.X)) + IX(new Vec2(c.Y, c.X)) * cp) * tileSize);
                            }
                            has = true;
                            break;
                        }

                        if ((marching.r ? bl : !bl) && (marching.l ? br : !br) && (marching.d ? bd : !bd) && (marching.u ? bu : !bu) &&
                            (marching.ld ? brd : !brd) && (marching.lu ? bru : !bru) && (marching.rd ? bld : !bld) && (marching.ru ? blu : !blu))
                        {
                            foreach (var (a, b, c, at, bt, ct, ac, bc, cc) in marching.triangles)
                            {
                                float
                                    ap = GetByType(at, true, false, false, s, l, r, d, u, lu, ld, ru, rd),
                                    bp = GetByType(bt, true, false, false, s, l, r, d, u, lu, ld, ru, rd),
                                    cp = GetByType(ct, true, false, false, s, l, r, d, u, lu, ld, ru, rd);
                                batch.Vertex((new Vec2(i, j) + ICX(ac) + IX(a) * ap) * tileSize);
                                batch.Vertex((new Vec2(i, j) + ICX(bc) + IX(b) * bp) * tileSize);
                                batch.Vertex((new Vec2(i, j) + ICX(cc) + IX(c) * cp) * tileSize);
                            }
                            has = true;
                            break;
                        }
                        if ((marching.l ? bl : !bl) && (marching.r ? br : !br) && (marching.u ? bd : !bd) && (marching.d ? bu : !bu) &&
                            (marching.ru ? brd : !brd) && (marching.rd ? bru : !bru) && (marching.lu ? bld : !bld) && (marching.ld ? blu : !blu))
                        {
                            foreach (var (a, b, c, at, bt, ct, ac, bc, cc) in marching.triangles)
                            {
                                float
                                    ap = GetByType(at, false, true, false, s, l, r, d, u, lu, ld, ru, rd),
                                    bp = GetByType(bt, false, true, false, s, l, r, d, u, lu, ld, ru, rd),
                                    cp = GetByType(ct, false, true, false, s, l, r, d, u, lu, ld, ru, rd);
                                batch.Vertex((new Vec2(i, j) + ICX(IC(ac)) - IX(a) * ap) * tileSize);
                                batch.Vertex((new Vec2(i, j) + ICX(IC(bc)) - IX(b) * bp) * tileSize);
                                batch.Vertex((new Vec2(i, j) + ICX(IC(cc)) - IX(c) * cp) * tileSize);
                            }
                            has = true;
                            break;
                        }
                        if ((marching.d ? bl : !bl) && (marching.u ? br : !br) && (marching.l ? bd : !bd) && (marching.r ? bu : !bu) &&
                            (marching.lu ? brd : !brd) && (marching.ru ? bru : !bru) && (marching.ld ? bld : !bld) && (marching.rd ? blu : !blu))
                        {
                            foreach (var (a, b, c, at, bt, ct, ac, bc, cc) in marching.triangles)
                            {
                                float
                                    ap = GetByType(at, false, false, true, s, l, r, d, u, lu, ld, ru, rd),
                                    bp = GetByType(bt, false, false, true, s, l, r, d, u, lu, ld, ru, rd),
                                    cp = GetByType(ct, false, false, true, s, l, r, d, u, lu, ld, ru, rd);
                                batch.Vertex((new Vec2(i, j) + IC(new Vec2(ac.Y, ac.X)) - new Vec2(a.Y, a.X) * ap) * tileSize);
                                batch.Vertex((new Vec2(i, j) + IC(new Vec2(bc.Y, bc.X)) - new Vec2(b.Y, b.X) * bp) * tileSize);
                                batch.Vertex((new Vec2(i, j) + IC(new Vec2(cc.Y, cc.X)) - new Vec2(c.Y, c.X) * cp) * tileSize);
                            }
                            has = true;
                            break;
                        }
                        if ((marching.u ? bl : !bl) && (marching.d ? br : !br) && (marching.r ? bd : !bd) && (marching.l ? bu : !bu) &&
                            (marching.rd ? brd : !brd) && (marching.ld ? bru : !bru) && (marching.ru ? bld : !bld) && (marching.lu ? blu : !blu))
                        {
                            foreach (var (a, b, c, at, bt, ct, ac, bc, cc) in marching.triangles)
                            {
                                float
                                    ap = GetByType(at, true, true, true, s, l, r, d, u, lu, ld, ru, rd),
                                    bp = GetByType(bt, true, true, true, s, l, r, d, u, lu, ld, ru, rd),
                                    cp = GetByType(ct, true, true, true, s, l, r, d, u, lu, ld, ru, rd);
                                batch.Vertex((new Vec2(i, j) + new Vec2(ac.Y, ac.X) + new Vec2(a.Y, a.X) * ap) * tileSize);
                                batch.Vertex((new Vec2(i, j) + new Vec2(bc.Y, bc.X) + new Vec2(b.Y, b.X) * bp) * tileSize);
                                batch.Vertex((new Vec2(i, j) + new Vec2(cc.Y, cc.X) + new Vec2(c.Y, c.X) * cp) * tileSize);
                            }
                            has = true;
                            break;
                        }
                    }
                    if (!has)
                    {
                        batch.Vertex(new Vec2(i, j) * tileSize);
                        batch.Vertex(new Vec2(i+1, j) * tileSize);
                        batch.Vertex(new Vec2(i, j+1) * tileSize);
                        batch.Vertex(new Vec2(i+1, j+1) * tileSize);
                        batch.Vertex(new Vec2(i + 1, j) * tileSize);
                        batch.Vertex(new Vec2(i, j + 1) * tileSize);
                    }
                }
            batch.End();
        }

        private float GetByType(byte type, bool ix, bool iy, bool rot, float s, float l, float r, float d, float u, float lu, float ld, float ru, float rd)
        {
            if (rot)
            {
                if (ix && iy) return MathF.Min(s, type switch
                {
                    0 => s,
                    1 => u,
                    2 => d,
                    3 => r,
                    4 => l,
                    5 => ru,
                    6 => lu,
                    7 => rd,
                    8 => ld,
                    9 => 1,
                });
                else if (ix) return MathF.Min(s, type switch
                {
                    0 => s,
                    1 => u,
                    2 => d,
                    3 => l,
                    4 => r,
                    5 => lu,
                    6 => ru,
                    7 => ld,
                    8 => rd,
                    9 => 1,
                });
                else if (iy) return MathF.Min(s, type switch
                {
                    0 => s,
                    1 => d,
                    2 => u,
                    3 => r,
                    4 => l,
                    5 => rd,
                    6 => ld,
                    7 => ru,
                    8 => lu,
                    9 => 1,
                });
                return MathF.Min(s, type switch
                {
                    0 => s,
                    1 => d,
                    2 => u,
                    3 => l,
                    4 => r,
                    5 => ld,
                    6 => rd,
                    7 => lu,
                    8 => ru,
                    9 => 1,
                });
            }
            if (ix && iy) return MathF.Min(s, type switch
            {
                0 => s,
                1 => r,
                2 => l,
                3 => u,
                4 => d,
                5 => ru,
                6 => rd,
                7 => lu,
                8 => ld,
                9 => 1,
            });
            else if (ix) return MathF.Min(s, type switch
            {
                0 => s,
                1 => r,
                2 => l,
                3 => d,
                4 => u,
                5 => rd,
                6 => ru,
                7 => ld,
                8 => lu,
                9 => 1,
            });
            else if (iy) return MathF.Min(s, type switch
            {
                0 => s,
                1 => l,
                2 => r,
                3 => u,
                4 => d,
                5 => lu,
                6 => ld,
                7 => ru,
                8 => rd,
                9 => 1,
            });
            return MathF.Min(s, type switch
            {
                0 => s,
                1 => l,
                2 => r,
                3 => d,
                4 => u,
                5 => ld,
                6 => lu,
                7 => rd,
                8 => ru,
                9 => 1,
            });
        }

        private Vec2 IC(Vec2 c) => new Vec2(1) - c;
        private Vec2 ICX(Vec2 c) => new Vec2(1-c.X, c.Y);
        private Vec2 ICY(Vec2 c) => new Vec2(c.X, 1-c.Y);

        private Vec2 IX(Vec2 c) => new Vec2(-c.X, c.Y);
        private Vec2 IY(Vec2 c) => new Vec2(c.X, -c.Y);

        /*
         * Types:
         * 0 - self population
         * 1 - left population
         * 2 - right population
         * 3 - down population
         * 4 - up population
         * 5 - down left population
         * 6 - up left population
         * 7 - down right population
         * 8 - up right population
         * 9 - ignore
         */
        readonly (bool lu, bool u, bool ru, bool l, bool r, bool ld, bool d, bool rd, (Vec2 a, Vec2 b, Vec2 c, byte at, byte bt, byte ct, Vec2 ac, Vec2 bc, Vec2 cc)[])[]
            march =
            new (bool, bool, bool, bool, bool, bool, bool, bool, (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[])[]
        {
            #region 1
            (
            false, false, false,
            false,        false,
            false, false, false,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(0, .5f), new(0, -.5f), new(-.5f, 0), 0, 0, 0, new(.5f, .5f), new(.5f, .5f), new(.5f, .5f)),
                (new(0, .5f), new(0, -.5f), new(.5f, 0), 0, 0, 0, new(.5f, .5f), new(.5f, .5f), new(.5f, .5f)),
            }),
            (
            true, false, false,
            false,        false,
            false, false, false,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(0, .5f), new(0, -.5f), new(-.5f, 0), 0, 0, 0, new(.5f, .5f), new(.5f, .5f), new(.5f, .5f)),
                (new(0, .5f), new(0, -.5f), new(.5f, 0), 0, 0, 0, new(.5f, .5f), new(.5f, .5f), new(.5f, .5f)),
            }),
            (
            true, false, true,
            false,        false,
            false, false, false,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(0, .5f), new(0, -.5f), new(-.5f, 0), 0, 0, 0, new(.5f, .5f), new(.5f, .5f), new(.5f, .5f)),
                (new(0, .5f), new(0, -.5f), new(.5f, 0), 0, 0, 0, new(.5f, .5f), new(.5f, .5f), new(.5f, .5f)),
            }),
            (
            true, false, true,
            false,        false,
            false, false, true,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(0, .5f), new(0, -.5f), new(-.5f, 0), 0, 0, 0, new(.5f, .5f), new(.5f, .5f), new(.5f, .5f)),
                (new(0, .5f), new(0, -.5f), new(.5f, 0), 0, 0, 0, new(.5f, .5f), new(.5f, .5f), new(.5f, .5f)),
            }),
            (
            true, false, true,
            false,        false,
            true, false, true,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(0, .5f), new(0, -.5f), new(-.5f, 0), 0, 0, 0, new(.5f, .5f), new(.5f, .5f), new(.5f, .5f)),
                (new(0, .5f), new(0, -.5f), new(.5f, 0), 0, 0, 0, new(.5f, .5f), new(.5f, .5f), new(.5f, .5f)),
            }),
            #endregion
            #region 2
            (
            false, false, false,
            false,        false,
            false, true,  false,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(-.5f, 0), new(.5f, 0), new(0, -1), 3, 3, 0, new(.5f, 1), new(.5f, 1), new(.5f, 1)),
            }),
            (
            false, false, true,
            false,        false,
            false, true,  false,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(-.5f, 0), new(.5f, 0), new(0, -1), 3, 3, 0, new(.5f, 1), new(.5f, 1), new(.5f, 1)),
            }),
            (
            true, false, true,
            false,        false,
            false, true,  false,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(-.5f, 0), new(.5f, 0), new(0, -1), 3, 3, 0, new(.5f, 1), new(.5f, 1), new(.5f, 1)),
            }),
            (
            false, false, false,
            false,        false,
            true, true,  true,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(-.5f, 0), new(.5f, 0), new(0, -1), 3, 3, 0, new(.5f, 1), new(.5f, 1), new(.5f, 1)),
            }),
            (
            false, false, false,
            false,        false,
            false, true,  true,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(-.5f, 0), new(.5f, 0), new(0, -1), 3, 3, 0, new(.5f, 1), new(.5f, 1), new(.5f, 1)),
            }),
            (
            false, false, true,
            false,        false,
            true, true,  true,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(-.5f, 0), new(.5f, 0), new(0, -1), 3, 3, 0, new(.5f, 1), new(.5f, 1), new(.5f, 1)),
            }),
            (
            false, false, true,
            false,        false,
            false, true,  true,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(-.5f, 0), new(.5f, 0), new(0, -1), 3, 3, 0, new(.5f, 1), new(.5f, 1), new(.5f, 1)),
            }),
            (
            true, false, false,
            false,        false,
            false, true,  true,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(-.5f, 0), new(.5f, 0), new(0, -1), 3, 3, 0, new(.5f, 1), new(.5f, 1), new(.5f, 1)),
            }),
            (
            true, false, true,
            false,        false,
            false, true,  true,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(-.5f, 0), new(.5f, 0), new(0, -1), 3, 3, 0, new(.5f, 1), new(.5f, 1), new(.5f, 1)),
            }),
            (
            true, false, true,
            false,        false,
            true, true,  true,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(-.5f, 0), new(.5f, 0), new(0, -1), 3, 3, 0, new(.5f, 1), new(.5f, 1), new(.5f, 1)),
            }),
            #endregion
            #region 3
            (
            false, true, true,
            false,        true,
            false, false, false,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(0, 0), new(0, 1), new(-1, 0), 9, 2, 4, new(1, 0), new(1, 0), new(1, 0)),
            }),
            (
            false, true, true,
            false,        true,
            true, false, false,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(0, 0), new(0, 1), new(-1, 0), 9, 2, 4, new(1, 0), new(1, 0), new(1, 0)),
            }),
            (
            true, true, true,
            false,        true,
            false, false, false,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(0, 0), new(0, 1), new(-1, 0), 9, 2, 4, new(1, 0), new(1, 0), new(1, 0)),
            }),
            (
            true, true, true,
            false,        true,
            true, false, false,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(0, 0), new(0, 1), new(-1, 0), 9, 2, 4, new(1, 0), new(1, 0), new(1, 0)),
            }),
            (
            true, true, true,
            false,        true,
            false, false, true,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(0, 0), new(0, 1), new(-1, 0), 9, 2, 4, new(1, 0), new(1, 0), new(1, 0)),
            }),
            (
            true, true, true,
            false,        true,
            true, false, true,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(0, 0), new(0, 1), new(-1, 0), 9, 2, 4, new(1, 0), new(1, 0), new(1, 0)),
            }),
            #endregion
            #region 4
            (
            false, false, false,
            false,        true,
            false, true, false,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(.5f, 0), new(-.5f, 0), new(.5f, .5f), 3, 3, 0, new(.5f, 1), new(.5f, 1), new(.5f, .5f)),
                (new(-.5f, 0), new(.5f, .5f), new(-.5f, -.5f), 3, 0, 0, new(.5f, 1), new(.5f, .5f), new(.5f, .5f)),
                (new(0, .5f), new(0, -.5f), new(.5f, .5f), 2, 2, 0, new(1, .5f), new(1, .5f), new(.5f, .5f)),
                (new(0, -.5f), new(.5f, .5f), new(-.5f, -.5f), 2, 0, 0, new(1, .5f), new(.5f, .5f), new(.5f, .5f)),
            }),
            (
            false, false, true,
            false,        true,
            false, true, false,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(.5f, 0), new(-.5f, 0), new(.5f, .5f), 3, 3, 0, new(.5f, 1), new(.5f, 1), new(.5f, .5f)),
                (new(-.5f, 0), new(.5f, .5f), new(-.5f, -.5f), 3, 0, 0, new(.5f, 1), new(.5f, .5f), new(.5f, .5f)),
                (new(0, .5f), new(0, -.5f), new(.5f, .5f), 2, 2, 0, new(1, .5f), new(1, .5f), new(.5f, .5f)),
                (new(0, -.5f), new(.5f, .5f), new(-.5f, -.5f), 2, 0, 0, new(1, .5f), new(.5f, .5f), new(.5f, .5f)),
            }),
            (
            true, false, false,
            false,        true,
            false, true, false,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(.5f, 0), new(-.5f, 0), new(.5f, .5f), 3, 3, 0, new(.5f, 1), new(.5f, 1), new(.5f, .5f)),
                (new(-.5f, 0), new(.5f, .5f), new(-.5f, -.5f), 3, 0, 0, new(.5f, 1), new(.5f, .5f), new(.5f, .5f)),
                (new(0, .5f), new(0, -.5f), new(.5f, .5f), 2, 2, 0, new(1, .5f), new(1, .5f), new(.5f, .5f)),
                (new(0, -.5f), new(.5f, .5f), new(-.5f, -.5f), 2, 0, 0, new(1, .5f), new(.5f, .5f), new(.5f, .5f)),
            }),
            (
            true, false, true,
            false,        true,
            false, true, false,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(.5f, 0), new(-.5f, 0), new(.5f, .5f), 3, 3, 0, new(.5f, 1), new(.5f, 1), new(.5f, .5f)),
                (new(-.5f, 0), new(.5f, .5f), new(-.5f, -.5f), 3, 0, 0, new(.5f, 1), new(.5f, .5f), new(.5f, .5f)),
                (new(0, .5f), new(0, -.5f), new(.5f, .5f), 2, 2, 0, new(1, .5f), new(1, .5f), new(.5f, .5f)),
                (new(0, -.5f), new(.5f, .5f), new(-.5f, -.5f), 2, 0, 0, new(1, .5f), new(.5f, .5f), new(.5f, .5f)),
            }),
            (
            false, false, true,
            false,        true,
            true, true, false,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(.5f, 0), new(-.5f, 0), new(.5f, .5f), 3, 3, 0, new(.5f, 1), new(.5f, 1), new(.5f, .5f)),
                (new(-.5f, 0), new(.5f, .5f), new(-.5f, -.5f), 3, 0, 0, new(.5f, 1), new(.5f, .5f), new(.5f, .5f)),
                (new(0, .5f), new(0, -.5f), new(.5f, .5f), 2, 2, 0, new(1, .5f), new(1, .5f), new(.5f, .5f)),
                (new(0, -.5f), new(.5f, .5f), new(-.5f, -.5f), 2, 0, 0, new(1, .5f), new(.5f, .5f), new(.5f, .5f)),
            }),
            (
            true, false, true,
            false,        true,
            true, true, false,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(.5f, 0), new(-.5f, 0), new(.5f, .5f), 3, 3, 0, new(.5f, 1), new(.5f, 1), new(.5f, .5f)),
                (new(-.5f, 0), new(.5f, .5f), new(-.5f, -.5f), 3, 0, 0, new(.5f, 1), new(.5f, .5f), new(.5f, .5f)),
                (new(0, .5f), new(0, -.5f), new(.5f, .5f), 2, 2, 0, new(1, .5f), new(1, .5f), new(.5f, .5f)),
                (new(0, -.5f), new(.5f, .5f), new(-.5f, -.5f), 2, 0, 0, new(1, .5f), new(.5f, .5f), new(.5f, .5f)),
            }),
            #endregion
            #region 5
            (
            false, true,  false,
            false,        false,
            false, true,  false,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(-.5f, 0), new(.5f, 0), new(.5f, 0), 3, 3, 0, new(.5f, 1), new(.5f, 1), new(.5f, .5f)),
                (new(-.5f, 0), new(-.5f, 0), new(.5f, 0), 3, 0, 0, new(.5f, 1), new(.5f, .5f), new(.5f, .5f)),
                (new(-.5f, 0), new(.5f, 0), new(.5f, 0), 4, 4, 0, new(.5f, 0), new(.5f, 0), new(.5f, .5f)),
                (new(-.5f, 0), new(-.5f, 0), new(.5f, 0), 4, 0, 0, new(.5f, 0), new(.5f, .5f), new(.5f, .5f)),
            }),
            (
            false, true,  true,
            false,        false,
            true, true,  false,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(-.5f, 0), new(.5f, 0), new(.5f, 0), 3, 3, 0, new(.5f, 1), new(.5f, 1), new(.5f, .5f)),
                (new(-.5f, 0), new(-.5f, 0), new(.5f, 0), 3, 0, 0, new(.5f, 1), new(.5f, .5f), new(.5f, .5f)),
                (new(-.5f, 0), new(.5f, 0), new(.5f, 0), 4, 4, 0, new(.5f, 0), new(.5f, 0), new(.5f, .5f)),
                (new(-.5f, 0), new(-.5f, 0), new(.5f, 0), 4, 0, 0, new(.5f, 0), new(.5f, .5f), new(.5f, .5f)),
            }),
            (
            false, true,  false,
            false,        false,
            false, true,  true,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(-.5f, 0), new(.5f, 0), new(.5f, 0), 3, 3, 0, new(.5f, 1), new(.5f, 1), new(.5f, .5f)),
                (new(-.5f, 0), new(-.5f, 0), new(.5f, 0), 3, 0, 0, new(.5f, 1), new(.5f, .5f), new(.5f, .5f)),
                (new(-.5f, 0), new(.5f, 0), new(.5f, 0), 4, 4, 0, new(.5f, 0), new(.5f, 0), new(.5f, .5f)),
                (new(-.5f, 0), new(-.5f, 0), new(.5f, 0), 4, 0, 0, new(.5f, 0), new(.5f, .5f), new(.5f, .5f)),
            }),
            (
            false, true,  true,
            false,        false,
            false, true,  true,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(-.5f, 0), new(.5f, 0), new(.5f, 0), 3, 3, 0, new(.5f, 1), new(.5f, 1), new(.5f, .5f)),
                (new(-.5f, 0), new(-.5f, 0), new(.5f, 0), 3, 0, 0, new(.5f, 1), new(.5f, .5f), new(.5f, .5f)),
                (new(-.5f, 0), new(.5f, 0), new(.5f, 0), 4, 4, 0, new(.5f, 0), new(.5f, 0), new(.5f, .5f)),
                (new(-.5f, 0), new(-.5f, 0), new(.5f, 0), 4, 0, 0, new(.5f, 0), new(.5f, .5f), new(.5f, .5f)),
            }),
            (
            false, true,  false,
            false,        false,
            true, true,  true,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(-.5f, 0), new(.5f, 0), new(.5f, 0), 3, 3, 0, new(.5f, 1), new(.5f, 1), new(.5f, .5f)),
                (new(-.5f, 0), new(-.5f, 0), new(.5f, 0), 3, 0, 0, new(.5f, 1), new(.5f, .5f), new(.5f, .5f)),
                (new(-.5f, 0), new(.5f, 0), new(.5f, 0), 4, 4, 0, new(.5f, 0), new(.5f, 0), new(.5f, .5f)),
                (new(-.5f, 0), new(-.5f, 0), new(.5f, 0), 4, 0, 0, new(.5f, 0), new(.5f, .5f), new(.5f, .5f)),
            }),
            (
            false, true,  true,
            false,        false,
            true, true,  true,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(-.5f, 0), new(.5f, 0), new(.5f, 0), 3, 3, 0, new(.5f, 1), new(.5f, 1), new(.5f, .5f)),
                (new(-.5f, 0), new(-.5f, 0), new(.5f, 0), 3, 0, 0, new(.5f, 1), new(.5f, .5f), new(.5f, .5f)),
                (new(-.5f, 0), new(.5f, 0), new(.5f, 0), 4, 4, 0, new(.5f, 0), new(.5f, 0), new(.5f, .5f)),
                (new(-.5f, 0), new(-.5f, 0), new(.5f, 0), 4, 0, 0, new(.5f, 0), new(.5f, .5f), new(.5f, .5f)),
            }),
            (
            true, true,  true,
            false,        false,
            true, true,  true,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(-.5f, 0), new(.5f, 0), new(.5f, 0), 3, 3, 0, new(.5f, 1), new(.5f, 1), new(.5f, .5f)),
                (new(-.5f, 0), new(-.5f, 0), new(.5f, 0), 3, 0, 0, new(.5f, 1), new(.5f, .5f), new(.5f, .5f)),
                (new(-.5f, 0), new(.5f, 0), new(.5f, 0), 4, 4, 0, new(.5f, 0), new(.5f, 0), new(.5f, .5f)),
                (new(-.5f, 0), new(-.5f, 0), new(.5f, 0), 4, 0, 0, new(.5f, 0), new(.5f, .5f), new(.5f, .5f)),
            }),
            #endregion
            #region 6
            (
            false, false, false,
            true,        true,
            true, true, true,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(0, 0), new(0, 0), new(0, -1), 9, 9, 1, new(0, 1), new(1, 1), new(0, 1)),
                (new(0, -1), new(0, 0), new(0, -1), 1, 9, 2, new(0, 1), new(1, 1), new(1, 1)),
            }),
            (
            false, false, true,
            true,        true,
            true, true, true,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(0, 0), new(0, 0), new(0, -1), 9, 9, 1, new(0, 1), new(1, 1), new(0, 1)),
                (new(0, -1), new(0, 0), new(0, -1), 1, 9, 2, new(0, 1), new(1, 1), new(1, 1)),
            }),
            (
            true, false, true,
            true,        true,
            true, true, true,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(0, 0), new(0, 0), new(0, -1), 9, 9, 1, new(0, 1), new(1, 1), new(0, 1)),
                (new(0, -1), new(0, 0), new(0, -1), 1, 9, 2, new(0, 1), new(1, 1), new(1, 1)),
            }),
            #endregion
            #region 7
            (
            false, false, false,
            true,        true,
            true, true, false,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(0, -1), new(0, .5f), new(1, 0), 1, 2, 3, new(0, 1), new(1, .5f), new(0, 1)),
                (new(0, -1), new(0, .5f), new(0, -.5f), 1, 2, 2, new(0, 1), new(1, .5f), new(1, .5f)),
                (new(0, -1), new(0, 0), new(1, 0), 1, 9, 3, new(0, 1), new(0, 1), new(0, 1)),
            }),
            (
            true, false, false,
            true,        true,
            true, true, false,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(0, -1), new(0, .5f), new(1, 0), 1, 2, 3, new(0, 1), new(1, .5f), new(0, 1)),
                (new(0, -1), new(0, .5f), new(0, -.5f), 1, 2, 2, new(0, 1), new(1, .5f), new(1, .5f)),
                (new(0, -1), new(0, 0), new(1, 0), 1, 9, 3, new(0, 1), new(0, 1), new(0, 1)),
            }),
            (
            false, false, true,
            true,        true,
            true, true, false,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(0, -1), new(0, .5f), new(1, 0), 1, 2, 3, new(0, 1), new(1, .5f), new(0, 1)),
                (new(0, -1), new(0, .5f), new(0, -.5f), 1, 2, 2, new(0, 1), new(1, .5f), new(1, .5f)),
                (new(0, -1), new(0, 0), new(1, 0), 1, 9, 3, new(0, 1), new(0, 1), new(0, 1)),
            }),
            (
            true, false, true,
            true,        true,
            true, true, false,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(0, -1), new(0, .5f), new(1, 0), 1, 2, 3, new(0, 1), new(1, .5f), new(0, 1)),
                (new(0, -1), new(0, .5f), new(0, -.5f), 1, 2, 2, new(0, 1), new(1, .5f), new(1, .5f)),
                (new(0, -1), new(0, 0), new(1, 0), 1, 9, 3, new(0, 1), new(0, 1), new(0, 1)),
            }),
            #endregion
            #region 8
            (
            true, true, true,
            true,        true,
            true, true, false,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(1, 0), new(0, 1), new(0, 0), 3, 2, 9, new(0, 1), new(1, 0), new(0, 0)),
                (new(1, 0), new(0, 0), new(0, 0), 3, 9, 9, new(0, 1), new(0, 1), new(0, 0)),
                (new(0, 1), new(0, 0), new(0, 0), 2, 9, 9, new(1, 0), new(1, 0), new(0, 0)),
            }),
            #endregion
            #region 9
            (
            true, true, false,
            true,        true,
            false, true, true,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(1, 0), new(0, -1), new(0, 1), 4, 2, 1, new(0, 0), new(1, 1), new(0, 0)),
                (new(-1, 0), new(0, 1), new(0, -1), 3, 1, 2, new(1, 1), new(0, 0), new(1, 1)),
                (new(1, 0), new(0, 0), new(0, 1), 4, 9, 1, new(0, 0), new(0, 0), new(0, 0)),
                (new(-1, 0), new(0, 0), new(0, -1), 3, 9, 2, new(1, 1), new(1, 1), new(1, 1)),
            }),
            #endregion
            #region 10
            (
            false, true, false,
            true,        true,
            false, false, false,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(0, .5f), new(0, -.5f), new(-.5f, 0), 1, 1, 4, new(0, .5f), new(0, .5f), new(.5f, 0)),
                (new(0, .5f), new(0, -.5f), new(.5f, 0), 2, 2, 4, new(1, .5f), new(1, .5f), new(.5f, 0)),
                (new(0, .5f), new(0, .5f), new(-.5f, 0), 2, 1, 4, new(1, .5f), new(0, .5f), new(.5f, 0)),
                (new(0, .5f), new(.5f, 0), new(-.5f, 0), 2, 4, 4, new(1, .5f), new(.5f, 0), new(.5f, 0)),
            }),
            (
            false, true, false,
            true,        true,
            false, false, true,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(0, .5f), new(0, -.5f), new(-.5f, 0), 1, 1, 4, new(0, .5f), new(0, .5f), new(.5f, 0)),
                (new(0, .5f), new(0, -.5f), new(.5f, 0), 2, 2, 4, new(1, .5f), new(1, .5f), new(.5f, 0)),
                (new(0, .5f), new(0, .5f), new(-.5f, 0), 2, 1, 4, new(1, .5f), new(0, .5f), new(.5f, 0)),
                (new(0, .5f), new(.5f, 0), new(-.5f, 0), 2, 4, 4, new(1, .5f), new(.5f, 0), new(.5f, 0)),
            }),
            (
            false, true, false,
            true,        true,
            true, false, true,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(0, .5f), new(0, -.5f), new(-.5f, 0), 1, 1, 4, new(0, .5f), new(0, .5f), new(.5f, 0)),
                (new(0, .5f), new(0, -.5f), new(.5f, 0), 2, 2, 4, new(1, .5f), new(1, .5f), new(.5f, 0)),
                (new(0, .5f), new(0, .5f), new(-.5f, 0), 2, 1, 4, new(1, .5f), new(0, .5f), new(.5f, 0)),
                (new(0, .5f), new(.5f, 0), new(-.5f, 0), 2, 4, 4, new(1, .5f), new(.5f, 0), new(.5f, 0)),
            }),
            #endregion
            (
            true, true, false,
            true,        true,
            false, true, false,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(0, 0), new(1, 0), new(0, 1), 9, 4, 1, new(0, 0), new(0, 0), new(0, 0)),
                (new(1, 0), new(0, 1), new(0, .5f), 4, 1, 2, new(0, 0), new(0, 0), new(1, .5f)),
                (new(0, 1), new(0, .5f), new(.5f, 0), 1, 2, 3, new(0, 0), new(1, .5f), new(.5f, 1)),

                (new(1, 0), new(0, -.5f), new(0, .5f), 4, 1, 2, new(0, 0), new(1, .5f), new(1, .5f)),
                (new(0, 1), new(-.5f, 0), new(.5f, 0), 1, 2, 3, new(0, 0), new(.5f, 1), new(.5f, 1)),
            }),
            (
            false, true, false,
            true,        true,
            true, true, true,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(0, 0), new(0, -1), new(-.5f, 0), 9, 1, 4, new(0, 1), new(0, 1), new(.5f, 0)),
                (new(0, 0), new(0, -1), new(.5f, 0), 9, 2, 4, new(1, 1), new(1, 1), new(.5f, 0)),
                (new(0, 0), new(0, 0), new(-.5f, 0), 9, 9, 4, new(1, 1), new(0, 1), new(.5f, 0)),
                (new(0, 0), new(.5f, 0), new(-.5f, 0), 9, 4, 4, new(1, 1), new(.5f, 0), new(.5f, 0)),
            }),
            (
            false, true, false,
            true,        true,
            false, true, false,
            new (Vec2, Vec2, Vec2, byte, byte, byte, Vec2, Vec2, Vec2)[]
            {
                (new(0, -.5f), new(-.5f, 0), new(.5f, 0), 1, 4, 4, new(0, .5f), new(.5f, 0), new(.5f, 0)),
                (new(0, -.5f), new(0, .5f), new(.5f, 0), 1, 1, 4, new(0, .5f), new(0, .5f), new(.5f, 0)),
                (new(0, .5f), new(-.5f, 0), new(.5f, 0), 1, 3, 4, new(0, .5f), new(.5f, 1), new(.5f, 0)),
                (new(.5f, 0), new(-.5f, 0), new(.5f, 0), 3, 3, 4, new(.5f, 1), new(.5f, 1), new(.5f, 0)),
                (new(.5f, 0), new(0, .5f), new(.5f, 0), 3, 2, 4, new(.5f, 1), new(1, .5f), new(.5f, 0)),
                (new(0, -.5f), new(0, .5f), new(.5f, 0), 3, 2, 4, new(1, .5f), new(1, .5f), new(.5f, 0)),
            }),
        };
    }
}