using Microsoft.Xna.Framework.Graphics;
using XnaGame.Utils;
using XnaGame.Utils.Graphics;

namespace XnaGame.UI.GUIElements
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

            base.Draw(spriteBatch, rectangle);
        }
    }
}
