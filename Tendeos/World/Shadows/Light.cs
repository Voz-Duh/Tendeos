using Microsoft.Xna.Framework;
using System;
using System.Runtime.CompilerServices;

namespace Tendeos.World.Shadows
{
    public struct Light
    {
        public bool available;
        public float x, y;
        public Vector3 color;
        public float intensity, radius;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Rectangle Generate(IMap map)
        {
            int s = (int) MathF.Ceiling(radius);
            int fs = (int) MathF.Ceiling(radius * 2);
            return new Rectangle((int) (x / map.TileSize) - s - 1, (int) (y / map.TileSize) - s - 1, fs + 2, fs + 2);
        }
    }
}