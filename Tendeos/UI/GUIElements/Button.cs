using Microsoft.Xna.Framework.Graphics;
using System;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;
using Tendeos.Utils.Input;

namespace Tendeos.UI.GUIElements
{
    public class Button : GUIElement
    {
        protected readonly Style style;
        protected Icon icon;
        protected Action action;

        public Button(Vec2 anchor, FRectangle rectangle, Action action, Style style, Sprite icon) : base(anchor, rectangle)
        {
            this.style = style;
            this.icon = Icon.From((spriteBatch, rectangle) => spriteBatch.Rect(icon, rectangle.Center));
            this.action = action;
        }

        public Button(Vec2 anchor, FRectangle rectangle, Action action, Style style, Icon icon) : base(anchor, rectangle)
        {
            this.style = style;
            this.icon = icon;
            this.action = action;
        }

        public override void Draw(SpriteBatch spriteBatch, FRectangle rectangle)
        {
            if (style.Idle != null)
            {
                DrawRectWindow(spriteBatch, MouseOn ? Mouse.LeftDown ? style.Down : style.On : style.Idle, rectangle);
            }
            icon?.Invoke(spriteBatch, rectangle, this);
        }

        public override void Update(FRectangle rectangle)
        {
            base.Update(rectangle);

            if (MouseOn && Mouse.LeftReleased)
                action();
        }

        public class Style
        {
            public Sprite[] Idle { get; }
            public Sprite[] On { get; }
            public Sprite[] Down { get; }

            public Style() => Idle = On = Down = null;

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
