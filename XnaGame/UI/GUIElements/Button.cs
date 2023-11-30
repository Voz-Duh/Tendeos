using System;
using XnaGame.Utils;
using XnaGame.Utils.Graphics;
using XnaGame.Utils.Input;

namespace XnaGame.UI.GUIElements
{
    public class Button : GUIElement
    {
        private readonly Style style;
        private readonly Action<FRectangle> icon;
        private readonly Action action;

        public Button(GUIElement GUI, FVector2 anchor, FRectangle rectangle, Action action, Style style, Sprite icon) : base(GUI, anchor)
        {
            this.rectangle = rectangle;
            this.style = style;
            this.icon = (rectangle) => SDraw.Rect(icon, rectangle.Center);
            this.action = action;
        }

        public Button(GUIElement GUI, FVector2 anchor, FRectangle rectangle, Action action, Style style, Action<FRectangle> icon) : base(GUI, anchor)
        {
            this.rectangle = rectangle;
            this.style = style;
            this.icon = icon;
            this.action = action;
        }

        public override void Draw(FRectangle rectangle)
        {
            Sprite[] texture = MouseOn ? Mouse.LeftDown ? style.Down : style.On : style.Idle;

            DrawRectWindow(texture, rectangle);

            icon(rectangle);

            base.Draw(rectangle);
        }

        public override void Update(FRectangle rectangle)
        {
            base.Update(rectangle);

            if (MouseOn && Mouse.LeftReleased)
                action();
        }

        public class Style
        {
            public Sprite[] Idle { get; init; }
            public Sprite[] On { get; init; }
            public Sprite[] Down { get; init; }

            public Style(Sprite texture)
            {
                Sprite[] textures = texture.Split(3, 1, 1);
                Sprite idle = textures[0], on = textures[1], down = textures[2];
                Idle = idle.Split(3, 3, 1);
                Down = down.Split(3, 3, 1);
                On = on.Split(3, 3, 1);
            }
        }
    }
}
