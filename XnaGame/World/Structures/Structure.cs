using Va;
using XnaGame.Coding;
using XnaGame.Content;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace XnaGame.World.Structures
{
    public class Structure
    {
        private static (ITile, ITile)[][] tiles;
        private static readonly CompileStyle main = null;

        static Structure()
        {
            Solution mainSolution = new Solution();

            main = new CompileStyle(
                new(
                    new TokenStyle[]
                    {
                        new TokenStyle(TokenType.Keyword),
                        new TokenStyle(":", TokenType.Special),
                        new TokenStyle(TokenType.Keyword),
                        new TokenStyle(TokenType.Keyword)
                    },
                    (CompileStyleDelegate)
                    ((sln, toks) =>
                    {
                        if (!sln.TryAdd(toks[0].Text, new DataStruct(VValueType.String, toks[2].data)))
                            throw new VaException(toks[1].line, $"Already have same variable.");
                        else
                            sln.TryAdd($"{toks[0].Text}0", new DataStruct(VValueType.String, toks[3].data));
                    })
                ),
                new(
                    new TokenStyle[]
                    {
                        new TokenStyle(TokenType.BoxBracket)
                    },
                    (CompileStyleDelegate)
                    ((sln, toks) =>
                    {
                        Token[][] rows = toks[0].tokens[0].Split(new TokenStyle(",", TokenType.Special));
                        tiles = new (ITile, ITile)[rows.Length][];
                        for (int i = 0; i < rows.Length; i++)
                        {
                            Token[][] cols = rows[i][0].tokens[0].Split(new TokenStyle(",", TokenType.Special));
                            tiles[i] = new (ITile, ITile)[cols.Length];
                            for (int j = 0; j < cols.Length; j++)
                            {
                                if (sln.TryGet(cols[j][0].Text, out DataStruct data))
                                {
                                    tiles[i][j] = (
                                        Tiles.Get<ITile>(data.Str)(),
                                        Tiles.Get<ITile>(sln[$"{cols[j][0].Text}0"].Str)()
                                    );
                                }
                                else
                                {
                                    tiles[i][j] = (
                                        Tiles.Get<ITile>(cols[j][0].Text)(),
                                        Tiles.Get<ITile>(cols[j][1].Text)()
                                    );
                                }
                            }
                        }
                    })
                )
            );
        }

        private readonly (ITile w, ITile t)[][] data;

        public Structure(string code)
        {
            Token[][] tokens = Compiler.GetTokens(code);
            Compiler.ParseStyle(new Solution(), main, tokens);
            data = tiles;
        }

        public void Spawn(Map map, int x, int y)
        {
            for (int i = 0; i < data.Length; i++)
                for (int j = 0; j < data[i].Length; j++)
                {
                    map.SetTile(false, data[i][j].w, x+j, y+i);
                    map.SetTile(true, data[i][j].t, x+j, y+i);
                }
        }
    }
}
