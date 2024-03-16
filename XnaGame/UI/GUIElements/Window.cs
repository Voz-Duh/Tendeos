using Microsoft.Xna.Framework.Graphics;
using XnaGame.Utils;
using XnaGame.Utils.Graphics;
using XnaGame.Utils.Input;

namespace XnaGame.UI.GUIElements
{
    public class Window : GUIElement
    {
        private readonly bool dragable;
        private readonly Style style;

        public Window(Vec2 anchor, FRectangle rectangle, Style style, bool dragable = false) : base(anchor, rectangle)
        {
            this.dragable = dragable;
            this.style = style;
        }

        public override void Update(FRectangle rectangle)
        {
            base.Update(rectangle);

            if (dragable)
            {
                if (MouseOn && Mouse.LeftPressed) Mouse.OnUpdate += Drag;
                if (!MouseOn || Mouse.LeftReleased) Mouse.OnUpdate -= Drag;
            }
        }

        public void Drag()
        {
            rectangle.Location += Mouse.GUIPositionDelta;
        }

        public override void Draw(SpriteBatch spriteBatch, FRectangle rectangle)
        {
            DrawRectWindow(spriteBatch, style.Rectangle, rectangle);

            base.Draw(spriteBatch, rectangle);
        }

        public class Style
        {
            public Sprite[] Rectangle { get; init; }

            public Style(Sprite rectangle)
            {
                Rectangle = rectangle.Split(3, 3, 1);
            }
        }
    }
}
