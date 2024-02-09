using Microsoft.Xna.Framework;
using System;

namespace XnaGame.World.Shadows
{
    public struct Light
    {
        public bool available;
        public float x, y;
        public Vector3 color;
        public float intensity, radius;
        public Rectangle rectangle;
        public void Generate()
        {
            int s = (int)MathF.Ceiling(radius);
            int fs = (int)MathF.Ceiling(radius*2);
            rectangle = new Rectangle((int)(x/Map.tileSize)-s-1, (int)(y/Map.tileSize)-s-1, fs+2, fs+2);
        }
    }
}