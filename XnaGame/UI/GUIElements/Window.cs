using XnaGame.Utils;
using XnaGame.Utils.Graphics;

namespace XnaGame.UI.GUIElements
{
    public class Window : GUIElement
    {
        private readonly Style style;

        public Window(GUIElement GUI, FVector2 anchor, FRectangle rectangle, Style style) : base(GUI, anchor)
        {
            this.rectangle = rectangle;
            this.style = style;
        }

        public override void Draw(FRectangle rectangle)
        {
            DrawRectWindow(style.Rectangle, rectangle);

            base.Draw(rectangle);
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
