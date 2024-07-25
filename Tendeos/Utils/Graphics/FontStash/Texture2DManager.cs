using System;
using System.Drawing;
using FontStashSharp.Interfaces;

namespace Tendeos.Utils.Graphics.FontStash
{
    public class Texture2DManager : ITexture2DManager
    {
        readonly Assets.Atlas atlas;

        public Texture2DManager(Assets.Atlas atlas)
        {
            this.atlas = atlas ?? throw new ArgumentNullException(nameof(atlas));
        }

        public object CreateTexture(int width, int height)
        {
            return atlas.Allocate(width, height, 1);
        }

        public Point GetTextureSize(object texture)
        {
            var allocated = (Microsoft.Xna.Framework.Rectangle) texture;
            return new Point(allocated.Width, allocated.Height);
        }

        public void SetTextureData(object allocated, Rectangle bounds, byte[] data)
        {
            atlas.DrawInAllocated(
                (Microsoft.Xna.Framework.Rectangle) allocated,
                new Microsoft.Xna.Framework.Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height),
                data);
        }
    }
}