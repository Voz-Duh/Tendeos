using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Tendeos.Utils
{
    public readonly struct Noise
    {
        private readonly Type type;
        private readonly uint seed;
        private readonly Vec2 scale;
        private readonly List<(MaskType, Noise)> noises = new List<(MaskType, Noise)>();

        private Noise(Type type, uint seed, float scale)
        {
            this.type = type;
            this.seed = seed;
            this.scale = new Vec2(scale);
        }

        private Noise(Type type, uint seed, Vec2 scale)
        {
            this.type = type;
            this.seed = seed;
            this.scale = scale;
        }

        public float Get(float x, float y) => Get(0, x, y);

        public float Get(uint seed, float x, float y)
        {
            float power = type switch
            {
                Type.Perlin => Perlin.Noise2D(this.seed + seed, x * scale.X, y * scale.Y),
                Type.Simplex => Simplex.Noise2D(this.seed + seed, x * scale.X, y * scale.Y),
                Type.Voronoi => Voronoi.Noise(this.seed + seed, x * scale.X, y * scale.Y),
                Type.VoronoiFill => Voronoi.Fill(this.seed + seed, x * scale.X, y * scale.Y),
                Type.Constant => scale.X
            };
            foreach ((MaskType mask, Noise noise) in noises)
                switch (mask)
                {
                    case MaskType.Add:
                        power = Math.Max(power + noise.Get(seed, x, y), 0);
                        break;
                    case MaskType.Sub:
                        power = Math.Min(power - noise.Get(seed, x, y), 1);
                        break;
                    case MaskType.Mul:
                        power *= noise.Get(seed, x, y);
                        break;
                    case MaskType.Div:
                        power /= noise.Get(seed, x, y);
                        break;
                }

            return power;
        }

        private Noise Add(Noise noise)
        {
            noises.Add((MaskType.Add, noise));
            return this;
        }

        private Noise Sub(Noise noise)
        {
            noises.Add((MaskType.Sub, noise));
            return this;
        }

        private Noise Mul(Noise noise)
        {
            noises.Add((MaskType.Mul, noise));
            return this;
        }

        private Noise Div(Noise noise)
        {
            noises.Add((MaskType.Div, noise));
            return this;
        }

        public static Noise operator +(Noise a, Noise b) => a.Add(b);
        public static Noise operator -(Noise a, Noise b) => a.Sub(b);
        public static Noise operator /(Noise a, Noise b) => a.Div(b);
        public static Noise operator *(Noise a, Noise b) => a.Mul(b);

        public static Noise operator +(Noise a, float b) => a.Add(new Noise(Type.Constant, 0, b));
        public static Noise operator -(Noise a, float b) => a.Sub(new Noise(Type.Constant, 0, b));
        public static Noise operator /(Noise a, float b) => a.Div(new Noise(Type.Constant, 0, b));
        public static Noise operator *(Noise a, float b) => a.Mul(new Noise(Type.Constant, 0, b));

        public static Noise CPerlin(float scale, uint seed = 0) => new(Type.Perlin, seed, scale);
        public static Noise CSimplex(float scale, uint seed = 0) => new(Type.Simplex, seed, scale);
        public static Noise CVoronoi(float scale, uint seed = 0) => new(Type.Voronoi, seed, scale);
        public static Noise CVoronoiFill(float scale, uint seed = 0) => new(Type.VoronoiFill, seed, scale);

        public static Noise CPerlin(float scaleX, float scaleY, uint seed = 0) =>
            new(Type.Perlin, seed, new Vec2(scaleX, scaleY));

        public static Noise CSimplex(float scaleX, float scaleY, uint seed = 0) =>
            new(Type.Simplex, seed, new Vec2(scaleX, scaleY));

        public static Noise CVoronoi(float scaleX, float scaleY, uint seed = 0) =>
            new(Type.Voronoi, seed, new Vec2(scaleX, scaleY));

        public static Noise CVoronoiFill(float scaleX, float scaleY, uint seed = 0) =>
            new(Type.VoronoiFill, seed, new Vec2(scaleX, scaleY));

        public static implicit operator Noise(float a) => new(Type.Constant, 0, a);

        private enum Type
        {
            Perlin,
            Simplex,
            Voronoi,
            VoronoiFill,
            Constant
        }

        private enum MaskType
        {
            Add,
            Sub,
            Mul,
            Div
        };
    }

    public static class NoiseHelper
    {
        public static float Get(this Noise[] noises, uint seed, float x, float y)
        {
            float power = 0;
            foreach (Noise noise in noises)
                power += noise.Get(seed, x, y);
            return power / noises.Length;
        }
    }

    public static class Perlin
    {
        private static readonly byte[] p = {
              151, 160, 137, 91, 90, 105,
              131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 203,
              190, 126, 148, 247, 120, 234, 75, 0, 6, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 133,
              88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 15, 71, 134, 139, 48, 27, 166, 177,
              146, 158, 21, 83, 111, 229, 12, 60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 102, 143,
              54, 65, 25, 63, 161, 111, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200, 196, 135, 130, 116,
              188, 159, 86, 164, 100, 109, 198, 173, 186, 223, 64, 52, 217, 226, 250, 124, 123, 5, 202, 38, 147,
              118, 126, 255, 182, 185, 212, 207, 206, 59, 227, 147, 16, 58, 17, 182, 189, 28, 142, 223, 183, 170,
              213, 119, 248, 152, 222, 4, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 119, 129, 222, 39, 253,
              19, 98, 108, 110, 189, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228,
              251, 134, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 181, 151, 145, 25, 249, 14, 29, 107,
              49, 192, 214, 131, 181, 199, 106, 57, 184, 84, 204, 176, 115, 121, 150, 145, 127, 24, 150, 254,
              138, 236, 205, 93, 222, 114, 167, 229, 224, 172, 243, 141, 128, 195, 178, 166, 215, 161, 156, 180,
              151, 160, 137, 91, 90, 105, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142,
              8, 99, 37, 240, 21, 10, 203, 190, 126, 148, 247, 120, 234, 75, 0, 6, 197, 62, 94, 252, 219, 203,
              117, 35, 11, 32, 57, 177, 133, 88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 15,
              71, 134, 139, 48, 27, 166, 177, 146, 158, 21, 83, 111, 229, 12, 60, 211, 133, 230, 220, 105, 92, 41,
              55, 46, 245, 40, 244, 102, 143, 54, 65, 25, 63, 161, 111, 216, 80, 73, 209, 76, 132, 187, 208,
              89, 18, 169, 200, 196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 223, 64, 52, 217,
              226, 250, 124, 123, 5, 202, 38, 147, 118, 126, 255, 182, 185, 212, 207, 206, 59, 227, 147, 16,
              58, 17, 182, 189, 28, 142, 223, 183, 170, 213, 119, 248, 152, 222, 4, 154, 163, 170, 221, 153, 101,
              155, 167, 43, 172, 119, 129, 222, 39, 253, 19, 98, 108, 110, 189, 113, 224, 232, 178, 185,
              112, 104, 218, 246, 97, 228, 251, 134, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 181, 151,
              145, 25, 249, 14, 29, 107, 49, 192, 214, 131, 181, 199, 106, 57, 184, 84, 204, 176, 115, 121,
              150, 145, 127, 24, 150, 254, 138, 236, 205, 93, 222, 114, 167, 229, 224, 172, 243, 141, 128, 195,
              178, 166, 215, 161, 156, 180
        };

        private static float Fade(float t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        private static float Lerp(float t, float v0, float v1)
        {
            return v0 + t * (v1 - v0);
        }

        private static float Grad(uint hash, float x, float y, float z)
        {
            uint h = hash & 15;
            float u = h < 8 ? x : y;
            float v = h < 4 ? y : (h == 12 || h == 14 ? x : z);
            return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
        }

        public static float Noise2D(uint seed, float x, float y) =>
            Noise3D(seed, x, y, .5f);

        public static float Noise3D(uint seed, float x, float y, float z)
        {
            byte x1 = (byte)((int)x & 255),
                y1 = (byte)((int)y & 255),
                z1 = (byte)((int)z & 255);

            x -= (int)x;
            y -= (int)y;
            z -= (int)z;

            float x2 = Fade(x),
                y2 = Fade(y),
                z2 = Fade(z);

            int A = p[x1] + y1, AA = p[A] + z1, AB = p[A + 1] + z1,
                B = p[x1 + 1] + y1, BA = p[B] + z1, BB = p[B + 1] + z1;

            return Lerp(z2, 
                Lerp(y2, 
                    Lerp(x2, Grad(p[AA] + seed, x, y, z),
                        Grad(p[BA] + seed, x - 1, y, z)),
                    Lerp(x2, Grad(p[AB] + seed, x, y - 1, z),
                        Grad(p[BB] + seed, x - 1, y - 1, z))),
                Lerp(y2, 
                    Lerp(x2, Grad(p[AA + 1], x, y, z - 1),
                        Grad(p[BA + 1] + seed, x - 1, y, z - 1)),
                    Lerp(x2, Grad(p[AB + 1] + seed, x, y - 1, z - 1),
                        Grad(p[BB + 1] + seed, x - 1, y - 1, z - 1))));
        }
    }

    public static class Simplex
    {
        private class Grad
        {
            public sbyte X, Y, Z, W;

            public Grad(sbyte x, sbyte y, sbyte z)
            {
                X = x;
                Y = y;
                Z = z;
                W = 0;
            }

            public Grad(sbyte x, sbyte y, sbyte z, sbyte w)
            {
                X = x;
                Y = y;
                Z = z;
                W = w;
            }
        }

        private static readonly byte[] perm = { 151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99,
            37, 240, 21, 10, 23, 190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33, 88, 237, 149, 56, 87, 174, 20, 125,
            136, 171, 168, 68, 175, 74, 165, 71, 134, 139, 48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40,
            244, 102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200, 196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173,
            186, 3, 64, 52, 217, 226, 250, 124, 123, 5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42, 223, 183, 170,
            213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9, 129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104,
            218, 246, 97, 228, 251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214, 31, 181, 199, 106, 157,
            184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180, 151,
            160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23, 190, 6, 148, 247, 120, 234, 75,
            0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33, 88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139, 48,
            27, 166, 77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73,
            209, 76, 132, 187, 208, 89, 18, 169, 200, 196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123, 5, 202, 38,
            147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42, 223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153,
            101, 155, 167, 43, 172, 9, 129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228, 251, 34, 242, 193, 238, 210, 144,
            12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254,
            138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180 };
        private static readonly byte[] permMod12 = { 7, 4, 5, 7, 6, 3, 11, 1, 9, 11, 0, 5, 2, 5, 7, 9, 8, 0, 7, 6, 9, 10, 8, 3, 1, 0, 9, 10, 11, 10, 6, 4,
            7, 0, 6, 3, 0, 2, 5, 2, 10, 0, 3, 11, 9, 11, 11, 8, 9, 9, 9, 4, 9, 5, 8, 3, 6, 8, 5, 4, 3, 0, 8, 7, 2, 9, 11, 2, 7, 0, 3, 10, 5, 2, 2, 3, 11, 3, 1, 2, 0,
            7, 1, 2, 4, 9, 8, 5, 7, 10, 5, 4, 4, 6, 11, 6, 5, 1, 3, 5, 1, 0, 8, 1, 5, 4, 0, 7, 4, 5, 6, 1, 8, 4, 3, 10, 8, 8, 3, 2, 8, 4, 1, 6, 5, 6, 3, 4, 4, 1, 10,
            10, 4, 3, 5, 10, 2, 3, 10, 6, 3, 10, 1, 8, 3, 2, 11, 11, 11, 4, 10, 5, 2, 9, 4, 6, 7, 3, 2, 9, 11, 8, 8, 2, 8, 10, 7, 10, 5, 9, 5, 11, 11, 7, 4, 9, 9, 10,
            3, 1, 7, 2, 0, 2, 7, 5, 8, 4, 10, 5, 4, 8, 2, 6, 1, 0, 11, 10, 2, 1, 10, 6, 0, 0, 11, 11, 6, 1, 9, 3, 1, 7, 9, 2, 11, 11, 1, 0, 10, 7, 1, 7, 10, 1, 4, 0,
            0, 8, 7, 1, 2, 9, 7, 4, 6, 2, 6, 8, 1, 9, 6, 6, 7, 5, 0, 0, 3, 9, 8, 3, 6, 6, 11, 1, 0, 0, 7, 4, 5, 7, 6, 3, 11, 1, 9, 11, 0, 5, 2, 5, 7, 9, 8, 0, 7, 6,
            9, 10, 8, 3, 1, 0, 9, 10, 11, 10, 6, 4, 7, 0, 6, 3, 0, 2, 5, 2, 10, 0, 3, 11, 9, 11, 11, 8, 9, 9, 9, 4, 9, 5, 8, 3, 6, 8, 5, 4, 3, 0, 8, 7, 2, 9, 11, 2,
            7, 0, 3, 10, 5, 2, 2, 3, 11, 3, 1, 2, 0, 7, 1, 2, 4, 9, 8, 5, 7, 10, 5, 4, 4, 6, 11, 6, 5, 1, 3, 5, 1, 0, 8, 1, 5, 4, 0, 7, 4, 5, 6, 1, 8, 4, 3, 10, 8, 8,
            3, 2, 8, 4, 1, 6, 5, 6, 3, 4, 4, 1, 10, 10, 4, 3, 5, 10, 2, 3, 10, 6, 3, 10, 1, 8, 3, 2, 11, 11, 11, 4, 10, 5, 2, 9, 4, 6, 7, 3, 2, 9, 11, 8, 8, 2, 8, 10,
            7, 10, 5, 9, 5, 11, 11, 7, 4, 9, 9, 10, 3, 1, 7, 2, 0, 2, 7, 5, 8, 4, 10, 5, 4, 8, 2, 6, 1, 0, 11, 10, 2, 1, 10, 6, 0, 0, 11, 11, 6, 1, 9, 3, 1, 7, 9, 2,
            11, 11, 1, 0, 10, 7, 1, 7, 10, 1, 4, 0, 0, 8, 7, 1, 2, 9, 7, 4, 6, 2, 6, 8, 1, 9, 6, 6, 7, 5, 0, 0, 3, 9, 8, 3, 6, 6, 11, 1, 0, 0 };

        private static readonly Grad[] grad3 = {
            new(1, 1, 0), new(-1, 1, 0), new(1, -1, 0), new(-1, -1, 0),
            new(1, 0, 1), new(-1, 0, 1), new(1, 0, -1), new(-1, 0, -1),
            new(0, 1, 1), new(0, -1, 1), new(0, 1, -1), new(0, -1, -1)
        };

        private static readonly Grad[] grad4 = {
            new(0, 1, 1, 1),  new(0, 1, 1, -1),  new(0, 1, -1, 1),  new(0, 1, -1, -1),
            new(0, -1, 1, 1), new(0, -1, 1, -1), new(0, -1, -1, 1), new(0, -1, -1, -1),
            new(1, 0, 1, 1),  new(1, 0, 1, -1),  new(1, 0, -1, 1),  new(1, 0, -1, -1),
            new(-1, 0, 1, 1), new(-1, 0, 1, -1), new(-1, 0, -1, 1), new(-1, 0, -1, -1),
            new(1, 1, 0, 1),  new(1, 1, 0, -1),  new(1, -1, 0, 1),  new(1, -1, 0, -1),
            new(-1, 1, 0, 1), new(-1, 1, 0, -1), new(-1, -1, 0, 1), new(-1, -1, 0, -1),
            new(1, 1, 1, 0),  new(1, 1, -1, 0),  new(1, -1, 1, 0),  new(1, -1, -1, 0),
            new(-1, 1, 1, 0), new(-1, 1, -1, 0), new(-1, -1, 1, 0), new(-1, -1, -1, 0)
        };
        
        private const float F2 = 0.366025403784439f;
        private const float G2 = 0.211324865405187f;
        private const float G22 = 0.4226497f;
        private const float F3 = 0.333333333333333f;
        private const float G3 = 0.166666666666667f;
        private const float G32 = 0.3333333f;
        private const float G33 = 0.5f;
        private const float F4 = 1.98606797749979f;
        private const float G4 = 0.138196601125011f;
        private const float G42 = 0.2763932f;
        private const float G43 = 0.4145898f;
        private const float G44 = 0.5527864f;
        
        
        private static float Dot(Grad g, float x, float y) => g.X * x + g.Y * y;

        private static float Dot(Grad g, float x, float y, float z) => g.X * x + g.Y * y + g.Z * z;

        private static float Dot(Grad g, float x, float y, float z, float w) => g.X * x + g.Y * y + g.Z * z + g.W * w;
        
        public static float Noise2D(uint seed, float x, float y)
        {
            float n0, n1, n2;

            n0 = (x + y) * F2;
            int i = (int)(x + n0);
            int j = (int)(y + n0);
            float t = (i + j) * G2;
            float x0 = x - (i - t);
            float y0 = y - (j - t);
            int i1, j1;
            if (x0 > y0)
            {
                i1 = 1;
                j1 = 0;
            }
            else
            {
                i1 = 0;
                j1 = 1;
            }
            float x1 = x0 - i1 + G2;
            float y1 = y0 - j1 + G2;
            float x2 = x0 - 1.0f + G22;
            float y2 = y0 - 1.0f + G22;
            i = (int)((i + seed) & 255);
            j = (int)((j + seed) & 255);
            
            byte gi0 = (byte)(permMod12[i + perm[j]] % 12);
            byte gi1 = (byte)(permMod12[i + i1 + perm[j + j1]] % 12);
            byte gi2 = (byte)(permMod12[i + 1 + perm[j + 1]] % 12);

            t = 0.5f - x0 * x0 - y0 * y0;
            if (t < 0) n0 = 0.0f;
            else
            {
                t *= t;
                n0 = t * t * Dot(grad3[gi0], x0, y0);
            }
            t = 0.5f - x1 * x1 - y1 * y1;
            if (t < 0) n1 = 0.0f;
            else
            {
                t *= t;
                n1 = t * t * Dot(grad3[gi1], x1, y1);
            }
            t = 0.5f - x2 * x2 - y2 * y2;
            if (t < 0) n2 = 0.0f;
            else
            {
                t *= t;
                n2 = t * t * Dot(grad3[gi2], x2, y2);
            }
            return (70.0f * (n0 + n1 + n2) + 1) / 2;
        }
        
        public static float Noise3D(uint seed, float x, float y, float z)
        {
            float n0, n1, n2, n3;

            n0 = (x + y + z) * F3;
            int i = (int)(x + n0);
            int j = (int)(y + n0);
            int k = (int)(z + n0);
            float t = (i + j + k) * G3;
            float x0 = x - (i - t);
            float y0 = y - (j - t);
            float z0 = z - (k - t);
            int i1, j1, k1;
            int i2, j2, k2;
            if (x0 >= y0)
            {
                if (y0 >= z0)
                {
                    i1 = 1;
                    j1 = 0;
                    k1 = 0;
                    i2 = 1;
                    j2 = 1;
                    k2 = 0;
                } // X Y Z order
                else if (x0 >= z0)
                {
                    i1 = 1;
                    j1 = 0;
                    k1 = 0;
                    i2 = 1;
                    j2 = 0;
                    k2 = 1;
                } // X Z Y order
                else
                {
                    i1 = 0;
                    j1 = 0;
                    k1 = 1;
                    i2 = 1;
                    j2 = 0;
                    k2 = 1;
                } // Z X Y order
            }
            else
            {
                if (y0 < z0)
                {
                    i1 = 0;
                    j1 = 0;
                    k1 = 1;
                    i2 = 0;
                    j2 = 1;
                    k2 = 1;
                } // Z Y X order
                else if (x0 < z0)
                {
                    i1 = 0;
                    j1 = 1;
                    k1 = 0;
                    i2 = 0;
                    j2 = 1;
                    k2 = 1;
                } // Y Z X order
                else
                {
                    i1 = 0;
                    j1 = 1;
                    k1 = 0;
                    i2 = 1;
                    j2 = 1;
                    k2 = 0;
                } // Y X Z order
            }
            float x1 = x0 - i1 + G3;
            float y1 = y0 - j1 + G3;
            float z1 = z0 - k1 + G3;
            float x2 = x0 - i2 + G32;
            float y2 = y0 - j2 + G32;
            float z2 = z0 - k2 + G32;
            float x3 = x0 - 1 + G33;
            float y3 = y0 - 1 + G33;
            float z3 = z0 - 1 + G33;
            i = (int)((i + seed) & 255);
            j = (int)((j + seed) & 255);
            k = (int)((k + seed) & 255);
            int gi0 = permMod12[i + perm[j + perm[k]]];
            int gi1 = permMod12[i + i1 + perm[j + j1 + perm[k + k1]]];
            int gi2 = permMod12[i + i2 + perm[j + j2 + perm[k + k2]]];
            int gi3 = permMod12[i + 1 + perm[j + 1 + perm[k + 1]]];

            t = 0.6f - x0 * x0 - y0 * y0 - z0 * z0;
            if (t < 0) n0 = 0;
            else
            {
                t *= t;
                n0 = t * t * Dot(grad3[gi0], x0, y0, z0);
            }
            t = 0.6f - x1 * x1 - y1 * y1 - z1 * z1;
            if (t < 0) n1 = 0;
            else
            {
                t *= t;
                n1 = t * t * Dot(grad3[gi1], x1, y1, z1); // grad3 должен быть определен
            }
            t = 0.6f - x2 * x2 - y2 * y2 - z2 * z2;
            if (t < 0) n2 = 0;
            else
            {
                t *= t;
                n2 = t * t * Dot(grad3[gi2], x2, y2, z2); // grad3 должен быть определен
            }
            t = 0.6f - x3 * x3 - y3 * y3 - z3 * z3;
            if (t < 0) n3 = 0;
            else
            {
                t *= t;
                n3 = t * t * Dot(grad3[gi3], x3, y3, z3); // grad3 должен быть определен
            }
            return (32 * (n0 + n1 + n2 + n3) + 1) / 2;
        }

        public static float Noise4D(uint seed, float x, float y, float z, float w)
        {
            float n0, n1, n2, n3, n4;
            n0 = (x + y + z + w) * F4;
            int i = (int)(x + n0);
            int j = (int)(y + n0);
            int k = (int)(z + n0);
            int l = (int)(w + n0);
            float t = (i + j + k + l) * G4;
            float x0 = x - (i - t);
            float y0 = y - (j - t);
            float z0 = z - (k - t);
            float w0 = w - (l - t);
            int rankx = 0;
            int ranky = 0;
            int rankz = 0;
            int rankw = 0;
            if (x0 > y0) rankx++;
            else ranky++;
            if (x0 > z0) rankx++;
            else rankz++;
            if (x0 > w0) rankx++;
            else rankw++;
            if (y0 > z0) ranky++;
            else rankz++;
            if (y0 > w0) ranky++;
            else rankw++;
            if (z0 > w0) rankz++;
            else rankw++;
            int i1, j1, k1, l1;
            int i2, j2, k2, l2;
            int i3, j3, k3, l3;
            i1 = rankx >= 3 ? 1 : 0;
            j1 = ranky >= 3 ? 1 : 0;
            k1 = rankz >= 3 ? 1 : 0;
            l1 = rankw >= 3 ? 1 : 0;
            i2 = rankx >= 2 ? 1 : 0;
            j2 = ranky >= 2 ? 1 : 0;
            k2 = rankz >= 2 ? 1 : 0;
            l2 = rankw >= 2 ? 1 : 0;
            i3 = rankx >= 1 ? 1 : 0;
            j3 = ranky >= 1 ? 1 : 0;
            k3 = rankz >= 1 ? 1 : 0;
            l3 = rankw >= 1 ? 1 : 0;
            float x1 = x0 - i1 + G4;
            float y1 = y0 - j1 + G4;
            float z1 = z0 - k1 + G4;
            float w1 = w0 - l1 + G4;
            float x2 = x0 - i2 + G42;
            float y2 = y0 - j2 + G42;
            float z2 = z0 - k2 + G42;
            float w2 = w0 - l2 + G42;
            float x3 = x0 - i3 + G43;
            float y3 = y0 - j3 + G43;
            float z3 = z0 - k3 + G43;
            float w3 = w0 - l3 + G43;
            float x4 = x0 - 1 + G44;
            float y4 = y0 - 1 + G44;
            float z4 = z0 - 1 + G44;
            float w4 = w0 - 1 + G44;
            i = (int)((i + seed) & 255);
            j = (int)((j + seed) & 255);
            k = (int)((k + seed) & 255);
            l = (int)((l + seed) & 255);
            int gi0 = perm[i + perm[j + perm[k + perm[l]]]] % 32;
            int gi1 = perm[i + i1 + perm[j + j1 + perm[k + k1 + perm[l + l1]]]] % 32;
            int gi2 = perm[i + i2 + perm[j + j2 + perm[k + k2 + perm[l + l2]]]] % 32;
            int gi3 = perm[i + i3 + perm[j + j3 + perm[k + k3 + perm[l + l3]]]] % 32;
            int gi4 = perm[i + 1 + perm[j + 1 + perm[k + 1 + perm[l + 1]]]] % 32;
            t = 0.6f - x0 * x0 - y0 * y0 - z0 * z0 - w0 * w0;
            if (t < 0) n0 = 0;
            else
            {
                t *= t;
                n0 = t * t * Dot(grad4[gi0], x0, y0, z0, w0);
            }
            t = 0.6f - x1 * x1 - y1 * y1 - z1 * z1 - w1 * w1;
            if (t < 0) n1 = 0;
            else
            {
                t *= t;
                n1 = t * t * Dot(grad4[gi1], x1, y1, z1, w1);
            }
            t = 0.6f - x2 * x2 - y2 * y2 - z2 * z2 - w2 * w2;
            if (t < 0) n2 = 0;
            else
            {
                t *= t;
                n2 = t * t * Dot(grad4[gi2], x2, y2, z2, w2);
            }
            t = 0.6f - x3 * x3 - y3 * y3 - z3 * z3 - w3 * w3;
            if (t < 0) n3 = 0;
            else
            {
                t *= t;
                n3 = t * t * Dot(grad4[gi3], x3, y3, z3, w3);
            }
            t = 0.6f - x4 * x4 - y4 * y4 - z4 * z4 - w4 * w4;
            if (t < 0) n4 = 0;
            else
            {
                t *= t;
                n4 = t * t * Dot(grad4[gi4], x4, y4, z4, w4);
            }
            return (27 * (n0 + n1 + n2 + n3 + n4) + 1) / 2;
        }
    }

    public static class Voronoi
    {
        private static float Frac(float number)
        {
            return number - (int)number;
        }

        private static float Dot2(float ax, float ay, float bx, float by)
        {
            return ax * bx + ay * by;
        }
        
        private static float RandomFromSeed(uint seed, float x, float y)
        {
            uint hash = seed ^ (uint)(x * 1000) ^ (uint)(y * 1000);
            hash = (hash ^ (hash >> 16)) * 0x45d9f3b;
            hash = (hash ^ (hash >> 16)) * 0x45d9f3b;
            hash = hash ^ (hash >> 16);
            return Frac(hash / (float)uint.MaxValue);
        }
    
        private static (float, float) Random2(uint seed, float x, float y) => (
            RandomFromSeed(seed, x, y),
            RandomFromSeed(seed, y, x)
        );
        
        public static float Noise(uint seed, float x, float y)
        {
            x *= 3;
            y *= 3;
            float iStx = (int)x;
            float iSty = (int)y;
            float fStx = Frac(x);
            float fSty = Frac(y);
            float mDist = 1;
            int i, j;
            for (j = -1; j <= 1; j++)
            {
                for (i = -1; i <= 1; i++)
                {
                    var (px, py) = Random2(seed, iStx + i, iSty + j);

                    px = 0.5f + (0.5f + MathF.Sin(6.2831f * px));
                    py = 0.5f + (0.5f + MathF.Sin(6.2831f * py));

                    px = i + px - fStx;
                    py = j + py - fSty;

                    px = px * px + py * py;
                    mDist = MathF.Min(mDist, px);
                }
            }
            return (1.0f - mDist + 1) / 2;
        }

        public static float Fill(uint seed, float x, float y)
        {
            x *= 3;
            y *= 3;
            float iStx = (int)x;
            float iSty = (int)y;
            float fStx = Frac(x);
            float fSty = Frac(y);
            float mDist = 1;
            float mx = 0, my = 0;
            int i, j;
            for (j = -1; j <= 1; j++)
            {
                for (i = -1; i <= 1; i++)
                {
                    var (px, py) = Random2(seed, iStx + i, iSty + j);

                    px = 0.5f + (0.5f + MathF.Sin(6.2831f * px));
                    py = 0.5f + (0.5f + MathF.Sin(6.2831f * py));

                    px = i + px - fStx;
                    py = j + py - fSty;

                    px = px * px + py * py;
                    if (px < mDist)
                    {
                        mDist = px;
                        mx = px;
                        my = py;
                    }
                }
            }
            return (Dot2(mx, my, 0.3f, 0.6f) + 1) / 2;
        }
    }
}