using System;
using Microsoft.Xna.Framework.Graphics;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;
using Tendeos.Utils.Input;

namespace Tendeos.UI.GUIElements
{
    public class Toggle : GUIElement
    {
        public bool Value { get; private set; }
        protected readonly Style style;
        private readonly Action<bool> changed;

        public Toggle(Vec2 anchor, Vec2 position, Style style) : base(anchor, new FRectangle(position, style.Sprites[0].Rect.Size.ToVector2()))
        {
            this.style = style;
        }

        public Toggle(Vec2 anchor, Vec2 position, Style style, Action<bool> changed) : base(anchor, new FRectangle(position, style.Sprites[0].Rect.Size.ToVector2()))
        {
            this.style = style;
            this.changed = changed;
        }

        public override void Draw(SpriteBatch spriteBatch, FRectangle rectangle)
        {
            spriteBatch.Rect(MouseOn && Mouse.LeftDown ? style.Sprites[2] : Value ? style.Sprites[0] : style.Sprites[1], rectangle.Location, 0, 1, 0, Origin.Zero, Origin.Zero);
        }

        public override void Update(FRectangle rectangle)
        {
            base.Update(rectangle);

            if (MouseOn && Mouse.LeftReleased)
            {
                Value = !Value;
                changed?.Invoke(Value);
            }
        }

        public class Style
        {
            public Sprite[] Sprites { get; }

            public Style() => Sprites = null;

            public Style(Sprite texture)
            {
                Sprites = texture.Split(3, 1, 1);
            }
        }
    }
}
