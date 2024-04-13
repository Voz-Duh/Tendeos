using Microsoft.Xna.Framework.Graphics;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;

namespace Tendeos.UI.GUIElements
{
    public class Image : GUIElement
    {
        public readonly Sprite style;

        public Image(Vec2 anchor, Vec2 offset, Sprite style) : base(anchor, new FRectangle(offset, style.Rect.Size.ToVector2()))
        {
            this.style = style;
        }

        public override void Draw(SpriteBatch spriteBatch, FRectangle rectangle)
        {
            spriteBatch.Rect(style, rectangle.Center);
        }
    }
}
