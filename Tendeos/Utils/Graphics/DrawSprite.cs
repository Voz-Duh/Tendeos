using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Tendeos.Utils.Graphics
{
    public static class DrawSprite
    {
        public static SpriteEffects SpriteEffects { get; set; }

        public static void Rect(this SpriteBatch spriteBatch, Sprite texture, FRectangle position, float rotation = 0, float depth = 0) =>
            Rect(spriteBatch, texture, Color.White, position, rotation, depth);

        public static void Rect(this SpriteBatch spriteBatch, Sprite texture, Color color, FRectangle position, float rotation = 0, float depth = 0) =>
            spriteBatch.Draw(
                texture,
                position.Location,
                texture.Rect,
                color,
                MathHelper.ToRadians(rotation),
                Vec2.Zero,
                position.Size / new Vec2(texture.Rect.Width, texture.Rect.Height),
                SpriteEffects,
                depth
            );

        public static void Rect(this SpriteBatch spriteBatch, Sprite texture, Vec2 position, float rotation = 0, float scale = 1, float depth = 0, Origin xOrigin = Origin.Center, Origin yOrigin = Origin.Center) =>
            Rect(spriteBatch, texture, Color.White, position, rotation, scale, depth, xOrigin, yOrigin);

        public static void Rect(this SpriteBatch spriteBatch, Sprite texture, Color color, Vec2 position, float rotation = 0, float scale = 1, float depth = 0, Origin xOrigin = Origin.Center, Origin yOrigin = Origin.Center) =>
            spriteBatch.Draw(
                texture,
                position,
                texture.Rect,
                color,
                MathHelper.ToRadians(rotation),
                new Vec2(texture.Rect.Width * GetOrigin(xOrigin), texture.Rect.Height * GetOrigin(yOrigin)),
                scale,
                SpriteEffects,
                depth
            );

        public static void Rect(this SpriteBatch spriteBatch, Sprite texture, Vec2 position, Vec2? scale, float rotation = 0, float depth = 0, Origin xOrigin = Origin.Center, Origin yOrigin = Origin.Center) =>
            Rect(spriteBatch, texture, Color.White, position, scale, rotation, depth, xOrigin, yOrigin);

        public static void Rect(this SpriteBatch spriteBatch, Sprite texture, Color color, Vec2 position, Vec2? scale, float rotation = 0, float depth = 0, Origin xOrigin = Origin.Center, Origin yOrigin = Origin.Center) =>
            spriteBatch.Draw(
                texture,
                position,
                texture.Rect,
                color,
                MathHelper.ToRadians(rotation),
                new Vec2(texture.Rect.Width * GetOrigin(xOrigin), texture.Rect.Height * GetOrigin(yOrigin)),
                scale ?? Vec2.One,
                SpriteEffects,
                depth
            );

        public static void Text(this SpriteBatch spriteBatch, DynamicSpriteFontScaled font, string text, Vec2 position, float scale = 1, Origin xOrigin = Origin.Center, Origin yOrigin = Origin.Center) =>
            Text(spriteBatch, font, Color.White, text, position, scale, xOrigin, yOrigin);

        public static void Text(this SpriteBatch spriteBatch, DynamicSpriteFontScaled font, string text, Vec2 position, Vec2? scale, Origin xOrigin = Origin.Center, Origin yOrigin = Origin.Center) =>
            Text(spriteBatch, font, Color.White, text, position, scale, xOrigin, yOrigin);

        public static void Text(this SpriteBatch spriteBatch, DynamicSpriteFontScaled font, Color color, string text, Vec2 position, float scale = 1, Origin xOrigin = Origin.Center, Origin yOrigin = Origin.Center)
        {
            Vec2 size = font.Dynamic.MeasureString(text) * scale;
            font.Dynamic.DrawText(spriteBatch,
                text,
                position,
                color, 0, size * new Vec2(GetOrigin(xOrigin), GetOrigin(yOrigin)),
                new Vec2(scale) * font.Scale
            );
        }

        public static void Text(this SpriteBatch spriteBatch, DynamicSpriteFontScaled font, Color color, string text, Vec2 position, Vec2? scale, Origin xOrigin = Origin.Center, Origin yOrigin = Origin.Center)
        {
            Vec2 rscale = scale ?? Vec2.One;
            Vec2 size = font.Dynamic.MeasureString(text) * (Vector2)rscale;
            font.Dynamic.DrawText(spriteBatch,
                text,
                position,
                color, 0, size * new Vec2(GetOrigin(xOrigin), GetOrigin(yOrigin)),
                rscale * font.Scale
            );
        }

        public static void RectXLine(this SpriteBatch spriteBatch, Sprite texture, Vec2 a, Vec2 b, float depth = 0, float scale = 1, Origin yOrigin = Origin.Center)
        {
            Vec2 diff = b - a;

            Vec2 position = a;
            float rotation = MathHelper.ToDegrees((float)Math.Atan2(diff.Y, diff.X));

            Rect(spriteBatch, texture, position, new Vec2(diff.Length() / texture.Width, scale), rotation, depth, Origin.Zero, yOrigin);
        }

        public static void RectYLine(this SpriteBatch spriteBatch, Sprite texture, Vec2 a, Vec2 b, float depth = 0, float scale = 1, Origin xOrigin = Origin.Center)
        {
            Vec2 diff = b - a;

            Vec2 position = a;
            float rotation = MathHelper.ToDegrees((float)Math.Atan2(-diff.X, diff.Y));

            Rect(spriteBatch, texture, position, new Vec2(scale, diff.Length() / texture.Height), rotation, depth, xOrigin, Origin.Zero);
        }

        public static void RectXArrow(this SpriteBatch spriteBatch, Sprite line, Sprite texture, Vec2 a, Vec2 b, float depth = 0, float scale = 1, Origin yOrigin = Origin.Center)
        {
            Vec2 diff = b - a;

            Vec2 position = a;
            float rotation = MathHelper.ToDegrees((float)Math.Atan2(diff.Y, diff.X));

            Rect(spriteBatch, line, position, new Vec2(diff.Length() / line.Width, scale), rotation, depth, Origin.Zero, yOrigin);

            Rect(spriteBatch, texture, b, rotation, 1, depth, Origin.Center, yOrigin);
        }

        public static void RectYArrow(this SpriteBatch spriteBatch, Sprite line, Sprite texture, Vec2 a, Vec2 b, float depth = 0, float scale = 1, Origin xOrigin = Origin.Center)
        {
            Vec2 diff = b - a;

            Vec2 position = a;
            float rotation = MathHelper.ToDegrees((float)Math.Atan2(-diff.X, diff.Y));

            Rect(spriteBatch, line, position, new Vec2(scale, diff.Length() / line.Height), rotation, depth, xOrigin, Origin.Zero);

            Rect(spriteBatch, texture, b, rotation, 1, depth, Origin.Center, xOrigin);
        }

        private static float GetOrigin(Origin origin) =>
            origin == Origin.One ? 1 : origin == Origin.Center ? 0.5f : 0;
    }
    public enum Origin { One, Center, Zero }
}
