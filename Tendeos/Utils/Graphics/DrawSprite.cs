using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Text;

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
                new Vec2(texture.Rect.Width * xOrigin.ToFloat(), texture.Rect.Height * yOrigin.ToFloat()),
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
                new Vec2(texture.Rect.Width * xOrigin.ToFloat(), texture.Rect.Height * yOrigin.ToFloat()),
                scale ?? Vec2.One,
                SpriteEffects,
                depth
            );

        public static void Text(this SpriteBatch spriteBatch, Font font, string text, Vec2 position, float scale = 1, Origin xOrigin = Origin.Center, Origin yOrigin = Origin.Center) =>
            Text(spriteBatch, font, Color.White, text, position, scale, xOrigin, yOrigin);

        public static void Text(this SpriteBatch spriteBatch, Font font, string text, Vec2 position, Vec2? scale, Origin xOrigin = Origin.Center, Origin yOrigin = Origin.Center) =>
            Text(spriteBatch, font, Color.White, text, position, scale, xOrigin, yOrigin);

        public static void Text(this SpriteBatch spriteBatch, Font font, Color color, string text, Vec2 position, float scale = 1, Origin xOrigin = Origin.Center, Origin yOrigin = Origin.Center)
        {
            Vec2 size = font.MeasureString(text, scale);
            font.Dynamic.DrawText(spriteBatch,
                text,
                position - size * new Vec2(xOrigin.ToFloat(), yOrigin.ToFloat()),
                color, 0, Vector2.Zero,
                new Vec2(scale) * font.Scale
            );
        }

        public static void Text(this SpriteBatch spriteBatch, Font font, Color color, string text, Vec2 position, Vec2? scale, Origin xOrigin = Origin.Center, Origin yOrigin = Origin.Center)
        {
            Vec2 rscale = scale ?? Vec2.One;
            Vec2 size = font.MeasureString(text, rscale);
            font.Dynamic.DrawText(spriteBatch,
                text,
                position - size * new Vec2(xOrigin.ToFloat(), yOrigin.ToFloat()),
                color, 0, Vec2.Zero,
                rscale * font.Scale
            );
        }

        public static float Text(this SpriteBatch spriteBatch, Font font, string text, FRectangle rectangle, float scale = 1) =>
            Text(spriteBatch, font, Color.White, text, rectangle, scale);

        public static float Text(this SpriteBatch spriteBatch, Font font, string text, FRectangle rectangle, Vec2? scale) =>
            Text(spriteBatch, font, Color.White, text, rectangle, scale);

        public static float Text(this SpriteBatch spriteBatch, Font font, Color color, string text, FRectangle rectangle, float scale = 1)
        {
            FRectangle[] rects = font.GetTextRects(text, Vec2.Zero, scale);
            StringBuilder resultMessage = new StringBuilder();
            int lines = 1;
            int last = 0;
            int i;
            float width = 0;
            for (i = 0; i < text.Length; i++)
            {
                width += rects[i].Width;
                if (text[i] == '\n')
                {
                    resultMessage.Append(text[last..i]);
                    last = i;
                    lines++;
                    width = 0;
                }
                else if (width >= rectangle.Width)
                {
                    resultMessage.Append('\n').Append(text[last..i]);
                    last = i;
                    lines++;
                    width = 0;
                }
            }
            resultMessage.Append('\n').Append(text[last..]);
            spriteBatch.Text(font, color, resultMessage.ToString().Trim(), rectangle.Location, scale, Origin.Zero, Origin.Zero);
            return lines * font.LineHeight;
        }

        public static float Text(this SpriteBatch spriteBatch, Font font, Color color, string text, FRectangle rectangle, Vec2? scale)
        {
            FRectangle[] rects = font.GetTextRects(text, Vec2.Zero, scale);
            StringBuilder resultMessage = new StringBuilder();
            int lines = 1;
            int last = 0;
            int i;
            float width = 0;
            for (i = 0; i < text.Length; i++)
            {
                width += rects[i].Width;
                if (text[i] == '\n')
                {
                    resultMessage.Append(text[last..i]);
                    last = i;
                    lines++;
                    width = 0;
                }
                else if (width >= rectangle.Width)
                {
                    resultMessage.Append('\n').Append(text[last..i]);
                    last = i;
                    lines++;
                    width = 0;
                }
            }
            resultMessage.Append('\n').Append(text[last..]);
            spriteBatch.Text(font, color, resultMessage.ToString().Trim(), rectangle.Location, scale, Origin.Zero, Origin.Zero);
            return lines * font.LineHeight;
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

        public static float ToFloat(this Origin origin) =>
            origin == Origin.One ? 1 : origin == Origin.Center ? 0.5f : 0;
    }
    public enum Origin { One, Center, Zero }
}
