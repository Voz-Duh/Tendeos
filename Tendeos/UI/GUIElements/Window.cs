using Microsoft.Xna.Framework.Graphics;
using System;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;
using Tendeos.Utils.Input;

namespace Tendeos.UI.GUIElements
{
    public class Window : GUIElement
    {
        protected readonly Icon addativeDraw;
        protected readonly bool dragable;
        protected readonly Style style;

        public Window(Vec2 anchor, FRectangle rectangle, Style style, Icon addativeDraw = null, bool dragable = false) : base(anchor, rectangle)
        {
            this.dragable = dragable;
            this.style = style;
            this.addativeDraw = addativeDraw;
        }

        public override void Update(FRectangle rectangle)
        {
            base.Update(rectangle);

            if (dragable)
            {
                if (style.Label != null)
                {
                    if (MouseOn && Mouse.LeftPressed && rectangle.Y + style.Label[0].Rect.Height > Mouse.GUIPosition.Y) Mouse.OnUpdate += Drag;
                    if (!MouseOn || Mouse.LeftReleased || rectangle.Y + style.Label[0].Rect.Height <= Mouse.GUIPosition.Y) Mouse.OnUpdate -= Drag;
                }
                else
                {
                    if (MouseOn && Mouse.LeftPressed) Mouse.OnUpdate += Drag;
                    if (!MouseOn || Mouse.LeftReleased) Mouse.OnUpdate -= Drag;
                }
            }
        }

        public void Drag()
        {
            rectangle.Location += Mouse.GUIPositionDelta;
        }

        public override void Draw(SpriteBatch spriteBatch, FRectangle rectangle)
        {
            Draw(spriteBatch, rectangle, style);
            if (style.Label != null) addativeDraw?.Invoke(spriteBatch, new FRectangle(0, 0, rectangle.Width, style.Label[0].Rect.Width), this);
            else addativeDraw?.Invoke(spriteBatch, rectangle, this);
        }

        public static void Draw(SpriteBatch spriteBatch, FRectangle rectangle, Style style)
        {
            if (style.Label != null)
            {
                DrawRectWindow(spriteBatch, style.Rectangle, new FRectangle(rectangle.X, rectangle.Y + style.Label[0].Rect.Height, rectangle.Width, rectangle.Height - style.Label[0].Rect.Height));
                spriteBatch.Rect(style.Label[0], rectangle.Location, 0, 1, 0, Origin.Zero, Origin.Zero);
                spriteBatch.Rect(style.Label[1], new FRectangle(rectangle.X + style.Label[0].Rect.Width, rectangle.Y, rectangle.Width - style.Label[0].Rect.Width * 2, style.Label[0].Rect.Height));
                spriteBatch.Rect(style.Label[2], new Vec2(rectangle.Right, rectangle.Y), 0, 1, 0, Origin.One, Origin.Zero);
            }
            else
            {
                DrawRectWindow(spriteBatch, style.Rectangle, rectangle);
            }
        }

        public class Style
        {
            public Sprite[] Rectangle { get; }
            public Sprite[] Label { get; }

            public Style(Sprite rectangle, Sprite? label = null)
            {
                Rectangle = rectangle.Split(3, 3, 1);
                Label = label?.Split(3, 1, 1) ?? null;
            }
        }
    }
}
