using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using Va;
using XnaGame.Utils.SaveSystem;

namespace XnaGame.Utils
{
    public static class Localization
    {
        private static readonly Dictionary<string, string> data = new Dictionary<string, string>();

        public static void Load(ContentManager content)
        {
            data.Clear();
            Compiler.ParseStyle(new Solution(), new CompileStyle(
                (
                    new TokenStyle[]
                    {
                        new TokenStyle(TokenType.Keyword),
                        new TokenStyle(":", TokenType.Special),
                        new TokenStyle(TokenType.String)
                    },
                    (CompileStyleDelegate)
                    ((sln, toks) =>
                    {
                        if (!data.TryAdd(toks[0].Text, toks[2].Text))
                            throw new VaException(toks[1].line, $"Already have \"{toks[0].Text}\" key.");
                    })
                )
            ), Compiler.GetTokens(content.LoadFileText($"languages/{Settings.GetString("language")}.lng")));
        }

        public static string Get(string key, params object[] args) => data.TryGetValue(key, out string value) ? string.Format(value, args) : key;
    }
}
