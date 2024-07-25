using Tendeos.Utils;
using Tendeos.Utils.Graphics;
using Tendeos.Utils.Input;

namespace Tendeos.UI.GUIElements
{
    public class WindowFiller : GUIElement
    {
        protected readonly Camera camera;
        public readonly Sprite style;
        protected readonly bool closeOnClick;

        public WindowFiller(Camera camera, Sprite style = default, bool closeOnClick = false, GUIElement[] childs = null) : base(Vec2.Zero,
            FRectangle.Zero, childs)
        {
            this.camera = camera;
            this.style = style;
            this.closeOnClick = closeOnClick;
        }

        public override void Update(FRectangle rectangle)
        {
            Mouse.OnGUI = MouseOn = true;
            if (closeOnClick && Mouse.LeftReleased)
                Parent.Remove(this);
        }

        public override void Draw(SpriteBatch spriteBatch, FRectangle rectangle)
        {
            Rectangle = new FRectangle(Vec2.Zero, camera.WorldViewport);
            if (style.Texture != null)
            {
                spriteBatch.Rect(style, this.Rectangle);
            }
        }
    }
}