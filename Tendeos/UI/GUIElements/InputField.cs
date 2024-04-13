using Microsoft.Xna.Framework.Graphics;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;

namespace Tendeos.UI.GUIElements
{
    public class InputField : GUIElement
    {
        private readonly Style style;
        private int position;
        private float scroll;

        public InputField(Vec2 anchor, FRectangle rectangle, Style style) : base(anchor, rectangle)
        {
            this.style = style;
        }

        public override void Draw(SpriteBatch spriteBatch, FRectangle rectangle)
        {

        }
        public override void Update(FRectangle rectangle)
        {
            base.Update(rectangle);


        }

        public class Style
        {
            public Sprite[] Rectangle { get; init; }
            public float Height { get; init; }
            public float FontSize { get; init; }

            public Style(Sprite rectangle, float fontSize, float height)
            {
                Rectangle = rectangle.Split(3, 3, 1);
                FontSize = fontSize;
                Height = height;
            }
        }
    }
}
