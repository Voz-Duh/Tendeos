using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Tendeos.Utils;

[Flags]
public enum TokenType: byte
{
    SPACE = 0,
    LINE_SPLIT = 1,
    SPECIAL = 2,
    KEYWORD = 4,
    INTEGER = 8,
    FLOATING = 16,
    GROUP = 32,
    LITERAL = 64
}

public struct Token
{
    public TokenType Type;
    public string? Value;
    public (int line, int column) Coline;
    public int Space;
    public string? Text;
    public Token[]? Tokens;

    public Token(TokenType Type, string Value, (int, int) Coline, Token[]? Tokens = null, string? Text = null,
        int Space = 0)
    {
        this.Type = Type;
        this.Value = Value;
        this.Coline = Coline;
        this.Tokens = Tokens;
        this.Text = Text;
        this.Space = Space;
    }

    public Token(TokenType Type, string Value) : this(Type, Value, (0, 0))
    {
    }

    public static bool operator ==(Token a, Token b) =>
        a.Type == b.Type && (a.Value == null || b.Value == null || a.Value == b.Value);

    public static bool operator !=(Token a, Token b) => !(a == b);

    public static bool operator ==(Token a, (TokenType Type, string? Value) b) =>
        a.Type == b.Type && (a.Value == null || b.Value == null || a.Value == b.Value);

    public static bool operator !=(Token a, (TokenType Type, string? Value) b) => !(a == b);

    public static bool operator ==(Token a, TokenType type) => a.Type == type;

    public static bool operator !=(Token a, TokenType type) => !(a == type);

    public override int GetHashCode() => Value == null ? Type.GetHashCode() : (Type, Value).GetHashCode();

    public override bool Equals(object? obj) => obj is Token other && this == other;
}

public static class TextParser
{
    public class ParsingException : Exception
    {
        public int Line { get; }
        public int Column { get; }
        public string BaseMessage { get; }

        public ParsingException((int, int) coline, string message, string code)
            : base($"{message} on line {coline.Item1 + 1} column {coline.Item2}:\n{code.Split('\n')[coline.Item1]}\n{new string(' ', coline.Item2-1)}^")
        {
            BaseMessage = message;
            Line = coline.Item1;
            Column = coline.Item2;
        }

        public override string ToString() => Message;
    }

    public class Parser
    {
        public string FullText { get; }
        public int Caret { get; set; }
        public string Text { get; set; }
        public int Length { get; set; }
        public List<Token> Tokens { get; set; }
        public (string, int, string, int)[] Comments { get; set; }
        public (string, int, string, int)[] Groups { get; set; }
        public (string, int, string, int, string, int, (string, int, string, string, int)[])[] Literals { get; set; }
        public int Lines { get; set; }
        public int LastLine { get; set; }
        public int Column { get; set; }

        public Parser(string text) => FullText = text;

        public string Slice(int from, int to) => Text[from..to];
        public string Substring(int index, int length) => Text.Substring(index, length);

        public bool SubstringEquals(int index, int length, string str) =>
            TextParser.SubstringEquals(Text, index, length, str);

        public (int, int, int) Coline => (Lines, Caret - LastLine + Column, Caret);
    }

    private class RefValue<T>
    {
        public T _;

