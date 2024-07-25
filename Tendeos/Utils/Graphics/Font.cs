using FontStashSharp;
using System.IO;
using System.Text;

namespace Tendeos.Utils.Graphics
{
    public class Font
    {
        public FontSystemSettings settings;
        public FontSystem fontSystem;
        public DynamicSpriteFont Dynamic { get; private set; }
        public float Scale { get; }
        public float LineHeight => Dynamic.LineHeight * Scale;

        private readonly float defaultSize;

        public Font(float defaultSize, float scale)
        {
            settings = new FontSystemSettings();
            fontSystem = new FontSystem(settings)
            {
                UseKernings = true
            };
            this.defaultSize = defaultSize;
            Scale = scale;
        }

        public void Load(string path) => fontSystem.AddFont(File.ReadAllBytes(path));
        public void Init() => Dynamic = fontSystem.GetFont(defaultSize);

        public Vec2 MeasureString(string text, float scale = 1, float characterSpacing = 0f, float lineSpacing = 0f,
            FontSystemEffect effect = FontSystemEffect.None, int effectAmount = 0) =>
            Dynamic.MeasureString(text, new Vec2(scale * Scale), characterSpacing, lineSpacing, effect, effectAmount);

        public Vec2 MeasureString(string text, Vec2? scale, float characterSpacing = 0f, float lineSpacing = 0f,
            FontSystemEffect effect = FontSystemEffect.None, int effectAmount = 0) =>
            Dynamic.MeasureString(text, scale * Scale ?? new Vec2(Scale), characterSpacing, lineSpacing, effect,
                effectAmount);

        public BoxedTextData GetBoxedTextData(string text, FRectangle rectangle, float scale = 1) =>
            GetBoxedTextData(text, rectangle, new Vec2(scale));

        public BoxedTextData GetBoxedTextData(string text, FRectangle rectangle, Vec2? scale)
        {
            StringBuilder resultMessage = new();
            var (lines, last, start) = (1, 0, 0);
            bool inWord = false;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\n' || text[i] == ' ' || text[i] == '\t') inWord = false;
                else
                {
                    if (!inWord) start = i;
                    inWord = true;
                }
                if (text[i] == '\n')
                {
                    resultMessage.Append('\n').Append(text[last..i]);
                    last = i;
                    lines += 2;
                }
                else if (MeasureString(text[last..i], scale).X >= rectangle.Width)
                {
                    if (inWord && MeasureString(text[start..i], scale).X < rectangle.Width)
                    {
                        resultMessage.Append('\n').Append(text[last..start]);
                        last = start;
                    }
                    else
                    {
                        resultMessage.Append('\n').Append(text[last..i]);
                        last = i;
                        start = i;
                    }
                    lines++;
                }
            }

            resultMessage.Append('\n').Append(text[last..]);
            return new BoxedTextData(scale ?? Vec2.One, lines * LineHeight, rectangle, resultMessage.ToString().Trim(),
                this);
        }

        public struct BoxedTextData
        {
            public readonly Vec2 scale;
            public float height;
            public FRectangle rectangle;
            public readonly string text;
            public readonly Font font;

            public BoxedTextData(Vec2 scale, float height, FRectangle rectangle, string text, Font font)
            {
                this.scale = scale;
                this.height = height;
                this.rectangle = rectangle;
                this.text = text;
                this.font = font;
            }

            public void Draw(SpriteBatch spriteBatch, Vec2? position = null)
            {
                spriteBatch.Text(font, text, position ?? rectangle.Location, scale, 0, 0, 0);
            }
        }
    }
}