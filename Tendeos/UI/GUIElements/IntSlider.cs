using Microsoft.Xna.Framework.Graphics;
using System;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;
using Tendeos.Utils.Input;

namespace Tendeos.UI.GUIElements
{
    public class IntSlider : GUIElement
    {
        public int sections;

        protected readonly Slider.Type type;
        protected readonly Slider.Style style;
        protected readonly Func<int> get;
        protected readonly Action<int> set;

        public IntSlider(Vec2 anchor, Slider.Type type, float offset, float start, float end, Slider.Style style, int sections, Func<int> get, Action<int> set) : base(anchor, type switch {
            Slider.Type.Up2Down => new FRectangle(offset, start, style.Sprites[0].Height, end - start),
            Slider.Type.Down2Up => new FRectangle(offset, start, style.Sprites[0].Height, end - start),
            Slider.Type.Left2Right => new FRectangle(start, offset, end - start, style.Sprites[0].Height),
            Slider.Type.Right2Left => new FRectangle(start, offset, end - start, style.Sprites[0].Height),
        })
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
            int value = get();
            switch (type)
            {
                case Slider.Type.Up2Down:
                    spriteBatch.Rect(style.Sprites[0], new Vec2(center.X, rectangle.Bottom), 90, 1, 0, Origin.Zero);
                    spriteBatch.Rect(style.Sprites[1], new Vec2(center.X, rectangle.Bottom + style.Sprites[0].Rect.Width), new Vec2((rectangle.Height - style.Sprites[0].Rect.Width*2) / style.Sprites[0].Rect.Width, 1), 90, 0, Origin.Zero);
                    spriteBatch.Rect(style.Sprites[2], new Vec2(center.X, rectangle.Top), 90, 1, 0, Origin.One);
                    if (style.HasBar) spriteBatch.Rect(style.Sprites[3], new Vec2(center.X, rectangle.Bottom + style.Bar.Start.Value), new Vec2((rectangle.Height - style.Bar.End.Value - style.Bar.Start.Value) * (value / (float)sections) / style.Sprites[0].Rect.Width, 1), 90, 0, Origin.Zero);
                    if (style.Thumb.HasValue) spriteBatch.Rect(style.Thumb.Value, new Vec2(center.X, rectangle.Bottom + style.Bar.Start.Value + (rectangle.Height - style.Bar.End.Value - style.Bar.Start.Value) * (value / (float)sections)), 90);
                    break;
                case Slider.Type.Down2Up:
                    spriteBatch.Rect(style.Sprites[0], new Vec2(center.X, rectangle.Bottom), 90, 1, 0, Origin.Zero);
                    spriteBatch.Rect(style.Sprites[1], new Vec2(center.X, rectangle.Bottom + style.Sprites[0].Rect.Width), new Vec2((rectangle.Height - style.Sprites[0].Rect.Width * 2) / style.Sprites[0].Rect.Width, 1), 90, 0, Origin.Zero);
                    spriteBatch.Rect(style.Sprites[2], new Vec2(center.X, rectangle.Top), 90, 1, 0, Origin.One);
                    if (style.HasBar) spriteBatch.Rect(style.Sprites[3], new Vec2(center.X, rectangle.Top - style.Bar.End.Value), new Vec2((rectangle.Height - style.Bar.End.Value - style.Bar.Start.Value) * (value / (float)sections) / style.Sprites[0].Rect.Width, 1), -90, 0, Origin.Zero);
                    if (style.Thumb.HasValue) spriteBatch.Rect(style.Thumb.Value, new Vec2(center.X, rectangle.Top - style.Bar.End.Value - (rectangle.Height - style.Bar.End.Value - style.Bar.Start.Value) * (value / (float)sections)), -90);
                    break;
                case Slider.Type.Left2Right:
                    spriteBatch.Rect(style.Sprites[0], new Vec2(rectangle.Left, center.Y), 0, 1, 0, Origin.Zero);
                    spriteBatch.Rect(style.Sprites[1], new Vec2(rectangle.Left + style.Sprites[0].Rect.Width, center.Y), new Vec2((rectangle.Width - style.Sprites[0].Rect.Width * 2) / style.Sprites[0].Rect.Width, 1), 0, 0, Origin.Zero);
                    spriteBatch.Rect(style.Sprites[2], new Vec2(rectangle.Right, center.Y), 0, 1, 0, Origin.One);
                    if (style.HasBar) spriteBatch.Rect(style.Sprites[3], new Vec2(rectangle.Left + style.Bar.Start.Value, center.Y), new Vec2((rectangle.Width - style.Bar.End.Value - style.Bar.Start.Value) * (value / (float)sections) / style.Sprites[0].Rect.Width, 1), 0, 0, Origin.Zero);
                    if (style.Thumb.HasValue) spriteBatch.Rect(style.Thumb.Value, new Vec2(rectangle.Left + style.Bar.Start.Value + (rectangle.Width - style.Bar.End.Value - style.Bar.Start.Value) * (value / (float)sections), center.Y), 0);
                    break;
                case Slider.Type.Right2Left:
                    spriteBatch.Rect(style.Sprites[0], new Vec2(rectangle.Left, center.Y), 0, 1, 0, Origin.Zero);
                    spriteBatch.Rect(style.Sprites[1], new Vec2(rectangle.Left + style.Sprites[0].Rect.Width, center.Y), new Vec2((rectangle.Width - style.Sprites[0].Rect.Width * 2) / style.Sprites[0].Rect.Width, 1), 0, 0, Origin.Zero);
                    spriteBatch.Rect(style.Sprites[2], new Vec2(rectangle.Right, center.Y), 0, 1, 0, Origin.One);
                    if (style.HasBar) spriteBatch.Rect(style.Sprites[3], new Vec2(rectangle.Right - style.Bar.End.Value, center.Y), new Vec2((rectangle.Width - style.Bar.End.Value - style.Bar.Start.Value) * (value / (float)sections) / style.Sprites[0].Rect.Width, 1), 180, 0, Origin.Zero);
                    if (style.Thumb.HasValue) spriteBatch.Rect(style.Thumb.Value, new Vec2(rectangle.Right - style.Bar.End.Value - (rectangle.Width - style.Bar.End.Value - style.Bar.Start.Value) * (value / (float)sections), center.Y), 180);
                    break;
            }
        }

