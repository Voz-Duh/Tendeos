using System;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;
using Tendeos.Utils.Input;

namespace Tendeos.UI.GUIElements
{
    public class Toggle : GUIElement
    {
        public bool Value { get; private set; }
        public readonly Style style;
        protected readonly Action<bool> changed;

        public Toggle(Vec2 anchor, Vec2 position, Style style, bool startValue = false, GUIElement[] childs = null) : base(anchor,
            new FRectangle(position, style.Sprites[0].Rect.Size.ToVector2()), childs)
        {
            this.style = style;
            Value = startValue;
        }

        public Toggle(Vec2 anchor, Vec2 position, Style style, Action<bool> changed, bool startValue = false, GUIElement[] childs = null) : base(anchor,
            new FRectangle(position, style.Sprites[0].Rect.Size.ToVector2()), childs)
        {
            this.style = style;
            this.changed = changed;
            Value = startValue;
        }

        public override void Draw(SpriteBatch spriteBatch, FRectangle rectangle)
        {
            spriteBatch.Rect(MouseOn && Mouse.LeftDown ? style.Sprites[2] : Value ? style.Sprites[0] : style.Sprites[1],
                rectangle.Location, 1, 0, 0, 0);
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