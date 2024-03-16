using Microsoft.Xna.Framework.Content;
using SpriteFontPlus;
using Va;
using XnaGame.Utils.SaveSystem;

namespace XnaGame.Utils.Graphics
{
    public class DynamicSpriteFontScaled
    {
        public DynamicSpriteFont Dynamic { get; private set; }
        public float Scale { get; init; }

        public DynamicSpriteFontScaled(ContentManager content, string[] files, float defaultSize, float scale, int textureWidth = 1024, int textureHeight = 1024)
        {
            string lng = Settings.GetString("language");

            for (int i = 0; i < files.Length; i++)
            {
                Compiler.ParseStyle(new Solution(), new CompileStyle(
                (
                    new TokenStyle[] { new TokenStyle(TokenType.Keyword) },
                    (CompileStyleDelegate)
                    ((sln, toks) =>
                    {
                        if (toks[0].Text == lng)
                            Dynamic = DynamicSpriteFont.FromTtf(content.LoadFileBytes(files[i]), defaultSize, textureWidth, textureHeight);
                    })
                )), Compiler.GetTokens(content.LoadFileText($"{files[i]}.lng")));
            }
            Scale = scale;
        }
    }
}