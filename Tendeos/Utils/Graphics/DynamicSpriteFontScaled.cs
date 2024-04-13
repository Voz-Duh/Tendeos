using FontStashSharp;
using Microsoft.Xna.Framework.Content;
using Va;
using Tendeos.Utils.SaveSystem;

namespace Tendeos.Utils.Graphics
{
    public class DynamicSpriteFontScaled
    {
        public DynamicSpriteFont Dynamic { get; private set; }
        public float Scale { get; init; }

        public DynamicSpriteFontScaled(ContentManager content, string[] files, float defaultSize, float scale)
        {
            string lng = Settings.GetString("language");
            var settings = new FontSystemSettings();
            var fontSystem = new FontSystem(settings)
            {
                UseKernings = true
            };

            for (int i = 0; i < files.Length; i++)
            {
                Compiler.ParseStyle(new Solution(), new CompileStyle(
                (
                    new TokenStyle[] { new TokenStyle(TokenType.Keyword) },
                    (CompileStyleDelegate)
                    ((sln, toks) =>
                    {
                        if (toks[0].Text == lng)
                            fontSystem.AddFont(content.LoadFileBytes(files[i]));
                    })
                )), Compiler.GetTokens(content.LoadFileText($"{files[i]}.lng")));
            }

            Dynamic = fontSystem.GetFont(defaultSize);
            Scale = scale;
        }
    }
}