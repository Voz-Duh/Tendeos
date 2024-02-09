using System;
using System.Text;
using Va;

namespace XnaGame.Coding
{
    public class CodedObject
    {
        ExpressionStyle expression = null;
        ValueStyle valueStyle = null;

        Solution mainSolution;

        CompileStyle inCodeRegion = null, main = null;

        public CodedObject(string code)
        {
            mainSolution = new Solution();

            valueStyle = new(
                (
                    new TokenStyle[]
                    {
                        new TokenStyle(TokenType.String)
                    },
                    (sln, toks) => new DataStruct(VValueType.String, toks[0].data)
                ),
                (
                    new TokenStyle[]
                    {
                        new TokenStyle(TokenType.Number)
                    },
                    (sln, toks) => new DataStruct(VValueType.Number, toks[0].data)
                ),
                (
                    new TokenStyle[]
                    {
                        new TokenStyle(TokenType.Keyword)
                    },
                    (sln, toks) => {
                        if (toks[0].data.Equals("false"))
                            return new DataStruct(VValueType.Bool, new byte[] { 0b0 });
                        else if (toks[0].data.Equals("true"))
                            return new DataStruct(VValueType.Bool, new byte[] { 0b1 });
                        return sln[toks[0].Text];
                    }
                ),
                (
                    new TokenStyle[]
                    {
                        new TokenStyle(TokenType.Keyword),
                        new TokenStyle(TokenType.Parenthese),
                    },
                    (sln, toks) => {
                        Solution solution = new Solution(mainSolution);

                        FunctionKey key = new FunctionKey(toks[0].Text,
                            toks[1].tokens.Length == 1 && toks[1].tokens[0].Length == 0
                            ? 0 : toks[1].tokens.Length);

                        Function function = sln[key];

                        for (int i = 0; i < key.arguments; i++)
                            solution.Add(function.args[i], expression.Parse(sln, toks[1].tokens[i]));
                        Compiler.ParseStyle(solution, inCodeRegion, function.code);
                        if (solution.breaked && solution.toReturn.HasValue)
                            return solution.toReturn.Value;
                        throw new VaException(toks[0].line, "Function doesn't have return value.");
                    }
                )
            );

            expression = new(
                valueStyle,
                new (TokenStyle, ExpressionStyleDelegate)[]
                {
                    (new TokenStyle("+", TokenType.Special),
                    (sln, line, a, b) =>
                    {
                        if (a.Is(VValueType.String))
                        {
                            if (b.Is(VValueType.String))
                                return new DataStruct(VValueType.String, a.data, b.data);
                            else if (b.Is(VValueType.Number))
                                return new DataStruct(VValueType.String, a.data, Encoding.Unicode.GetBytes($"{BitConverter.ToDouble(b.data)}"));
                            else if (b.Is(VValueType.Bool))
                                return new DataStruct(VValueType.String, a.data, Encoding.Unicode.GetBytes(b.data[0] == 0b1 ? "true" : "false"));
                        }
                        else if (a.Is(VValueType.Number))
                        {
                            if (b.Is(VValueType.String))
                                return new DataStruct(VValueType.String, Encoding.Unicode.GetBytes($"{BitConverter.ToDouble(a.data)}"), b.data);
                            else if (b.Is(VValueType.Number))
                                return new DataStruct(VValueType.Number, BitConverter.GetBytes(BitConverter.ToDouble(a.data) + BitConverter.ToDouble(b.data)));
                        }
                        else if (a.Is(VValueType.Bool))
                        {
                            if (b.Is(VValueType.String))
                                return new DataStruct(VValueType.String, Encoding.Unicode.GetBytes(b.data[0] == 0b1 ? "true" : "false"), b.data);
                        }
                        throw new VaException(line, $"Not supports + operation between {a.type} and {b.type}.");
                    }),
                    (new TokenStyle("-", TokenType.Special),
                    (sln, line, a, b) =>
                    {
                        if (a.Is(VValueType.Number))
                        {
                            if (b.Is(VValueType.Number))
                                return new DataStruct(VValueType.Number, BitConverter.GetBytes(BitConverter.ToDouble(a.data) - BitConverter.ToDouble(b.data)));
                        }
                        throw new VaException(line, $"Not supports - operation between {a.type} and {b.type}.");
                    })
                },
                new (TokenStyle, ExpressionStyleDelegate)[]
                {
                    (new TokenStyle("*", TokenType.Special),
                    (sln, line, a, b) =>
                    {
                        if (a.Is(VValueType.String))
                        {
                            if (b.Is(VValueType.Number))
                            {
                                int i = (int)BitConverter.ToDouble(b.data);
                                DataStruct ptr = new DataStruct(VValueType.String, new byte[a.data.Length*i]);
                                for (int j = 0; j < i; j++)
                                    ptr.Set(a.data, j * a.data.Length);
                                return ptr;
                            }
                        }
                        else if (a.Is(VValueType.Number))
                        {
                            if (b.Is(VValueType.Number))
                            {
                                return new DataStruct(VValueType.Number, BitConverter.GetBytes(BitConverter.ToDouble(a.data) * BitConverter.ToDouble(b.data)));
                            }
                        }
                        throw new VaException(line, $"Not supports * operation between {a.type} and {b.type}.");
                    }),
                    (new TokenStyle("/", TokenType.Special),
                    (sln, line, a, b) =>
                    {
                        if (a.Is(VValueType.Number))
                        {
                            if (b.Is(VValueType.Number))
                                return new DataStruct(VValueType.Number, BitConverter.GetBytes(BitConverter.ToDouble(a.data) / BitConverter.ToDouble(b.data)));
                        }
                        throw new VaException(line, $"Not supports / operation between {a.type} and {b.type}.");
                    })
                },
                new (TokenStyle, ExpressionStyleDelegate)[]
                {
                    (new TokenStyle("==", TokenType.Special),
                    (sln, line, a, b) =>
                    {
                        if (a.Is(VValueType.Bool))
                        {
                            if (b.Is(VValueType.Bool))
                            {
                                return new DataStruct(VValueType.Bool, new byte[] { (byte)(a[0] & b[0]) });
                            }
                        }
                        else if (a.Is(VValueType.Number))
                        {
                            if (b.Is(VValueType.Number))
                                return new DataStruct(VValueType.Number, new byte[] { (byte)(BitConverter.ToDouble(a.data) == BitConverter.ToDouble(b.data) ? 0b1 : 0b0) });
                        }
                        throw new VaException(line, $"Not supports == operation between {a.type} and {b.type}.");
                    }),
                    (new TokenStyle("!=", TokenType.Special),
                    (sln, line, a, b) =>
                    {
                        if (a.Is(VValueType.Bool))
                        {
                            if (b.Is(VValueType.Bool))
                            {
                                return new DataStruct(VValueType.Bool, new byte[] { (byte)((a[0] & b[0]) ^ 0b1) });
                            }
                        }
                        else if (a.Is(VValueType.Number))
                        {
                            if (b.Is(VValueType.Number))
                                return new DataStruct(VValueType.Number, new byte[] { (byte)(BitConverter.ToDouble(a.data) != BitConverter.ToDouble(b.data) ? 0b1 : 0b0) });
                        }
                        throw new VaException(line, $"Not supports != operation between {a.type} and {b.type}.");
                    }),
                    (new TokenStyle("<", TokenType.Special),
                    (sln, line, a, b) =>
                    {
                        if (a.Is(VValueType.Number))
                        {
                            if (b.Is(VValueType.Number))
                                return new DataStruct(VValueType.Number, new byte[] { (byte)(BitConverter.ToDouble(a.data) <= BitConverter.ToDouble(b.data) ? 0b1 : 0b0) });
                        }
                        throw new VaException(line, $"Not supports < operation between {a.type} and {b.type}.");
                    }),
                    (new TokenStyle("<=", TokenType.Special),
                    (sln, line, a, b) =>
                    {
                        if (a.Is(VValueType.Number))
                        {
                            if (b.Is(VValueType.Number))
                                return new DataStruct(VValueType.Number, new byte[] { (byte)(BitConverter.ToDouble(a.data) < BitConverter.ToDouble(b.data) ? 0b1 : 0b0) });
                        }
                        throw new VaException(line, $"Not supports <= operation between {a.type} and {b.type}.");
                    }),
                    (new TokenStyle(">", TokenType.Special),
                    (sln, line, a, b) =>
                    {
                        if (a.Is(VValueType.Number))
                        {
                            if (b.Is(VValueType.Number))
                                return new DataStruct(VValueType.Number, new byte[] { (byte)(BitConverter.ToDouble(a.data) > BitConverter.ToDouble(b.data) ? 0b1 : 0b0) });
                        }
                        throw new VaException(line, $"Not supports > operation between {a.type} and {b.type}.");
                    }),
                    (new TokenStyle(">=", TokenType.Special),
                    (sln, line, a, b) =>
                    {
                        if (a.Is(VValueType.Number))
                        {
                            if (b.Is(VValueType.Number))
                                return new DataStruct(VValueType.Number, new byte[] { (byte)(BitConverter.ToDouble(a.data) >= BitConverter.ToDouble(b.data) ? 0b1 : 0b0) });
                        }
                        throw new VaException(line, $"Not supports >= operation between {a.type} and {b.type}.");
                    })
                }
            );

            inCodeRegion = new CompileStyle(
                new(
                    new TokenStyle[]
                    {
                        new TokenStyle("print", TokenType.Keyword),
                        new TokenStyle(TokenType.Parenthese)
                    },
                    (CompileStyleDelegate)
                    ((sln, toks) =>
                    {
                        var val = expression.Parse(sln, toks[1].tokens[0]);
                        Console.WriteLine(Encoding.Unicode.GetString(val.data));
                    })
                ),
                new(
                    new TokenStyle[]
                    {
                        new TokenStyle(TokenType.Keyword),
                        new TokenStyle("=", TokenType.Special),
                        new TokenStyle(TokenType.ToEnd),
                    },
                    (CompileStyleDelegate)
                    ((sln, toks) =>
                    {
                        sln[toks[0].Text] = expression.Parse(sln, toks[2..]);
                    })
                ),
                new(
                    new TokenStyle[]
                    {
                        new TokenStyle(TokenType.Keyword),
                        new TokenStyle(TokenType.Parenthese)
                    },
                    (CompileStyleDelegate)
                    ((sln, toks) =>
                    {
                        Solution solution = new Solution(mainSolution);

                        FunctionKey key = new FunctionKey(toks[0].Text,
                            toks[1].tokens.Length == 1 && toks[1].tokens[0].Length == 0
                            ? 0 : toks[1].tokens.Length);

                        Function function = sln[key];

                        for (int i = 0; i < key.arguments; i++)
                            solution.Add(function.args[i], expression.Parse(sln, toks[1].tokens[i]));
                        Compiler.ParseStyle(solution, inCodeRegion, function.code);
                    })
                ),
                new(
                    new TokenStyle[]
                    {
                        new TokenStyle("ret", TokenType.Keyword)
                    },
                    (CompileStyleDelegate)
                    ((sln, toks) =>
                    {
                        sln.breaked = true;
                    })
                ),
                new(
                    new TokenStyle[]
                    {
                        new TokenStyle("ret", TokenType.Keyword),
                        new TokenStyle(TokenType.ToEnd)
                    },
                    (CompileStyleDelegate)
                    ((sln, toks) =>
                    {
                        sln.toReturn = expression.Parse(sln, toks[1..]);
                        sln.breaked = true;
                    })
                )
            );
            main = new CompileStyle(
                new(
                    new TokenStyle[]
                    {
                        new TokenStyle("func", TokenType.Keyword),
                        new TokenStyle(TokenType.Keyword),
                        new TokenStyle(TokenType.Parenthese),
                        new TokenStyle(TokenType.Brace)
                    },
                    (CompileStyleDelegate)
                    ((sln, toks) =>
                    {
                        FunctionKey functionKey;
                        if (toks[2].tokens.Length == 1 && toks[2].tokens[0].Length == 0)
                            functionKey = new FunctionKey(toks[1].Text, 0);
                        else
                            functionKey = new FunctionKey(toks[1].Text, toks[2].tokens.Length);
                        string[] args = new string[functionKey.arguments];
                        for (int i = 0; i < functionKey.arguments; i++)
                        {
                            args[i] = toks[2].tokens[i][0].Text;
                        }
                        if (!sln.TryAdd(functionKey, new Function(toks[3].tokens, args)))
                            throw new VaException(toks[1].line, $"Already have function with same name and arguments {toks[1].Text}.");
                    })
                ),
                new(
                    new TokenStyle[]
                    {
                        new TokenStyle("static", TokenType.Keyword),
                        new TokenStyle(TokenType.Keyword),
                        new TokenStyle(TokenType.Parenthese),
                        new TokenStyle(TokenType.Brace)
                    },
                    (CompileStyleDelegate)
                    ((sln, toks) =>
                    {
                        FunctionKey functionKey;
                        if (toks[2].tokens.Length == 1 && toks[2].tokens[0].Length == 0)
                            functionKey = new FunctionKey(toks[1].Text, 0);
                        else
                            functionKey = new FunctionKey(toks[1].Text, toks[2].tokens.Length);
                        string[] args = new string[functionKey.arguments];
                        for (int i = 0; i < functionKey.arguments; i++)
                        {
                            args[i] = toks[2].tokens[i][0].Text;
                        }
                        if (!sln.TryAdd(functionKey, new Function(toks[3].tokens, args)))
                            throw new VaException(toks[1].line, $"Already have function with same name and arguments {toks[1].Text}.");
                    })
                )
            );

            Token[][] tokens = Compiler.GetTokens(code);
            Compiler.ParseStyle(mainSolution, main, tokens);
        }

        public void Call(string name, params string[] args)
        {
            Solution solution = new Solution(mainSolution);
            FunctionKey key = new FunctionKey(name, args.Length);

            Function function = mainSolution[key];

            for (int i = 0; i < key.arguments; i++)
                solution.Add(function.args[i], expression.Parse(mainSolution, Compiler.GetTokens(args[i])[0]));

            Compiler.ParseStyle(solution, inCodeRegion, function.code);
        }
    }
}
