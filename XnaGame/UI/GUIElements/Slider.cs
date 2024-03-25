using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XnaGame.Utils;
using XnaGame.Utils.Graphics;
using XnaGame.Utils.Input;

namespace XnaGame.UI.GUIElements
{
    public class Slider : GUIElement
    {
        protected readonly Range range;
        protected readonly Style style;
        protected readonly Func<float> get;
        protected readonly Action<float> set;

        public Slider(Vec2 anchor, float offset, Range range, Style style, Func<float> get, Action<float> set) : base(anchor, style.Type switch {
            Type.Up2Down => new FRectangle(offset, range.Start.Value, style.Sprites[0].Height, range.End.Value - range.Start.Value),
            Type.Down2Up => new FRectangle(offset, range.Start.Value, style.Sprites[0].Height, range.End.Value - range.Start.Value),
            Type.Left2Right => new FRectangle(range.Start.Value, offset, range.End.Value - range.Start.Value, style.Sprites[0].Height),
            Type.Right2Left => new FRectangle(range.Start.Value, offset, range.End.Value - range.Start.Value, style.Sprites[0].Height),
        })
        {
            this.range = range;
            this.style = style;
            this.get = get;
            this.set = set;
        }

        public override void Draw(SpriteBatch spriteBatch, FRectangle rectangle)
        {
            Vec2 center = rectangle.Center;
            float value = get();
            switch (style.Type)
            {
                case Type.Up2Down:
                    spriteBatch.Rect(style.Sprites[0], new Vec2(center.X, rectangle.Bottom), 90, 1, 0, Origin.Zero);
                    spriteBatch.Rect(style.Sprites[1], new Vec2(center.X, rectangle.Bottom + style.Sprites[0].Rect.Width), new Vec2((rectangle.Height - style.Sprites[0].Rect.Width*2) / style.Sprites[0].Rect.Width, 1), 90, 0, Origin.Zero);
                    spriteBatch.Rect(style.Sprites[2], new Vec2(center.X, rectangle.Top), 90, 1, 0, Origin.One);
                    spriteBatch.Rect(style.Sprites[3], new Vec2(center.X, rectangle.Bottom + style.Bar.Start.Value), new Vec2((rectangle.Height - style.Bar.End.Value - style.Bar.Start.Value) * value / style.Sprites[0].Rect.Width, 1), 90, 0, Origin.Zero);
                    break;
                case Type.Down2Up:
                    spriteBatch.Rect(style.Sprites[0], new Vec2(center.X, rectangle.Bottom), 90, 1, 0, Origin.Zero);
                    spriteBatch.Rect(style.Sprites[1], new Vec2(center.X, rectangle.Bottom + style.Sprites[0].Rect.Width), new Vec2((rectangle.Height - style.Sprites[0].Rect.Width * 2) / style.Sprites[0].Rect.Width, 1), 90, 0, Origin.Zero);
                    spriteBatch.Rect(style.Sprites[2], new Vec2(center.X, rectangle.Top), 90, 1, 0, Origin.One);
                    spriteBatch.Rect(style.Sprites[3], new Vec2(center.X, rectangle.Top - style.Bar.End.Value), new Vec2((rectangle.Height - style.Bar.End.Value - style.Bar.Start.Value) * value / style.Sprites[0].Rect.Width, 1), -90, 0, Origin.Zero);
                    break;
                case Type.Left2Right:
                    spriteBatch.Rect(style.Sprites[0], new Vec2(rectangle.Left, center.Y), 0, 1, 0, Origin.Zero);
                    spriteBatch.Rect(style.Sprites[1], new Vec2(rectangle.Left + style.Sprites[0].Rect.Width, center.Y), new Vec2((rectangle.Width - style.Sprites[0].Rect.Width * 2) / style.Sprites[0].Rect.Width, 1), 0, 0, Origin.Zero);
                    spriteBatch.Rect(style.Sprites[2], new Vec2(rectangle.Right, center.Y), 0, 1, 0, Origin.One);
                    spriteBatch.Rect(style.Sprites[3], new Vec2(rectangle.Left + style.Bar.Start.Value, center.Y), new Vec2((rectangle.Width - style.Bar.End.Value - style.Bar.Start.Value) * value / style.Sprites[0].Rect.Width, 1), 0, 0, Origin.Zero);
                    break;
                case Type.Right2Left:
                    spriteBatch.Rect(style.Sprites[0], new Vec2(rectangle.Left, center.Y), 0, 1, 0, Origin.Zero);
                    spriteBatch.Rect(style.Sprites[1], new Vec2(rectangle.Left + style.Sprites[0].Rect.Width, center.Y), new Vec2((rectangle.Width - style.Sprites[0].Rect.Width * 2) / style.Sprites[0].Rect.Width, 1), 0, 0, Origin.Zero);
                    spriteBatch.Rect(style.Sprites[2], new Vec2(rectangle.Right, center.Y), 0, 1, 0, Origin.One);
                    spriteBatch.Rect(style.Sprites[3], new Vec2(rectangle.Right - style.Bar.End.Value, center.Y), new Vec2((rectangle.Width - style.Bar.End.Value - style.Bar.Start.Value) * value / style.Sprites[0].Rect.Width, 1), 180, 0, Origin.Zero);
                    break;
            }

            base.Draw(spriteBatch, rectangle);
        }

        public override void Update(FRectangle rectangle)
        {
            base.Update(rectangle);

            if (MouseOn && Mouse.LeftDown)
                switch (style.Type)
                {
                    case Type.Up2Down:
                        set(Math.Clamp((Mouse.GUIPosition.Y - (rectangle.Bottom + style.Bar.Start.Value)) / (rectangle.Height - style.Bar.End.Value - style.Bar.Start.Value), 0, 1));
                        break;
                    case Type.Down2Up:
                        set(Math.Clamp((rectangle.Top - style.Bar.End.Value - Mouse.GUIPosition.Y) / (rectangle.Height - style.Bar.End.Value - style.Bar.Start.Value), 0, 1));
                        break;
                    case Type.Left2Right:
                        set(Math.Clamp((Mouse.GUIPosition.X - (rectangle.Left + style.Bar.Start.Value)) / (rectangle.Width - style.Bar.End.Value - style.Bar.Start.Value), 0, 1));
                        break;
                    case Type.Right2Left:
                        set(Math.Clamp((rectangle.Right - style.Bar.End.Value - Mouse.GUIPosition.X) / (rectangle.Width - style.Bar.End.Value - style.Bar.Start.Value), 0, 1));
                        break;
                }
        }

        public enum Type { Down2Up, Up2Down, Left2Right, Right2Left }

        public class Style
        {
            public Sprite[] Sprites { get; init; }
            public Range Bar { get; init; }
            public Type Type { get; init; }

            public Style(Sprite sprite, Range bar, Type type)
            {
                Sprites = sprite.Split(4, 1, 1);
                Bar = bar;
                Type = type;
            }
        }
    }
}
