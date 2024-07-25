using System;
using FontStashSharp;
using FontStashSharp.Interfaces;
using Microsoft.Xna.Framework;

namespace Tendeos.Utils.Graphics.FontStash
{
    public class Renderer : IFontStashRenderer
    {
        SpriteBatch batch;
        Texture2DManager textureManager;

        public ITexture2DManager TextureManager => textureManager;

        public Renderer(SpriteBatch batch, Assets.Atlas atlas)
        {
            this.batch = batch ?? throw new ArgumentNullException(nameof(batch));
            textureManager = new Texture2DManager(atlas);
        }

        public void Draw(object texture, System.Numerics.Vector2 pos, System.Drawing.Rectangle? src, FSColor color,
            float rotation, System.Numerics.Vector2 scale, float depth)
        {
            var textureWrapper = (Rectangle) texture;

            if (src == null)
            {
                batch.Rect(new Color(color.R, color.G, color.B, color.A), textureWrapper, pos, scale, rotation, 0, 0);
            }
            else
            {
                System.Drawing.Rectangle rect = src ?? System.Drawing.Rectangle.Empty;
                batch.Rect(new Color(color.R, color.G, color.B, color.A), textureWrapper,
                    new Rectangle(rect.X, rect.Y, rect.Width, rect.Height), pos, scale, rotation, 0, 0);
            }
        }
    }
}