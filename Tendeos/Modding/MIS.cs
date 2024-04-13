using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Va;

namespace Tendeos.Modding
{
    public enum MISValue { Object, Number, Range, String, Key, Boolean, Non }

    public static class MIS
    {
        private static readonly ValueStyle valueStyle = null;
        private static readonly CompileStyle compileStyle = null;

        static MIS()
        {
            valueStyle = new ValueStyle(
                (new[] { new TokenStyle(TokenType.Number) }, (sln, toks) => new DataStruct(MISValue.Number, toks[0].data)),
                (new[] { new TokenStyle(TokenType.Number), new TokenStyle("::", TokenType.Special), new TokenStyle(TokenType.Number) },
                    (sln, toks) => new DataStruct(MISValue.Range, toks[0].data, toks[2].data)),
                (new[] { new TokenStyle(TokenType.String) },
                    (sln, toks) => new DataStruct(MISValue.String, toks[0].data)),
                (new[] { new TokenStyle("false", TokenType.Keyword) },
                    (sln, toks) => new DataStruct(MISValue.Boolean, new byte[] { 0b0 })),
                (new[] { new TokenStyle("true", TokenType.Keyword) },
                    (sln, toks) => new DataStruct(MISValue.Boolean, new byte[] { 0b1 })),
                (new[] { new TokenStyle(TokenType.Keyword) },
                    (sln, toks) => new DataStruct(MISValue.Key, toks[0].data)),
                (new[] { new TokenStyle(TokenType.BoxBracket) },
                    (sln, toks) => new DataStruct(MISValue.Object, GenerateSolution(toks[0].tokens)))
            );

            compileStyle = new CompileStyle(
                (
                    new TokenStyle[]
                    {
                        new TokenStyle(TokenType.Keyword),
                        new TokenStyle(":", TokenType.Special),
                        new TokenStyle(TokenType.ToEnd)
                    },
                    (CompileStyleDelegate)Parameter
                ),
                (
                    new TokenStyle[]
                    {
                        new TokenStyle(TokenType.Brace),
                        new TokenStyle(TokenType.Keyword),
                        new TokenStyle(":", TokenType.Special),
                        new TokenStyle(TokenType.ToEnd)
                    },
                    (CompileStyleDelegate)Parameter
                )
            );
        }

        private static void Parameter(Solution sln, Token[] toks)
        {
            int from = 0;
            while (toks[from].type == TokenType.Brace) from++;
            Solution array = new Solution();
            Token[][] values = toks[(from+2)..].Split(new TokenStyle(",", TokenType.Special));
            for (int i = 0; i < values.Length; i++)
            {
                array.Add($"{i}", valueStyle.Parse(sln, values[i]));
            }
            sln.Add(toks[from].Text, new DataStruct(MISValue.Non, array, BitConverter.GetBytes(values.Length)));
        }

        public static MISObject Generate(string path) =>
            Generate(Compiler.GetTokens(File.ReadAllText(path)), path);

        public static MISObject Generate(Token[][] tokens, string path) =>
            GenerateFromSolution(GenerateSolution(tokens), path);

        public static MISObject GenerateFromSolution(Solution solution, string path)
        {
            Dictionary<string, object[]> parameters = new Dictionary<string, object[]>();
            object[] array;
            int i, length;
            foreach (var var in solution.VarArray())
            {
                length = BitConverter.ToInt32(var.Value.data);
                array = new object[length];
                for (i = 0; i < length; i++)
                    if (var.Value.sln.TryGet($"{i}", out DataStruct arrData))
                        array[i] = (MISValue)arrData.type switch
                        {
                            MISValue.Number => BitConverter.ToDouble(arrData.data),
                            MISValue.Range => new MISRange(BitConverter.ToDouble(arrData.data), BitConverter.ToDouble(arrData.data, 8)),
                            MISValue.String => Encoding.Unicode.GetString(arrData.data),
                            MISValue.Key => new MISKey(Encoding.Unicode.GetString(arrData.data)),
                            MISValue.Boolean => arrData.data[0] != 0,
                            MISValue.Object => GenerateFromSolution(arrData.sln, path)
                        };
                parameters[var.Key] = array;
            }
            return new MISObject(parameters, path);
        }

        public static Solution GenerateSolution(Token[][] tokens)
        {
            Solution solution = new Solution();
            Compiler.ParseStyle(solution, compileStyle, tokens);
            return solution;
        }

