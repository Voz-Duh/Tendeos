using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
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
                Type.Perlin => Perlin(this.seed + seed, x * scale.X, y * scale.Y),
                Type.Simplex => Simplex(this.seed + seed, x * scale.X, y * scale.Y),
                Type.Voronoi => Voronoi(this.seed + seed, x * scale.X, y * scale.Y),
                Type.VoronoiFill => VoronoiFill(this.seed + seed, x * scale.X, y * scale.Y),
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

        public static Noise CPerlin(float scale, uint seed = 0) => new Noise(Type.Perlin, seed, scale);
        public static Noise CSimplex(float scale, uint seed = 0) => new Noise(Type.Simplex, seed, scale);
        public static Noise CVoronoi(float scale, uint seed = 0) => new Noise(Type.Voronoi, seed, scale);
        public static Noise CVoronoiFill(float scale, uint seed = 0) => new Noise(Type.VoronoiFill, seed, scale);

        public static Noise CPerlin(float scaleX, float scaleY, uint seed = 0) => new Noise(Type.Perlin, seed, new Vec2(scaleX, scaleY));
        public static Noise CSimplex(float scaleX, float scaleY, uint seed = 0) => new Noise(Type.Simplex, seed, new Vec2(scaleX, scaleY));
        public static Noise CVoronoi(float scaleX, float scaleY, uint seed = 0) => new Noise(Type.Voronoi, seed, new Vec2(scaleX, scaleY));
        public static Noise CVoronoiFill(float scaleX, float scaleY, uint seed = 0) => new Noise(Type.VoronoiFill, seed, new Vec2(scaleX, scaleY));

        #region PERLIN
        public static float Perlin(uint seed, float x) => Perlin(seed, x, 0, 0);

        public static float Perlin(uint seed, float x, float y) => Perlin(seed, x, y, 0);

        public static float Perlin(uint seed, float x, float y, float z) => (DLLPerlin(seed, x, y, z) + 1) / 2;

        [DllImport("Noise.dll", EntryPoint = "Perlin")]
        private static extern float DLLPerlin(uint seed, float x, float y, float z);
        #endregion

        #region SIMPLEX
        public static float Simplex(uint seed, float x) => (Simplex2D(seed, x, 0) + 1) / 2;

        public static float Simplex(uint seed, float x, float y) => (Simplex2D(seed, x, y) + 1) / 2;

        public static float Simplex(uint seed, float x, float y, float z) => (Simplex3D(seed, x, y, z) + 1) / 2;

        public static float Simplex(uint seed, float x, float y, float z, float w) => (Simplex4D(seed, x, y, z, w) + 1) / 2;

        [DllImport("Noise.dll")]
        private static extern float Simplex2D(uint seed, float x, float y);

        [DllImport("Noise.dll")]
        private static extern float Simplex3D(uint seed, float x, float y, float z);

        [DllImport("Noise.dll")]
        private static extern float Simplex4D(uint seed, float x, float y, float z, float w);
        #endregion

        #region VORONOI
        [DllImport("Noise.dll", EntryPoint = "Voronoi")]
        private static extern float DLLVoronoi(uint seed, float x, float y);
        public static float Voronoi(uint seed, float x, float y) => (DLLVoronoi(seed, x, y) + 1) / 2;
        public static float Voronoi(uint seed, float x) => Voronoi(seed, x, 0f);

        [DllImport("Noise.dll", EntryPoint = "VoronoiFill")]
        private static extern float DLLVoronoiFill(uint seed, float x, float y);
        public static float VoronoiFill(uint seed, float x, float y) => (DLLVoronoiFill(seed, x, y) + 1) / 2;
        public static float VoronoiFill(uint seed, float x) => VoronoiFill(seed, x, 0f);
        #endregion

        private enum Type { Perlin, Simplex, Voronoi, VoronoiFill, Constant }
        private enum MaskType { Add, Sub, Mul, Div };
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
}