using Microsoft.Xna.Framework.Graphics;
using System;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;
using Tendeos.Utils.Input;

namespace Tendeos.UI.GUIElements
{
    public class Slider : GUIElement
    {
        protected readonly Type type;
        protected readonly Style style;
        protected readonly Func<float> get;
        protected readonly Action<float> set;

        public Slider(Vec2 anchor, Type type, float offset, float start, float end, Style style, Func<float> get, Action<float> set) : base(anchor, type switch
        {
            Type.Up2Down => new FRectangle(offset, start, style.Sprites[0].Height, end - start),
            Type.Down2Up => new FRectangle(offset, start, style.Sprites[0].Height, end - start),
            Type.Left2Right => new FRectangle(start, offset, end - start, style.Sprites[0].Height),
            Type.Right2Left => new FRectangle(start, offset, end - start, style.Sprites[0].Height),
        })
        {
            this.type = type;
            this.style = style;
            this.get = get;
            this.set = set;
        }

        public override void Draw(SpriteBatch spriteBatch, FRectangle rectangle)
        {
            Vec2 center = rectangle.Center;
            unsafe
            {
                float value = get();
                switch (type)
                {
                    case Type.Up2Down:
                        spriteBatch.Rect(style.Sprites[0], new Vec2(center.X, rectangle.Bottom), 90, 1, 0, Origin.Zero);
                        spriteBatch.Rect(style.Sprites[1], new Vec2(center.X, rectangle.Bottom + style.Sprites[0].Rect.Width), new Vec2((rectangle.Height - style.Sprites[0].Rect.Width * 2) / style.Sprites[0].Rect.Width, 1), 90, 0, Origin.Zero);
                        spriteBatch.Rect(style.Sprites[2], new Vec2(center.X, rectangle.Top), 90, 1, 0, Origin.One);
                        if (style.HasBar) spriteBatch.Rect(style.Sprites[3], new Vec2(center.X, rectangle.Bottom + style.Bar.Start.Value), new Vec2((rectangle.Height - style.Bar.End.Value - style.Bar.Start.Value) * value / style.Sprites[0].Rect.Width, 1), 90, 0, Origin.Zero);
                        if (style.Thumb.HasValue) spriteBatch.Rect(style.Thumb.Value, new Vec2(center.X, rectangle.Bottom + style.Bar.Start.Value + (rectangle.Height - style.Bar.End.Value - style.Bar.Start.Value) * value), 90);
                        break;
                    case Type.Down2Up:
                        spriteBatch.Rect(style.Sprites[0], new Vec2(center.X, rectangle.Bottom), 90, 1, 0, Origin.Zero);
                        spriteBatch.Rect(style.Sprites[1], new Vec2(center.X, rectangle.Bottom + style.Sprites[0].Rect.Width), new Vec2((rectangle.Height - style.Sprites[0].Rect.Width * 2) / style.Sprites[0].Rect.Width, 1), 90, 0, Origin.Zero);
                        spriteBatch.Rect(style.Sprites[2], new Vec2(center.X, rectangle.Top), 90, 1, 0, Origin.One);
                        if (style.HasBar) spriteBatch.Rect(style.Sprites[3], new Vec2(center.X, rectangle.Top - style.Bar.End.Value), new Vec2((rectangle.Height - style.Bar.End.Value - style.Bar.Start.Value) * value / style.Sprites[0].Rect.Width, 1), -90, 0, Origin.Zero);
                        if (style.Thumb.HasValue) spriteBatch.Rect(style.Thumb.Value, new Vec2(center.X, rectangle.Top - style.Bar.End.Value - (rectangle.Height - style.Bar.End.Value - style.Bar.Start.Value) * value), -90);
                        break;
                    case Type.Left2Right:
                        spriteBatch.Rect(style.Sprites[0], new Vec2(rectangle.Left, center.Y), 0, 1, 0, Origin.Zero);
                        spriteBatch.Rect(style.Sprites[1], new Vec2(rectangle.Left + style.Sprites[0].Rect.Width, center.Y), new Vec2((rectangle.Width - style.Sprites[0].Rect.Width * 2) / style.Sprites[0].Rect.Width, 1), 0, 0, Origin.Zero);
                        spriteBatch.Rect(style.Sprites[2], new Vec2(rectangle.Right, center.Y), 0, 1, 0, Origin.One);
                        if (style.HasBar) spriteBatch.Rect(style.Sprites[3], new Vec2(rectangle.Left + style.Bar.Start.Value, center.Y), new Vec2((rectangle.Width - style.Bar.End.Value - style.Bar.Start.Value) * value / style.Sprites[0].Rect.Width, 1), 0, 0, Origin.Zero);
                        if (style.Thumb.HasValue) spriteBatch.Rect(style.Thumb.Value, new Vec2(rectangle.Left + style.Bar.Start.Value + (rectangle.Width - style.Bar.End.Value - style.Bar.Start.Value) * value, center.Y), 0);
                        break;
                    case Type.Right2Left:
                        spriteBatch.Rect(style.Sprites[0], new Vec2(rectangle.Left, center.Y), 0, 1, 0, Origin.Zero);
                        spriteBatch.Rect(style.Sprites[1], new Vec2(rectangle.Left + style.Sprites[0].Rect.Width, center.Y), new Vec2((rectangle.Width - style.Sprites[0].Rect.Width * 2) / style.Sprites[0].Rect.Width, 1), 0, 0, Origin.Zero);
                        spriteBatch.Rect(style.Sprites[2], new Vec2(rectangle.Right, center.Y), 0, 1, 0, Origin.One);
                        if (style.HasBar) spriteBatch.Rect(style.Sprites[3], new Vec2(rectangle.Right - style.Bar.End.Value, center.Y), new Vec2((rectangle.Width - style.Bar.End.Value - style.Bar.Start.Value) * value / style.Sprites[0].Rect.Width, 1), 180, 0, Origin.Zero);
                        if (style.Thumb.HasValue) spriteBatch.Rect(style.Thumb.Value, new Vec2(rectangle.Right - style.Bar.End.Value - (rectangle.Width - style.Bar.End.Value - style.Bar.Start.Value) * value, center.Y), 180);
                        break;
                }
            }
        }