        public RefValue(T val) => _ = val;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static int FindSplitPoint(string text, int start, int end,
        (string, string, string, (string, string)[])[] literals,
        (string, string)[] comments,
        (string, string)[] groups)
    {
        bool inLiteral = false;
        bool inComment = false;
        bool inGroup = false;
        string literalSkip = "";
        string literalEnd = "";

        int i = start;
        while (i < end)
        {
            if (inLiteral)
            {
                if (SubstringEquals(text, i, Math.Min(literalSkip.Length, end - i), literalSkip))
                {
                    i += 1;
                }
                else if (SubstringEquals(text, i, Math.Min(literalEnd.Length, end - i), literalEnd))
                {
                    inLiteral = false;
                    i += literalEnd.Length - 1;
                }
            }
            else if (inComment)
            {
                if (SubstringEquals(text, i, Math.Min(literalEnd.Length, end - i), literalEnd))
                {
                    inComment = false;
                    i += literalEnd.Length - 1;
                }
            }
            else if (inGroup)
            {
                foreach (var lit in literals)
                {
                    if (!SubstringEquals(text, i, Math.Min(lit.Item1.Length, end - i), lit.Item1)) continue;

                    i += lit.Item1.Length - 1;

                    while (i < end)
                    {
                        if (SubstringEquals(text, i, Math.Min(literalSkip.Length, end - i), literalSkip))
                        {
                            i += 1;
                        }
                        else if (SubstringEquals(text, i, Math.Min(literalEnd.Length, end - i), literalEnd))
                        {
                            i += literalEnd.Length - 1;
                            break;
                        }
                    }
                }
                
                if (SubstringEquals(text, i, Math.Min(literalEnd.Length, end - i), literalEnd))
                {
                    inGroup = false;
                    i += literalEnd.Length - 1;
                }
            }
            else
            {
                foreach (var lit in literals)
                {
                    if (!SubstringEquals(text, i, Math.Min(lit.Item1.Length, end - i), lit.Item1)) continue;

                    inLiteral = true;
                    literalEnd = lit.Item2;
                    literalSkip = lit.Item3;
                    i += lit.Item1.Length - 1;
                    break;
                }

                foreach (var com in comments)
                {
                    if (!SubstringEquals(text, i, Math.Min(com.Item1.Length, end - i), com.Item1)) continue;

                    inComment = true;
                    literalEnd = com.Item2;
                    i += com.Item1.Length - 1;
                    break;
                }

                foreach (var grp in groups)
                {
                    if (!SubstringEquals(text, i, Math.Min(grp.Item1.Length, end - i), grp.Item1)) continue;

                    inGroup = true;
                    literalEnd = grp.Item2;
                    i += grp.Item1.Length - 1;
                    break;
                }
            }

            if (!inLiteral && !inComment && !inGroup && text[i] == ' ' && i > start + 4500)
            {
                return i;
            }

            i++;
        }

        return -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static Parser[] CreatePredefinedParser(string text,
        (string start, int, string end, int, string escape, int, (string specialSymbol, int, string realSpecialSymbol,
            string, int)[] specialSymbols)[] literals,
        (string start, int, string end, int)[] comments,
        (string start, int, string end, int)[] groups,
        int column = 0, string? fullText = null)
    {
        Parser parser;
        Parser[] additionalParser = Array.Empty<Parser>();

        int textLen = text.Length;
        int splitPoint = textLen >= 5000
            ? FindSplitPoint(text, 0, textLen,
                literals.Select(e => (e.start, e.end, e.escape,
                    e.specialSymbols.Select(e => (e.specialSymbol, e.realSpecialSymbol)).ToArray())).ToArray(),
                comments.Select(e => (e.start, e.end)).ToArray(),
                groups.Select(e => (e.start, e.end)).ToArray())
            : -1;

        if (splitPoint != -1)
        {
            additionalParser = CreatePredefinedParser(text.Substring(splitPoint), literals, comments, groups, column, fullText);
            text = text.Substring(0, splitPoint);
        }

        parser = new Parser(fullText ?? text)
        {
            Caret = 0,
            Text = text,
            Length = textLen,
            Tokens = new(),
            Comments = comments,
            Groups = groups,
            Literals = literals,
            Lines = 0,
            LastLine = 0,
            Column = column
        };

        var result = additionalParser.ToList();
        result.Insert(0, parser);
        return result.ToArray();
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static Parser[] CreateParser(string text,
        (string start, string end, string escape, (string specialSymbol, string realSpecialSymbol)[] specialSymbols)[]
            literals,
        (string start, string end)[] comments,
        (string start, string end)[] groups,
        int column = 0, string? fullText = null)
    {
        Parser parser;
        Parser[] additionalParser = Array.Empty<Parser>();

        int textLen = text.Length;
        int splitPoint = textLen >= 5000 ? FindSplitPoint(text, 0, textLen, literals, comments, groups) : -1;

        var realComments = comments.Select(com => (
            com.Item1, com.Item1.Length,
            com.Item2, com.Item2.Length
        )).ToArray();

        var realGroups = groups.Select(grp => (
            grp.Item1, grp.Item1.Length,
            grp.Item2, grp.Item2.Length
        )).ToArray();

        var realLiterals = literals.Select(lit => (
            lit.Item1, lit.Item1.Length,
            lit.Item2, lit.Item2.Length,
            lit.Item3, lit.Item3.Length,
            lit.Item4.Select(special => (
                special.Item1, special.Item1.Length,
                special.Item2,
                lit.Item3 + special.Item1, special.Item1.Length + lit.Item3.Length
            )).Append((
                lit.Item2, lit.Item2.Length,
                lit.Item2,
                lit.Item3 + lit.Item2, lit.Item2.Length + lit.Item3.Length
            )).ToArray()
        )).ToArray();

        if (splitPoint != -1)
        {
            additionalParser = CreatePredefinedParser(text.Substring(splitPoint), realLiterals, realComments, realGroups, column, fullText);
            text = text.Substring(0, splitPoint);
        }

        parser = new Parser(fullText ?? text)
        {
            Caret = 0,
            Text = text,
            Length = textLen,
            Tokens = new(),
            Comments = realComments,
            Groups = realGroups,
            Literals = realLiterals,
            Lines = 0,
            LastLine = 0,
            Column = column
        };

        var result = additionalParser.ToList();
        result.Insert(0, parser);
        return result.ToArray();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static Token? GetLastValidToken(Parser parser)
    {
        int length = parser.Tokens.Count;
        if (length <= 0) return null;
        for (int i = 0; i > -length; i--)
            if (parser.Tokens[i].Type is not TokenType.SPACE or TokenType.LINE_SPLIT)
                return parser.Tokens[i];
        return null;
    }
    

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static bool SubstringEquals(string source, int index, int length, string str)
    {
        if (str.Length != length) return false;
        for (int i = 0; i < length; i++)
            if (source[index+i] != str[i]) return false;

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static string Remove(string s, int start, int length, string inside = "") =>
        new StringBuilder(s.Length + inside.Length - length)
            .Append(s, 0, start)
            .Append(inside)
            .Append(s, start + length, s.Length - length - start)
            .ToString();

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static int GetLines(string s, int start, int length)
    {
        int end = start + length;
        int count = 0;
    
        for (int i = start; i < end; i++)
            if (s[i] == '\n')
                count++;
    
        return count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void RemoveComments(Parser parser) => RemoveComment(0, parser);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static void RemoveComment(int start, Parser parser)
    {
        NEW: ;
        int i = start;
        while (i < parser.Length)
        {
            int skip = SkipLiteral((0, 0, i), parser, false);

            if (skip > 0)
            {
                i += skip;
                continue;
            }
            
            foreach (var comment in parser.Comments)
            {
                if (i + comment.Item2 >= parser.Length
                    || !parser.SubstringEquals(i, comment.Item2, comment.Item1))
                    continue;

                int j = comment.Item2;
                while (j < parser.Length - i + 1)
                {
                    if (i + j + comment.Item4 < parser.Length)
                    {
                        if (parser.SubstringEquals(i + j, comment.Item4, comment.Item3))
                        {
                            string newText = Remove(parser.Text, i, j + comment.Item4,
                                new string('\n', GetLines(parser.Text, i, j + comment.Item4)));
                            parser.Text = newText;
                            parser.Length = newText.Length;
                            start = i;
                            goto NEW;
                        }
                    }
                    else
                    {
                        int lines = GetLines(parser.Text, i, parser.Length);
                        parser.Text = new StringBuilder(i + lines)
                            .Append(parser.Substring(0, i))
                            .Append(new string('\n', lines))
                            .ToString();
                        parser.Length = parser.Text.Length;
                    }

                    j++;
                }

                throw new Exception($"Not closed comment at index {i}");
            }

            i++;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static void SkipWhitespaces(Parser parser, RefValue<int> lineSummary)
    {
        int count = 0;
        while (parser.Caret < parser.Length)
        {
            char cur = parser.Text[parser.Caret];
            if (cur == ' ')
            {
                count++;
                parser.Caret++;
            }
            else if (cur == '\t')
            {
                count += 4;
                parser.Caret++;
            }
            else if (cur == '\n')
            {
                if (count != 0)
                {
                    parser.Tokens.Add(new Token(TokenType.SPACE, null, (lineSummary._, parser.Column), Space: count));
                }

                count = 0;
                parser.Lines++;
                lineSummary._++;
                parser.LastLine = parser.Caret;

                parser.Tokens.Add(new Token(TokenType.LINE_SPLIT, "\n", (lineSummary._, parser.Column)));
                parser.Column = 0;
                parser.Caret++;
            }
            else
            {
                if (count != 0)
                {
                    parser.Tokens.Add(new Token(TokenType.SPACE, null, (lineSummary._, parser.Column), Space: count));
                }

                break;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSpecial(char c) =>
        !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c) && c != '_';

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsKeywordStart(char c) =>
        char.IsLetter(c) || c == '_';

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsKeyword(char c) =>
        char.IsLetterOrDigit(c) || c == '_';

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsNumber(char c) => char.IsDigit(c);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static void GetSpecial((int, int, int) start, Parser parser)
    {
        while (parser.Caret < parser.Length)
        {
            char cur = parser.Text[parser.Caret];
            if (!IsSpecial(cur))
            {
                parser.Tokens.Add(new Token(TokenType.SPECIAL, parser.Slice(start.Item3, parser.Caret),
                    (start.Item1, start.Item2)));
                return;
            }

            parser.Caret++;
        }

        parser.Tokens.Add(new Token(TokenType.SPECIAL, parser.Slice(start.Item3, parser.Length),
            (start.Item1, start.Item2)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static void GetKeyword((int, int, int) start, Parser parser)
    {
        while (parser.Caret < parser.Length)
        {
            char cur = parser.Text[parser.Caret];
            if (!IsKeyword(cur))
            {
                parser.Tokens.Add(new Token(TokenType.KEYWORD, parser.Slice(start.Item3, parser.Caret),
                    (start.Item1, start.Item2)));
                return;
            }

            parser.Caret++;
        }

        parser.Tokens.Add(new Token(TokenType.KEYWORD, parser.Slice(start.Item3, parser.Length),
            (start.Item1, start.Item2)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static void GetNumber((int, int, int) start, Parser parser)
    {
        while (parser.Caret < parser.Length)
        {
            char cur = parser.Text[parser.Caret];
            if (!(IsNumber(cur) || cur == '.'))
            {
                string text = parser.Slice(start.Item3, parser.Caret);
                parser.Tokens.Add(new Token(text.Contains('.') ? TokenType.FLOATING : TokenType.INTEGER, text,
                    (start.Item1, start.Item2)));
                return;
            }

            parser.Caret++;
        }

        string finalText = parser.Slice(start.Item3, parser.Length);
        parser.Tokens.Add(new Token(finalText.Contains('.') ? TokenType.FLOATING : TokenType.INTEGER, finalText,
            (start.Item1, start.Item2)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static void GetDot((int, int, int) start, Parser parser)
    {
        char cur = parser.Text[parser.Caret];

        if (IsNumber(cur))
        {
            parser.Caret++;
            GetNumber(start, parser);
        }
        else if (IsSpecial(cur))
        {
            parser.Caret++;
            GetSpecial(start, parser);
        }
        else
        {
            parser.Tokens.Add(new Token(TokenType.SPECIAL, parser.Slice(start.Item3, parser.Caret),
                (start.Item1, start.Item2)));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static void GetMinus((int, int, int) start, Parser parser)
    {
        char cur = parser.Text[parser.Caret];
        Token? last = GetLastValidToken(parser);
        bool canBeNumber = !last.HasValue || last.Value.Type == TokenType.SPECIAL;

        switch (canBeNumber)
        {
            case true when IsNumber(cur):
                parser.Caret++;
                GetNumber(start, parser);
                break;
            case true when cur == '.':
                parser.Caret++;
                GetSpecial(start, parser);
                break;
            default:
            {
                if (IsSpecial(cur))
                {
                    parser.Caret++;
                    GetSpecial(start, parser);
                }
                else
                {
                    parser.Tokens.Add(new Token(TokenType.SPECIAL, parser.Slice(start.Item3, parser.Caret),
                        (start.Item1, start.Item2)));
                }

                break;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static int SkipLiteral((int, int, int) start, Parser parser, bool errors = true)
    {
        int startPosition = start.Item3;
        (int line, int col) = (start.Item1, start.Item2);

        foreach (var literal in parser.Literals)
        {
            if (startPosition + literal.Item2 >= parser.Length
                || !parser.SubstringEquals(startPosition, literal.Item2, literal.Item1))
                continue;

            bool lastEscape = false;
            int j = literal.Item2;

            while (j < parser.Length - startPosition)
            {
                int currentPosition = startPosition + j;
                if (currentPosition + literal.Item6 - 1 < parser.Length &&
                    parser.SubstringEquals(currentPosition, literal.Item6, literal.Item5))
                {
                    lastEscape = true;
                    j += literal.Item6 - 1;
                    currentPosition += literal.Item6 - 1;
                }

                if (lastEscape)
                {
                    bool invalid = literal.Item7.All(special =>
                        currentPosition + special.Item2 >= parser.Length ||
                        !parser.SubstringEquals(currentPosition, special.Item2, special.Item1));

                    if (invalid)
                    {
                        return errors ?
                            throw new ParsingException(
                                (line, col + j + literal.Item2 - 1),
                                "Undefined literal special symbol", parser.FullText) : 0;
                    }
                }
                else
                {
                    if (currentPosition + literal.Item4 - 1 >= parser.Length)
                    {
                        return errors ? throw new ParsingException((line, col), "Not closed literal", parser.FullText) : 0;
                    }

                    if (parser.SubstringEquals(currentPosition, literal.Item4, literal.Item3))
                    {
                        return j + literal.Item4;
                    }
                }

                lastEscape = false;
                j += 1;
            }

            return errors ? throw new ParsingException((line, col), "Not closed literal", parser.FullText) : 0;
        }

        return 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static async Task<bool> GetGroup((int, int, int) start, Parser parser, RefValue<int> lineSummary)
    {
        var coline = (lineSummary._, start.Item2);
        int startPos = start.Item3;

        foreach (var group in parser.Groups)
        {
            if (startPos + group.Item2 < parser.Length &&
                parser.SubstringEquals(startPos, group.Item2, group.Item1))
            {
                int j = group.Item2;
                int counter = 1;

                while (j < parser.Length - startPos)
                {
                    var skipColine = parser.Coline;
                    int skip = SkipLiteral((lineSummary._, skipColine.Item2 + j, startPos + j), parser);

                    if (skip > 0)
                    {
                        j += skip;
                    }

                    if (startPos + j + group.Item2 - 1 < parser.Length &&
                        parser.SubstringEquals(startPos + j, group.Item2, group.Item1))
                    {
                        counter += 1;
                    }
                    else if (startPos + j + group.Item4 - 1 < parser.Length)
                    {
                        if (parser.SubstringEquals(startPos + j, group.Item4, group.Item3))
                        {
                            counter -= 1;
                            if (counter == 0)
                            {
                                var text = parser.Slice(startPos + group.Item2, startPos + j);
                                lineSummary._ += GetLines(text, 0, j - group.Item2);
                                parser.Caret += j + group.Item4 - 1;

                                var insideParser = CreatePredefinedParser(text, parser.Literals, parser.Comments,
                                    parser.Groups, coline.Item2 + 1);
                                var tokens = await GetParsersTokens(insideParser, coline.Item1);

                                parser.Tokens.Add(new Token(TokenType.GROUP, group.Item1, (coline.Item1, coline.Item2),
                                    Tokens: tokens.ToArray()));
                                return true;
                            }
                        }
                    }
                    else
                    {
                        throw new ParsingException(coline, "Not closed group", parser.FullText);
                    }

                    j += 1;
                }

                throw new ParsingException(coline, "Not closed group", parser.FullText);
            }
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static bool GetLiteral((int, int, int) start, Parser parser, RefValue<int> lineSummary)
    {
        int startPos = start.Item3;
        var coline = (lineSummary._, start.Item2);

        foreach (var literal in parser.Literals)
        {
            if (startPos + literal.Item2 < parser.Length &&
                parser.SubstringEquals(startPos, literal.Item2, literal.Item1))
            {
                bool lastEscape = false;
                int j = literal.Item2;

                while (j < parser.Length - startPos)
                {
                    int cur = startPos + j;

                    if (cur + literal.Item6 < parser.Length &&
                        parser.SubstringEquals(cur, literal.Item6, literal.Item5))
                    {
                        lastEscape = true;
                        j += literal.Item6;
                        cur += literal.Item6;
                    }

                    if (lastEscape)
                    {
                        bool invalid = literal.Item7.All(special =>
                            cur + special.Item2 >= parser.Length ||
                            !parser.SubstringEquals(cur, special.Item2, special.Item1));

                        if (invalid)
                        {
                            throw new ParsingException((coline.Item1, coline.Item2 + j + literal.Item2 - 1),
                                "Undefined literal special symbol", parser.FullText);
                        }
                    }
                    else
                    {
                        if (cur + literal.Item4 - 1 >= parser.Length)
                        {
                            throw new ParsingException(coline, "Not closed literal", parser.FullText);
                        }

                        if (parser.SubstringEquals(cur, literal.Item4, literal.Item3))
                        {
                            string text = parser.Slice(startPos + literal.Item2, cur);
                            int length = text.Length;
                            int l = 0;

                            while (l < length)
                            {
                                foreach (var special in literal.Item7)
                                {
                                    if (l + special.Item5 <= length &&
                                        SubstringEquals(text, l, special.Item5, special.Item4))
                                    {
                                        length -= special.Item5 - 1;
                                        text = new StringBuilder(length - (special.Item2 + 1) + special.Item3.Length)
                                            .Append(text[..l])
                                            .Append(special.Item3)
                                            .Append(text[(l + special.Item2 + 1)..])
                                            .ToString();
                                        break;
                                    }
                                }

                                l++;
                            }

                            parser.Caret += j + literal.Item4 - 1;
                            parser.Tokens.Add(new Token(
                                TokenType.LITERAL,
                                literal.Item1,
                                coline,
                                Text: text));
                            return true;
                        }
                    }

                    lastEscape = false;
                    j++;
                }

                throw new ParsingException(coline, "Not closed literal", parser.FullText);
            }
        }

        return false;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static async Task GetToken(Parser parser, RefValue<int> lineSummary)
    {
        SkipWhitespaces(parser, lineSummary);
        if (parser.Caret >= parser.Length) return;
        char cur = parser.Text[parser.Caret];
        var start = parser.Coline;
        start = (lineSummary._, start.Item2, start.Item3);
        parser.Caret++;

        switch (cur)
        {
            case '.':
                GetDot(start, parser);
                return;
            case '-':
                GetMinus(start, parser);
                return;
        }

        if (await GetGroup(start, parser, lineSummary)
            || GetLiteral(start, parser, lineSummary))
            return;

        if (IsSpecial(cur))
        {
            GetSpecial(start, parser);
            return;
        }

        if (IsKeywordStart(cur))
        {
            GetKeyword(start, parser);
            return;
        }

        if (IsNumber(cur))
        {
            GetNumber(start, parser);
            return;
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static async Task GetParserTokens(Parser parser, RefValue<int>? lineSummary = null)
    {
        lineSummary ??= new RefValue<int>(0);

        RemoveComments(parser);
        while (parser.Caret < parser.Length)
        {
            await GetToken(parser, lineSummary);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static async Task<List<Token>> GetParsersTokens(Parser[] parsers, int line = 0)
    {
        var tasks = parsers.Select(parser => GetParserTokens(parser, new RefValue<int>(line))).ToList();

        foreach (Task t in tasks)
        {
            try
            {
                await t;
            }
            catch (ParsingException e)
            {
                line = e.Line;
                throw new ParsingException((line, e.Column), e.BaseMessage, parsers[0].FullText);
            }
        }

        List<Token> result = null;
        int last = 0;

        foreach (Parser parser in parsers)
        {
            if (result != null)
            {
                result.AddRange(parser.Tokens.ConvertAll(token => new Token(token.Type, token.Value,
                    (token.Coline.line + last, token.Coline.column), token.Tokens, token.Text, token.Space)));
            }
            else
            {
                result = parser.Tokens;
            }

            last = parser.Tokens.Count > 0 ? parser.Tokens[^1].Coline.line : 0;
        }

        return result;
    }

    public static List<Token> GetTokens(Parser[] parsers)
    {
        var task = GetParsersTokens(parsers);
        task.Wait();

        return task.Result;
    }
}

public class TokenManager
{
    private List<Token> tokens;
    private string fullText;
    
    public int Length { get; private set; }
    public int Caret { get; private set; }

    public TokenManager(TextParser.Parser[] parsers, List<Tuple<Action<TokenManager, object[]>, object[]>>? modules = null)
    {
        fullText = parsers[0].FullText;
        tokens = TextParser.GetTokens(parsers);
        Length = tokens.Count;
        Caret = -1;
        
        modules?.ForEach(module =>
        {
            module.Item1(this, module.Item2);
            Caret = -1;
        });
    }

    public TokenManager(TokenManager parent, List<Token> tokens)
    {
        fullText = parent.fullText;
        this.tokens = tokens;
        Length = tokens.Count;
        Caret = -1;
    }

    public TokenManager(TokenManager parent, Token[] tokens) : this(parent, tokens.ToList()) {}

    public Token Current => tokens[Caret];

    public Token this[int i] => tokens[i];

    public List<Token> this[Range range] => tokens[range];

    public Token Next
    {
        get
        {
            Caret++;
            if (!Valid) throw new TextParser.ParsingException(tokens[Length - 1].Coline, "Not expected end", fullText);
            return tokens[Caret];
        }
    }

    public bool Move => ++Caret < Length;

    public bool MoveBack => --Caret >= 0;

    public bool Valid => Caret >= 0 && Caret < Length;

    public Token INext
    {
        get
        {
            Caret++;
            if (!Valid)
            {
                throw new TextParser.ParsingException(tokens[Length - 1].Coline, "Not expected end", fullText);
            }

            while (tokens[Caret].Type is TokenType.SPACE or TokenType.LINE_SPLIT)
            {
                Caret++;
                if (!Valid)
                {
                    throw new TextParser.ParsingException(tokens[Length - 1].Coline, "Not expected end", fullText);
                }
            }

            return tokens[Caret];
        }
    }

    public bool IMove
    {
        get
        {
            Caret++;
            if (Caret >= Length)
            {
                return false;
            }

            while (tokens[Caret].Type is TokenType.SPACE or TokenType.LINE_SPLIT)
            {
                Caret++;
                if (Caret >= Length)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public bool IMoveBack
    {
        get
        {
            Caret--;
            if (Caret < 0)
            {
                return false;
            }

            while (tokens[Caret].Type is TokenType.SPACE or TokenType.LINE_SPLIT)
            {
                Caret--;
                if (Caret < 0)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public void ToEnd() => Caret = Length;

    public void ToStart() => Caret = -1;

    public bool IsEmpty(int start = 0, int end = -1)
    {
        end = end == -1 ? Length : end;
        return !tokens.Skip(start).Take(end - start).Any(token => token.Type != TokenType.SPACE && token.Type != TokenType.LINE_SPLIT);
    }

    public List<Token> SkipTokensAfter
    {
        get
        {
            var result = new List<Token>();
            Caret++;
            if (Caret >= Length)
            {
                return result;
            }

            while (tokens[Caret].Type != TokenType.LINE_SPLIT)
            {
                result.Add(tokens[Caret]);
                Caret++;
                if (Caret >= Length)
                {
                    return result;
                }
            }

            return result;
        }
    }

    public bool NextLine
    {
        get
        {
            Caret++;
            if (!Valid)
            {
                return false;
            }

            while (tokens[Caret].Type != TokenType.LINE_SPLIT)
            {
                Caret++;
                if (Valid) continue;
                Caret--;
                break;
            }

            return true;
        }
    }

    public bool LastLine
    {
        get
        {
            Caret--;
            if (!Valid)
            {
                return false;
            }

            while (tokens[Caret].Type != TokenType.LINE_SPLIT)
            {
                Caret--;
                if (Valid) continue;
                Caret++;
                break;
            }

            return true;
        }
    }

    public bool MoveTo(Token token)
    {
        while (Valid)
        {
            if (tokens[Caret] == token)
                return true;
            Caret++;
        }
        return false;
    }

    public void Remove(int index)
    {
        tokens.RemoveAt(index);
        Length--;
    }

    public void RemoveSlice(int start, int end)
    {
        tokens.RemoveRange(start, end - start);
        Length -= end - start;
    }

    public void RemoveFrom(int start)
    {
        tokens.RemoveRange(start, Caret - start);
        Length -= Caret - start;
        Caret = start;
    }

    public void RemoveWhitespaces()
    {
        tokens = tokens.Where(token => token.Type != TokenType.SPACE && token.Type != TokenType.LINE_SPLIT).ToList();
        Length = tokens.Count;
    }

    public List<((int line, int column) coline, List<Token> tokens)> Split(TokenType type, string value)
    {
        if (Length == 0)
        {
            return new List<((int, int), List<Token>)>();
        }

        var result = new List<((int, int), List<Token>)>();
        int i = 0;
        int last = 0;

        foreach (var token in tokens)
        {
            if (token.Type == type && token.Value == value)
            {
                result.Add((tokens[last == 0 ? 0 : last - 1].Coline, tokens.GetRange(last, i - last)));
                last = i + 1;
            }
            i++;
        }
        result.Add((tokens[last == 0 ? 0 : last - 1].Coline, tokens.GetRange(last, tokens.Count - last)));
        return result;
    }

    public TextParser.ParsingException CreateException((int, int) coline, string message) =>
        new(coline, message, fullText);

    public TextParser.ParsingException CreateExceptionOnCurrent( string message) =>
        new(Current.Coline, message, fullText);
}
