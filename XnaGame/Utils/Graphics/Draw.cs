﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteFontPlus;
using System;

namespace XnaGame.Utils.Graphics
{
    public static class SDraw
    {
        public static SpriteBatch spriteBatch;

        public static bool Updated = false;

        public static SpriteEffects SpriteEffects { get; set; } = SpriteEffects.None;
        public static SpriteSortMode SortMode { get; set; } = SpriteSortMode.Deferred;
        public static Matrix? Matrix { get; set; } = null;
        public static bool Batching { get; private set; } = false;

        public static void Apply()
        {
            if (Batching) spriteBatch.End();
            spriteBatch.Begin(SortMode, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, Matrix);
            Batching = true;
        }

        public static void End()
        {
            spriteBatch.End();
            Batching = false;
        }

        public static void Rect(Sprite texture, FRectangle position, float rotation = 0, float depth = 0) =>
            Rect(texture, Color.White, position, rotation, depth);

        public static void Rect(Sprite texture, Color color, FRectangle position, float rotation = 0, float depth = 0) =>
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

        public static void Rect(Sprite texture, Vec2 position, float rotation = 0, float scale = 1, float depth = 0, Origin xOrigin = Origin.Center, Origin yOrigin = Origin.Center) =>
            Rect(texture, Color.White, position, rotation, scale, depth, xOrigin, yOrigin);

        public static void Rect(Sprite texture, Color color, Vec2 position, float rotation = 0, float scale = 1, float depth = 0, Origin xOrigin = Origin.Center, Origin yOrigin = Origin.Center) =>
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

        public static void Rect(Sprite texture, Vec2 position, Vec2? scale, float rotation = 0, float depth = 0, Origin xOrigin = Origin.Center, Origin yOrigin = Origin.Center) =>
            Rect(texture, Color.White, position, scale, rotation, depth, xOrigin, yOrigin);

        public static void Rect(Sprite texture, Color color, Vec2 position, Vec2? scale, float rotation = 0, float depth = 0, Origin xOrigin = Origin.Center, Origin yOrigin = Origin.Center) =>
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

        public static void Text(DynamicSpriteFontScaled font, string text, Vec2 position, float scale = 1, float depth = 0, Origin xOrigin = Origin.Center, Origin yOrigin = Origin.Center) =>
            Text(font, Color.White, text, position, scale, depth, xOrigin, yOrigin);

        public static void Text(DynamicSpriteFontScaled font, string text, Vec2 position, Vec2? scale, float depth = 0, Origin xOrigin = Origin.Center, Origin yOrigin = Origin.Center) =>
            Text(font, Color.White, text, position, scale, depth, xOrigin, yOrigin);

        public static void Text(DynamicSpriteFontScaled font, Color color, string text, Vec2 position, float scale = 1, float depth = 0, Origin xOrigin = Origin.Center, Origin yOrigin = Origin.Center)
        {
            Vec2 size = font.dynamic.MeasureString(text) * scale * font.scale;
            font.dynamic.DrawString(spriteBatch,
                text,
                position - new Vec2(size.X * GetOrigin(xOrigin), size.Y * GetOrigin(yOrigin)),
                color,
                new Vec2(scale) * font.scale,
                depth
            );
        }

        public static void Text(DynamicSpriteFontScaled font, Color color, string text, Vec2 position, Vec2? scale, float depth = 0, Origin xOrigin = Origin.Center, Origin yOrigin = Origin.Center)
        {
            Vec2 size = font.dynamic.MeasureString(text) * font.scale;
            size *= scale ?? Vec2.One;
            font.dynamic.DrawString(spriteBatch,
                text,
                position - new Vec2(size.X * GetOrigin(xOrigin), size.Y * GetOrigin(yOrigin)),
                color,
                (scale ?? Vec2.One) * font.scale,
                depth
            );
        }

        public static void RectXLine(Sprite texture, Vec2 a, Vec2 b, float depth = 0, float scale = 1, Origin yOrigin = Origin.Center)
        {
            Vec2 diff = b - a;

            Vec2 position = a;
            float rotation = MathHelper.ToDegrees((float)Math.Atan2(diff.Y, diff.X));

            Rect(texture, position, new Vec2(diff.Length() / texture.Width, scale), rotation, depth, Origin.Zero, yOrigin);
        }

        public static void RectYLine(Sprite texture, Vec2 a, Vec2 b, float depth = 0, float scale = 1, Origin xOrigin = Origin.Center)
        {
            Vec2 diff = b - a;

            Vec2 position = a;
            float rotation = MathHelper.ToDegrees((float)Math.Atan2(-diff.X, diff.Y));

            Rect(texture, position, new Vec2(scale, diff.Length() / texture.Height), rotation, depth, xOrigin, Origin.Zero);
        }

        public static void RectXArrow(Sprite line, Sprite texture, Vec2 a, Vec2 b, float depth = 0, float scale = 1, Origin yOrigin = Origin.Center)
        {
            Vec2 diff = b - a;

            Vec2 position = a;
            float rotation = MathHelper.ToDegrees((float)Math.Atan2(diff.Y, diff.X));

            Rect(line, position, new Vec2(diff.Length() / line.Width, scale), rotation, depth, Origin.Zero, yOrigin);

            Rect(texture, b, rotation, 1, depth, Origin.Center, yOrigin);
        }

        public static void RectYArrow(Sprite line, Sprite texture, Vec2 a, Vec2 b, float depth = 0, float scale = 1, Origin xOrigin = Origin.Center)
        {
            Vec2 diff = b - a;

            Vec2 position = a;
            float rotation = MathHelper.ToDegrees((float)Math.Atan2(-diff.X, diff.Y));

            Rect(line, position, new Vec2(scale, diff.Length() / line.Height), rotation, depth, xOrigin, Origin.Zero);

            Rect(texture, b, rotation, 1, depth, Origin.Center, xOrigin);
        }

        private static float GetOrigin(Origin origin) =>
            origin == Origin.One ? 1 : origin == Origin.Center ? 0.5f : 0;
    }
    public enum Origin { One, Center, Zero }
}
