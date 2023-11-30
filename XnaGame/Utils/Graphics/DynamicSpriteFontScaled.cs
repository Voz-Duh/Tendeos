using SpriteFontPlus;

namespace XnaGame.Utils.Graphics
{
    public class DynamicSpriteFontScaled
    {
        public readonly DynamicSpriteFont dynamic;
        public readonly float scale;

        public DynamicSpriteFontScaled(byte[] ttf, float defaultSize, float scale, int textureWidth = 1024, int textureHeight = 1024)
        {
            dynamic = DynamicSpriteFont.FromTtf(ttf, defaultSize, textureWidth, textureHeight);
            this.scale = scale;
        }
    }
}