using Microsoft.Xna.Framework.Graphics;
using System;
using XnaGame.Utils.Graphics;
using XnaGame.Utils;
using XnaGame.Utils.Input;

namespace XnaGame.UI.GUIElements
{
    public class SwitchButtons : GUIElement
    {
        private int selected = -1;
        private readonly Button.Style style;
        private readonly (Action close, Action<SpriteBatch, FRectangle> off, Action open, Action<SpriteBatch, FRectangle> on)[] buttons;

        public SwitchButtons(Vec2 anchor, FRectangle buttonRectangle, Button.Style style, params (Action close, Action<SpriteBatch, FRectangle> off, Action open, Action<SpriteBatch, FRectangle> on)[] buttons) : base(anchor, new FRectangle(buttonRectangle.X, buttonRectangle.Y, buttonRectangle.Width, buttonRectangle.Height * buttons.Length))
        {
            this.style = style;
            this.buttons = buttons;
        }

        public override void Draw(SpriteBatch spriteBatch, FRectangle rectangle)
        {
            float height = rectangle.Height/buttons.Length;
            FRectangle button = new FRectangle(rectangle.X, rectangle.Y, rectangle.Width, height);
            for (int i = 0; i < buttons.Length; i++)
            {
                Sprite[] texture = MouseOn && Mouse.GUIPosition.Y >= button.Y ? Mouse.LeftDown ? style.Down : style.On : style.Idle;
                button.Location += new Vec2(0, height);
                DrawRectWindow(spriteBatch, texture, rectangle);

                (i == selected ? buttons[i].on : buttons[i].off)?.Invoke(spriteBatch, rectangle);
            }

            base.Draw(spriteBatch, rectangle);
        }

        public override void Update(FRectangle rectangle)
        {
            base.Update(rectangle);

            float height = rectangle.Height/buttons.Length;
            FRectangle button = new FRectangle(rectangle.X, rectangle.X + rectangle.Height - height, rectangle.Width, height);
            for (int i = buttons.Length - 1; i >= 0; i--)
                if (MouseOn && Mouse.GUIPosition.Y >= button.Y && Mouse.LeftReleased)
                {
                    if (selected != -1) buttons[selected].close();
                    if (selected == i) selected = -1;
                    else
                    {
                        buttons[i].open();
                        selected = i;
                    }
                    button.Location -= new Vec2(0, height);
                    break;
                }
        }
    }
}
