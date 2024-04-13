using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Va;
using Tendeos.Utils.Graphics;

namespace Tendeos.Utils
{
    public static class ContentHelpers
    {
        public static byte[] LoadFileBytes(this ContentManager content, string filePath)
        {
            string fullPath = Path.Combine(content.RootDirectory, filePath);
            return File.ReadAllBytes(fullPath);
        }

        public static string LoadFileText(this ContentManager content, string filePath)
        {
            string fullPath = Path.Combine(content.RootDirectory, filePath);
            return File.ReadAllText(fullPath);
        }

        public static void LoadSpriteData(this ContentManager content, string filePath, Dictionary<string, Sprite> to)
        {
            string fullPath = Path.Combine(content.RootDirectory, $"{filePath}.sd");
            Texture2D texture = content.Load<Texture2D>(filePath);
            Compiler.ParseStyle(new Solution(), new CompileStyle(
                (
                    new TokenStyle[]
                    {
                        new TokenStyle(TokenType.Keyword),
                        new TokenStyle(":", TokenType.Special),
                        new TokenStyle(TokenType.Number),
                        new TokenStyle(",", TokenType.Special),
                        new TokenStyle(TokenType.Number),
                        new TokenStyle(",", TokenType.Special),
                        new TokenStyle(TokenType.Number),
                        new TokenStyle(",", TokenType.Special),
                        new TokenStyle(TokenType.Number)
                    },
                    (CompileStyleDelegate)
                    ((sln, toks) =>
                    {
                        if (!to.TryAdd(toks[0].Text, new Sprite(texture, new Rectangle((int)BitConverter.ToDouble(toks[2].data), (int)BitConverter.ToDouble(toks[4].data), (int)BitConverter.ToDouble(toks[6].data), (int)BitConverter.ToDouble(toks[8].data)))))
                            throw new VaException(toks[1].line, $"Already have \"{toks[0].Text}\" key.");
                    })
                )
            ), Compiler.GetTokens(File.ReadAllText(fullPath)));
        }
    }
}
