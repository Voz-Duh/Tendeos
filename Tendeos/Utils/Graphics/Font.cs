using Esprima;
using FontStashSharp;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Text;

namespace Tendeos.Utils.Graphics
{
    public class Font
    {
        public DynamicSpriteFont Dynamic { get; }
        public float Scale { get; }
        public float LineHeight => Dynamic.LineHeight * Scale;

        public Font(ContentManager content, string[] files, float defaultSize, float scale)
        {
            var settings = new FontSystemSettings();
            var fontSystem = new FontSystem(settings)
            {
                UseKernings = true
            };

            for (int i = 0; i < files.Length; i++)
            {
                fontSystem.AddFont(content.LoadFileBytes(files[i]));
            }

            Dynamic = fontSystem.GetFont(defaultSize);
            Scale = scale;
        }

        public Vec2 MeasureString(string text, float scale = 1, float characterSpacing = 0f, float lineSpacing = 0f, FontSystemEffect effect = FontSystemEffect.None, int effectAmount = 0) =>
            Dynamic.MeasureString(text, new Vec2(scale * Scale), characterSpacing, lineSpacing, effect, effectAmount);

        public Vec2 MeasureString(string text, Vec2? scale, float characterSpacing = 0f, float lineSpacing = 0f, FontSystemEffect effect = FontSystemEffect.None, int effectAmount = 0) =>
            Dynamic.MeasureString(text, scale * Scale ?? new Vec2(Scale), characterSpacing, lineSpacing, effect, effectAmount);

        public FRectangle[] GetTextRects(string text, Vec2 position, float scale = 1, float characterSpacing = 0f, float lineSpacing = 0f, Origin xOrigin = Origin.Center, Origin yOrigin = Origin.Center)
        {
            Vec2 size = MeasureString(text, scale, characterSpacing, lineSpacing);
            List<FRectangle> rectangles = new List<FRectangle>();
            Vec2 o = position - size * new Vec2(xOrigin.ToFloat(), yOrigin.ToFloat());
            float s = scale * Scale;
            foreach (var glyph in Dynamic.GetGlyphs(text, Vec2.Zero, Vec2.Zero, Vec2.One, characterSpacing, lineSpacing))
            {
                rectangles.Add(new FRectangle((Vec2)glyph.Bounds.Location.ToVector2() * s + o, (Vec2)glyph.Bounds.Size.ToVector2() * s));
            }
            return rectangles.ToArray();
        }

        public FRectangle[] GetTextRects(string text, Vec2 position, Vec2? scale, float characterSpacing = 0f, float lineSpacing = 0f, Origin xOrigin = Origin.Center, Origin yOrigin = Origin.Center)
        {
            Vec2 size = MeasureString(text, scale ?? Vec2.One, characterSpacing, lineSpacing);
            List<FRectangle> rectangles = new List<FRectangle>();
            Vec2 o = position - size * new Vec2(xOrigin.ToFloat(), yOrigin.ToFloat());
            Vec2 s = scale * Scale ?? new Vec2(Scale);
            foreach (var glyph in Dynamic.GetGlyphs(text, Vec2.Zero, Vec2.Zero, Vec2.One, characterSpacing, lineSpacing))
            {
                rectangles.Add(new FRectangle((Vec2)glyph.Bounds.Location.ToVector2() * s + o, (Vec2)glyph.Bounds.Size.ToVector2() * s));
            }
            return rectangles.ToArray();
        }

        public BoxedTextData GetBoxedTextData(string text, FRectangle rectangle, float scale = 1)
        {
            StringBuilder resultMessage = new StringBuilder();
            int lines = 1;
            int last = 0;
            int i;
            for (i = 0; i < text.Length; i++)
            {
                if (text[i] == '\n')
                {
                    resultMessage.Append('\n').Append(text[last..i]);
                    last = i;
                    lines += 2;
                }
                else if (MeasureString(text[last..i], scale).X >= rectangle.Width)
                {
                    resultMessage.Append('\n').Append(text[last..i]);
                    last = i;
                    lines++;
                }
            }
            resultMessage.Append('\n').Append(text[last..]);
            return new BoxedTextData(new Vec2(scale), lines * LineHeight, rectangle, resultMessage.ToString().Trim(), this);
        }

        public BoxedTextData GetBoxedTextData(string text, FRectangle rectangle, Vec2? scale)
        {
            StringBuilder resultMessage = new StringBuilder();
            int lines = 1;
            int last = 0;
            int i;
            for (i = 0; i < text.Length; i++)
            {
                if (text[i] == '\n')
                {
                    resultMessage.Append('\n').Append(text[last..i]);
                    last = i;
                    lines += 2;
                }
                else if (MeasureString(text[last..i], scale).X >= rectangle.Width)
                {
                    resultMessage.Append('\n').Append(text[last..i]);
                    last = i;
                    lines++;
                }
            }
            resultMessage.Append('\n').Append(text[last..]);
            return new BoxedTextData(scale ?? Vec2.One, lines * LineHeight, rectangle, resultMessage.ToString().Trim(), this);
        }

        public struct BoxedTextData
        {
            Vec2 scale;
            public float height;
            public FRectangle rectangle;
            string text;
            Font font;

            public BoxedTextData(Vec2 scale, float height, FRectangle rectangle, string text, Font font)
            {
                this.scale = scale;
                this.height = height;
                this.rectangle = rectangle;
                this.text = text;
                this.font = font;
            }

            public void Draw(SpriteBatch spriteBatch)
            {
                spriteBatch.Text(font, text, rectangle.Location, scale, Origin.Zero, Origin.Zero);
            }
        }
    }
}