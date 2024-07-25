using Tendeos.Utils.Graphics;
using Tendeos.Utils;

namespace Tendeos.UI.GUIElements
{
    public class TextLabel : GUIElement
    {
        public readonly Font font;
        public readonly float scale;

        public string text;

        public TextLabel(Vec2 anchor, FRectangle rectangle, string text, Font font, float scale = 1, GUIElement[] childs = null) : base(anchor,
            rectangle, childs)
        {
            this.text = text;
            this.font = font;
            this.scale = scale;
        }

        public override void Draw(SpriteBatch spriteBatch, FRectangle rectangle)
        {
            spriteBatch.Text(font, text, rectangle.Center, scale);
        }
    }
}