        public override void Update(FRectangle rectangle)
        {
            base.Update(rectangle);

            if (MouseOn && Mouse.LeftDown)
                switch (type)
                {
                    case Slider.Type.Up2Down:
                        set((int)MathF.Round(Math.Clamp((Mouse.GUIPosition.Y - (rectangle.Bottom + style.Bar.Start.Value)) / (rectangle.Height - style.Bar.End.Value - style.Bar.Start.Value), 0, 1) * sections));
                        break;
                    case Slider.Type.Down2Up:
                        set((int)MathF.Round(Math.Clamp((rectangle.Top - style.Bar.End.Value - Mouse.GUIPosition.Y) / (rectangle.Height - style.Bar.End.Value - style.Bar.Start.Value), 0, 1) * sections));
                        break;
                    case Slider.Type.Left2Right:
                        set((int)MathF.Round(Math.Clamp((Mouse.GUIPosition.X - (rectangle.Left + style.Bar.Start.Value)) / (rectangle.Width - style.Bar.End.Value - style.Bar.Start.Value), 0, 1) * sections));
                        break;
                    case Slider.Type.Right2Left:
                        set((int)MathF.Round(Math.Clamp((rectangle.Right - style.Bar.End.Value - Mouse.GUIPosition.X) / (rectangle.Width - style.Bar.End.Value - style.Bar.Start.Value), 0, 1) * sections));
                        break;
                }
        }
    }
}
