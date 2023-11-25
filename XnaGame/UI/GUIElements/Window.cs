using XnaGame.Utils;

namespace XnaGame.UI.GUIElements
{
    public class Window : GUIElement
    {
        private readonly Style style;

        public Window(GUIElement GUI, FRectangle rectangle, Style style) : base(GUI)
        {
            this.rectangle = rectangle;
            this.style = style;
        }

        public override void Draw(FRectangle rectangle)
        {
            DrawRectWindow(style.Rectangle, rectangle);

            base.Draw(rectangle);
        }

        public override void Update(FRectangle point)
        {
            base.Update(point);
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