        public override void Update(FRectangle rectangle)
        {
            base.Update(rectangle);

            if (MouseOn && Mouse.LeftDown)
                unsafe
                {
                    float res = 0;
                    switch (type)
                    {
                        case Type.Up2Down:
                            res = Math.Clamp((Mouse.GUIPosition.Y - (rectangle.Bottom + style.Bar.Start.Value)) / (rectangle.Height - style.Bar.End.Value - style.Bar.Start.Value), 0, 1);
                            break;
                        case Type.Down2Up:
                            res = Math.Clamp((rectangle.Top - style.Bar.End.Value - Mouse.GUIPosition.Y) / (rectangle.Height - style.Bar.End.Value - style.Bar.Start.Value), 0, 1);
                            break;
                        case Type.Left2Right:
                            res = Math.Clamp((Mouse.GUIPosition.X - (rectangle.Left + style.Bar.Start.Value)) / (rectangle.Width - style.Bar.End.Value - style.Bar.Start.Value), 0, 1);
                            break;
                        case Type.Right2Left:
                            res = Math.Clamp((rectangle.Right - style.Bar.End.Value - Mouse.GUIPosition.X) / (rectangle.Width - style.Bar.End.Value - style.Bar.Start.Value), 0, 1);
                            break;
                    }
                    set(res);
                }
        }

        public enum Type { Down2Up, Up2Down, Left2Right, Right2Left }

        public class Style
        {
            public Sprite[] Sprites { get; }
            public Range Bar { get; }
            public bool HasBar { get; }
            public Sprite? Thumb { get; }

            public Style(Sprite sprite, Range bar, bool hasBar)
            {
                Sprites = sprite.Split(hasBar ? 4 : 3, 1, 1);
                Thumb = null;
                Bar = bar;
                HasBar = hasBar;
            }

            public Style(Sprite sprite, Range bar, bool hasBar, Sprite thumb)
            {
                Sprites = sprite.Split(hasBar ? 4 : 3, 1, 1);
                Thumb = thumb;
                Bar = bar;
                HasBar = hasBar;
            }
        }
    }
}
