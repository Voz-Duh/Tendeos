using System;

namespace XnaGame.Utils
{
    public static class Noise
    {
        private static float Interpolate(float a0, float a1, float w)
        {
            return (a1 - a0) * w + a0;
        }

        private static Vec2 RandomGradient(uint seed, int ix, int iy)
        {
            uint w = 8 * sizeof(uint);
            uint s = w / 2;
            uint a = (uint)ix, b = (uint)iy;
            a *= 3284157443; a += seed; b ^= a << (int)s | a >> (int)w - (int)s;
            b *= 1911520717; b += seed; a ^= b << (int)s | b >> (int)w - (int)s;
            a *= 2048419325;
            float random = a * (3.14159265f / ~(~0u >> 1));
            Vec2 v;
            v.X = MathF.Cos(random); v.Y = MathF.Sin(random);
            return v;
        }

        private static float DotGridGradient(uint seed, int ix, int iy, float x, float y)
        {
            Vec2 gradient = RandomGradient(seed, ix, iy);

            float dx = x - ix;
            float dy = y - iy;

            return (dx * gradient.X + dy * gradient.Y);
        }

        public static float Perlin(uint seed, float x, float y)
        {
            int x0 = (int)MathF.Floor(x);
            int x1 = x0 + 1;
            int y0 = (int)MathF.Floor(y);
            int y1 = y0 + 1;

            float sx = x - x0;
            float sy = y - y0;

            float n0, n1, ix0, ix1, value;

            n0 = DotGridGradient(seed, x0, y0, x, y);
            n1 = DotGridGradient(seed, x1, y0, x, y);
            ix0 = Interpolate(n0, n1, sx);

            n0 = DotGridGradient(seed, x0, y1, x, y);
            n1 = DotGridGradient(seed, x1, y1, x, y);
            ix1 = Interpolate(n0, n1, sx);

            value = Interpolate(ix0, ix1, sy);
            return 0.5f + value / 2;
        }
    }
}