        public static MISValue ToMISType(this Type type)
        {
            if (type == typeof(double)) return MISValue.Number;
            if (type == typeof(MISRange)) return MISValue.Range;
            if (type == typeof(string)) return MISValue.String;
            if (type == typeof(MISKey)) return MISValue.Key;
            if (type == typeof(bool)) return MISValue.Boolean;
            if (type == typeof(MISObject)) return MISValue.Object;
            throw new NotSupportedException();
        }

        public static Dictionary<string, object[]> MISRequire(this Dictionary<string, object[]> value, string parameter, Delegate action)
        {
            ParameterInfo[] args = action.Method.GetParameters();
            if (value.TryGetValue(parameter, out object[] values)) {
                if (args.Length != values.Length) goto ERROR;
                for (int i = 0; i < args.Length; i++)
                    if (values[i].GetType().ToMISType() != args[i].ParameterType.ToMISType()) goto ERROR;
                action.DynamicInvoke(values);
                return value;
            }
            ERROR:;
            StringBuilder builder = new StringBuilder();
            builder.Append("Parameter \"").Append(parameter).Append("\" with arguments \"");
            for (int j = 0; j < args.Length; j++)
            {
                if (j != 0) builder.Append(", ");
                builder.Append(args[j].ParameterType.ToMISType().ToString());
            }
            throw new NotImplementedException(builder.Append("\" is not implemented.").ToString());
        }


        public static Dictionary<string, object[]> MISCheck(this Dictionary<string, object[]> value, string parameter, Delegate action)
        {
            if (value.TryGetValue(parameter, out object[] values))
            {
                ParameterInfo[] args = action.Method.GetParameters();
                if (args.Length != values.Length) return value;
                for (int i = 0; i < args.Length; i++)
                    if (values[i].GetType().ToMISType() != args[i].ParameterType.ToMISType()) return value;
                action.DynamicInvoke(values);
            }
            return value;
        }
    }

    public readonly struct MISKey
    {
        public readonly string value;
        public MISKey(string value)
        {
            this.value = value;
        }
    }

    public readonly struct MISRange
    {
        public readonly double from, to;
        public MISRange(double from, double to)
        {
            this.from = from;
            this.to = to;
        }
    }

    public readonly struct MISObject
    {
        public readonly string type, path, name;
        public readonly Dictionary<string, object[]> parameters;

        public MISObject(Dictionary<string, object[]> parameters, string path)
        {
            this.parameters = parameters;
            string type = "";
            Check("type", (MISKey key) => type = key.value);
            this.type = type;
            this.path = $"{Path.GetDirectoryName(path)}{Path.DirectorySeparatorChar}";
            this.name = Path.GetFileNameWithoutExtension(path);
        }

        public MISObject require(string parameter, IModMethod method, MISValue[] types)
        {
            if (parameters.TryGetValue(parameter, out object[] values))
            {
                if (types.Length != values.Length) goto ERROR;
                for (int i = 0; i < types.Length; i++)
                    if (values[i].GetType().ToMISType() != types[i]) goto ERROR;
                method.call(values);
                return this;
            }
            ERROR:;
            StringBuilder builder = new StringBuilder();
            builder.Append("Parameter \"").Append(parameter).Append("\" with arguments \"");
            for (int j = 0; j < types.Length; j++)
            {
                if (j != 0) builder.Append(", ");
                builder.Append(types[j].ToString());
            }
            throw new NotImplementedException(builder.Append("\" is not implemented.").ToString());
        }

        public MISObject check(string parameter, IModMethod method, MISValue[] types)
        {
            if (parameters.TryGetValue(parameter, out object[] values))
            {
                if (types.Length != values.Length) return this;
                for (int i = 0; i < types.Length; i++)
                    if (values[i].GetType().ToMISType() != types[i]) return this;
                method.call(values);
            }
            return this;
        }

        public bool has(string parameter) => parameters.ContainsKey(parameter);

        public object[] get(string parameter) => parameters[parameter];

        public MISObject Require(string parameter, Delegate action)
        {
            parameters.MISRequire(parameter, action);
            return this;
        }

        public MISObject Check(string parameter, Delegate action)
        {
            parameters.MISCheck(parameter, action);
            return this;
        }
    }
}
