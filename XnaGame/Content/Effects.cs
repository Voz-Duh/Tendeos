using Microsoft.Xna.Framework.Content;
using XnaGame.Utils;
using XnaGame.Utils.Graphics;

namespace XnaGame.Content
{
    public static class Effects
    {
        public static Effect slashMedium;

        public static void Init(ContentManager content)
        {
            //Sprite slashSmallSprite = Sprite.Load(content, "slash_small");

            slashMedium = new Effect();
            slashMedium.SetDraw(Sprite.Load(content, "effects/slash_medium").Split(3, 1, 1), 0.15f, true);
            slashMedium.SetEmits(1);
            slashMedium.SetSpeed(50);
        }

        public static Effect Get(string value) => (Effect)typeof(Effects).GetField(value).GetValue(null);
    }
}
