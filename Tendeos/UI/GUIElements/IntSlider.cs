using System;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;
using Tendeos.Utils.Input;

namespace Tendeos.UI.GUIElements
{
    public class IntSlider : GUIElement
    {
        public int sections;
        protected readonly Func<int> get;
        protected readonly Action<int> set;
        protected readonly Slider.Type type;
        public readonly Slider.Style style;

        public IntSlider(Vec2 anchor, Slider.Type type, float offset, float start, float end, Slider.Style style,
            int sections, Func<int> get, Action<int> set, GUIElement[] childs = null) : base(anchor, type switch
        {
            Slider.Type.Up2Down => new FRectangle(offset, start, style.Sprites[0].Rect.Height, end - start),
            Slider.Type.Down2Up => new FRectangle(offset, start, style.Sprites[0].Rect.Height, end - start),
            Slider.Type.Left2Right => new FRectangle(start, offset, end - start, style.Sprites[0].Rect.Height),
            Slider.Type.Right2Left => new FRectangle(start, offset, end - start, style.Sprites[0].Rect.Height),
        }, childs)
        {
            this.type = type;
            this.style = style;
            this.sections = sections;
            this.get = get;
            this.set = set;
        }

        public override void Draw(SpriteBatch spriteBatch, FRectangle rectangle)
        {
            Vec2 center = rectangle.Center;
            unsafe
            {
                int value = get();
                switch (type)
                {
                    case Slider.Type.Up2Down:
                        spriteBatch.Rect(style.Sprites[0], new Vec2(center.X, rectangle.Top), 1, 90, 0);
                        spriteBatch.Rect(style.Sprites[1],
                            new Vec2(center.X, rectangle.Top + style.Sprites[0].Rect.Width),
                            new Vec2((rectangle.Height - style.Sprites[0].Rect.Width * 2) / style.Sprites[0].Rect.Width,
                                1), 90, 0);
                        spriteBatch.Rect(style.Sprites[2], new Vec2(center.X, rectangle.Bottom), 1, 90, 1);
                        if (style.HasBar)
                            spriteBatch.Rect(style.Sprites[3],
                                new Vec2(center.X, rectangle.Top + style.Bar.Start.Value),
                                new Vec2(
                                    (rectangle.Height - style.Bar.End.Value - style.Bar.Start.Value) *
                                    (value / (float) sections) / style.Sprites[0].Rect.Width, 1), 90, 0);
                        if (style.Thumb.HasValue)
                            spriteBatch.Rect(style.Thumb.Value,
                                new Vec2(center.X,
                                    rectangle.Top + style.Bar.Start.Value +
                                    (rectangle.Height - style.Bar.End.Value - style.Bar.Start.Value) *
                                    (value / (float) sections)), 1, 90);
                        break;
                    case Slider.Type.Down2Up:
                        spriteBatch.Rect(style.Sprites[0], new Vec2(center.X, rectangle.Top), 1, 90, 0);
                        spriteBatch.Rect(style.Sprites[1],
                            new Vec2(center.X, rectangle.Top + style.Sprites[0].Rect.Width),
                            new Vec2((rectangle.Height - style.Sprites[0].Rect.Width * 2) / style.Sprites[0].Rect.Width,
                                1), 90, 0);
                        spriteBatch.Rect(style.Sprites[2], new Vec2(center.X, rectangle.Bottom), 1, 90, 1);
                        if (style.HasBar)
                            spriteBatch.Rect(style.Sprites[3], new Vec2(center.X, rectangle.Bottom - style.Bar.End.Value),
                                new Vec2(
                                    (rectangle.Height - style.Bar.End.Value - style.Bar.Start.Value) *
                                    (value / (float) sections) / style.Sprites[0].Rect.Width, 1), -90, 0);
                        if (style.Thumb.HasValue)
                            spriteBatch.Rect(style.Thumb.Value,
                                new Vec2(center.X,
                                    rectangle.Bottom - style.Bar.End.Value -
                                    (rectangle.Height - style.Bar.End.Value - style.Bar.Start.Value) *
                                    (value / (float) sections)), 1, -90);
                        break;
                    case Slider.Type.Left2Right:
                        spriteBatch.Rect(style.Sprites[0], new Vec2(rectangle.Left, center.Y), 1, 0, 0);
                        spriteBatch.Rect(style.Sprites[1],
                            new Vec2(rectangle.Left + style.Sprites[0].Rect.Width, center.Y),
                            new Vec2((rectangle.Width - style.Sprites[0].Rect.Width * 2) / style.Sprites[0].Rect.Width,
                                1), 0, 0);
                        spriteBatch.Rect(style.Sprites[2], new Vec2(rectangle.Right, center.Y), 1, 0, 1);
                        if (style.HasBar)
                            spriteBatch.Rect(style.Sprites[3],
                                new Vec2(rectangle.Left + style.Bar.Start.Value, center.Y),
                                new Vec2(
                                    (rectangle.Width - style.Bar.End.Value - style.Bar.Start.Value) *
                                    (value / (float) sections) / style.Sprites[0].Rect.Width, 1), 0, 0);
                        if (style.Thumb.HasValue)
                            spriteBatch.Rect(style.Thumb.Value,
                                new Vec2(
                                    rectangle.Left + style.Bar.Start.Value +
                                    (rectangle.Width - style.Bar.End.Value - style.Bar.Start.Value) *
                                    (value / (float) sections), center.Y), 1, 0);
                        break;
                    case Slider.Type.Right2Left:
                        spriteBatch.Rect(style.Sprites[0], new Vec2(rectangle.Left, center.Y), 1, 0, 0);
                        spriteBatch.Rect(style.Sprites[1],
                            new Vec2(rectangle.Left + style.Sprites[0].Rect.Width, center.Y),
                            new Vec2((rectangle.Width - style.Sprites[0].Rect.Width * 2) / style.Sprites[0].Rect.Width,
                                1), 0, 0);
                        spriteBatch.Rect(style.Sprites[2], new Vec2(rectangle.Right, center.Y), 1, 0, 1);
                        if (style.HasBar)
                            spriteBatch.Rect(style.Sprites[3],
                                new Vec2(rectangle.Right - style.Bar.End.Value, center.Y),
                                new Vec2(
                                    (rectangle.Width - style.Bar.End.Value - style.Bar.Start.Value) *
                                    (value / (float) sections) / style.Sprites[0].Rect.Width, 1), 180, 0);
                        if (style.Thumb.HasValue)
                            spriteBatch.Rect(style.Thumb.Value,
                                new Vec2(
                                    rectangle.Right - style.Bar.End.Value -
                                    (rectangle.Width - style.Bar.End.Value - style.Bar.Start.Value) *
                                    (value / (float) sections), center.Y), 1, 180);
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
                    int res = 0;
                    switch (type)
                    {
                        case Slider.Type.Up2Down:
                            res = (int) MathF.Round(
                                Math.Clamp(
                                    (Mouse.GUIPosition.Y - (rectangle.Top + style.Bar.Start.Value)) /
                                    (rectangle.Height - style.Bar.End.Value - style.Bar.Start.Value), 0, 1) * sections);
                            break;
                        case Slider.Type.Down2Up:
                            res = (int) MathF.Round(
                                Math.Clamp(
                                    (rectangle.Bottom - style.Bar.End.Value - Mouse.GUIPosition.Y) /
                                    (rectangle.Height - style.Bar.End.Value - style.Bar.Start.Value), 0, 1) * sections);
                            break;
                        case Slider.Type.Left2Right:
                            res = (int) MathF.Round(
                                Math.Clamp(
                                    (Mouse.GUIPosition.X - (rectangle.Left + style.Bar.Start.Value)) /
                                    (rectangle.Width - style.Bar.End.Value - style.Bar.Start.Value), 0, 1) * sections);
                            break;
                        case Slider.Type.Right2Left:
                            res = (int) MathF.Round(
                                Math.Clamp(
                                    (rectangle.Right - style.Bar.End.Value - Mouse.GUIPosition.X) /
                                    (rectangle.Width - style.Bar.End.Value - style.Bar.Start.Value), 0, 1) * sections);
                            break;
                    }

                    set(res);
                }
        }
    }
}