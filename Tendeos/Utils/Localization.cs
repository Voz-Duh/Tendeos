using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using Va;
using Tendeos.Utils.SaveSystem;
using System.Text.RegularExpressions;

namespace Tendeos.Utils
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

        private static readonly Regex regex = new Regex("\\<(.*)\\>");

        public static string Translate(this string key, params object[] args) => data.TryGetValue(key, out string value) ? string.Format(value, args) : key;
        public static string WithTranslates(this string text, params object[] args)
        {
            string result = text;
            MatchCollection matches = regex.Matches(text);
            for (int i = matches.Count - 1; i >= 0; i--)
            {
                Match match = matches[i];
                Group group0 = match.Groups[0];
                Group group1 = match.Groups[1];
                if (data.TryGetValue(result.Substring(group1.Index, group1.Length), out string value))
                {
                    result = result.Remove(group0.Index, group0.Length).Insert(group0.Index, value);
                }
            }
            return string.Format(result, args);
        }
    }
}
