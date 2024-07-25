using System;
using Tendeos.Utils.Graphics;
using Tendeos.Utils;
using Tendeos.Utils.Input;

namespace Tendeos.UI.GUIElements
{
    public class SwitchButtons : GUIElement
    {
        protected int selected = -1;
        public readonly Button.Style style;

        protected readonly (Action close, Action<SpriteBatch, FRectangle> off, Action open,
            Action<SpriteBatch, FRectangle> on)[] buttons;

        public SwitchButtons(Vec2 anchor, FRectangle buttonRectangle, Button.Style style, GUIElement[] childs = null,
            params (Action close, Action<SpriteBatch, FRectangle> off, Action open, Action<SpriteBatch, FRectangle> on)
                [] buttons) : base(anchor,
            new FRectangle(buttonRectangle.X, buttonRectangle.Y, buttonRectangle.Width,
                buttonRectangle.Height * buttons.Length), childs)
        {
            this.style = style;
            this.buttons = buttons;
        }

        public override void Draw(SpriteBatch spriteBatch, FRectangle rectangle)
        {
            float height = rectangle.Height / buttons.Length;
            FRectangle button = new FRectangle(rectangle.X, rectangle.Y, rectangle.Width, height);

            bool has = false;
            for (int i = 0; i < buttons.Length; i++)
            {
                Sprite[] texture = !has && (has = MouseOn && Mouse.GUIPosition.Y <= button.Bottom)
                    ? (Mouse.LeftDown ? style.Down : style.On)
                    : style.Idle;
                DrawRectWindow(spriteBatch, texture, button);

                (i == selected ? buttons[i].on : buttons[i].off)?.Invoke(spriteBatch, button);
                button.Location += new Vec2(0, height);
            }
        }

        public override void Update(FRectangle rectangle)
        {
            base.Update(rectangle);

            if (MouseOn)
            {
                float height = rectangle.Height / buttons.Length;
                FRectangle button = new FRectangle(rectangle.X, rectangle.X + rectangle.Height - height,
                    rectangle.Width, height);
                for (int i = buttons.Length - 1; i >= 0; i--)
                    if (Mouse.GUIPosition.Y >= button.Y && Mouse.LeftReleased)
                    {
                        if (selected != -1) buttons[selected].close();
                        if (selected == i) selected = -1;
                        else
                        {
                            buttons[i].open();
                            selected = i;
                        }

                        break;
                    }
                    else button.Location -= new Vec2(0, height);
            }
        }
    }
}