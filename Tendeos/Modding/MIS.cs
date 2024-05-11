using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using LZ4;
using Tendeos.Utils;
using Va;

namespace Tendeos.Modding
{
    public enum MISValue { Object, Number, Range, String, Key, Boolean, Parameter, Array, Non }

    public static class MIS
    {
        private static readonly ValueStyle valueStyle;
        private static readonly CompileStyle compileStyle;
        private static readonly ExpressionStyle expressionStyle;

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
                    (sln, toks) => {
                        if (sln.TryGet(toks[0].Text, out DataStruct data)) return data.sln.FlatGet("0");
                        return new DataStruct(MISValue.Key, toks[0].data);
                    }),
                (new[] { new TokenStyle(TokenType.Keyword), new TokenStyle(".", TokenType.Special), new TokenStyle(TokenType.ToEnd) }, GetFromObject),
                (new[] { new TokenStyle(TokenType.BoxBracket) },
                    (sln, toks) => {
                        if (toks[0].tokens.Length > 1 || (toks[0].tokens[0].Length > 1 && toks[0].tokens[0][1].type == TokenType.Special && toks[0].tokens[0][1].Text == ":"))
                            return new DataStruct(MISValue.Object, GenerateSolution(toks[0].tokens, sln));
                        else
                            return GenerateArrayData(toks[0].tokens[0], sln);
                    }),
                (new[] { new TokenStyle(TokenType.Brace), new TokenStyle(TokenType.ToEnd) },
                    (sln, toks) => valueStyle.Parse(sln, toks[1..]))
            );

            expressionStyle = new ExpressionStyle(valueStyle,
                new (TokenStyle, ExpressionStyleDelegate)[]
                {
                    (
                        new TokenStyle("+", TokenType.Special),
                        (sln, line, l, r) =>
                        {
                            MISValue lt = (MISValue)l.type;
                            MISValue rt = (MISValue)r.type;
                            if ((lt & rt) == MISValue.Number)
                            {
                                return new DataStruct(MISValue.Number, BitConverter.GetBytes(l.Num + r.Num));
                            }
                            if ((lt & rt) == MISValue.String)
                            {
                                return new DataStruct(MISValue.Number, l.data, r.data);
                            }
                            if (lt == MISValue.String && rt == MISValue.Key)
                            {
                                return new DataStruct(MISValue.String, l.data, r.data);
                            }
                            if (lt == MISValue.Key && rt == MISValue.String)
                            {
                                return new DataStruct(MISValue.String, l.data, r.data);
                            }
                            if (lt == MISValue.String && rt == MISValue.Number)
                            {
                                return new DataStruct(MISValue.String, l.data, Encoding.Unicode.GetBytes($"{r.Num}"));
                            }
                            if (lt == MISValue.Number && rt == MISValue.String)
                            {
                                return new DataStruct(MISValue.String, Encoding.Unicode.GetBytes($"{l.Num}"), r.data);
                            }
                            if (lt == MISValue.Key && rt == MISValue.Number)
                            {
                                return new DataStruct(MISValue.String, l.data, Encoding.Unicode.GetBytes($"{r.Num}"));
                            }
                            if (lt == MISValue.Number && rt == MISValue.Key)
                            {
                                return new DataStruct(MISValue.String, Encoding.Unicode.GetBytes($"{l.Num}"), r.data);
                            }
                            if ((lt & rt) == MISValue.Array)
                            {
                                int i = 0;
                                Solution arr = new Solution(sln);
                                foreach ((string _, DataStruct value) in l.sln.VarArray()) arr.Add($"{i++}", value);
                                foreach ((string _, DataStruct value) in r.sln.VarArray()) arr.Add($"{i++}", value);
                                return new DataStruct(MISValue.Array, arr, BitConverter.GetBytes(i+1));
                            }
                            throw new VaException(line, $"Invalid + operation between {lt} and {rt}.");
                        }
                    ),
                    (
                        new TokenStyle("-", TokenType.Special),
                        (sln, line, l, r) =>
                        {
                            MISValue lt = (MISValue)l.type;
                            MISValue rt = (MISValue)r.type;
                            if ((lt & rt) == MISValue.Number)
                            {
                                return new DataStruct(MISValue.Number, BitConverter.GetBytes(l.Num - r.Num));
                            }
                            throw new VaException(line, $"Invalid - operation between {lt} and {rt}.");
                        }
                    ),
                    (
                        new TokenStyle("*", TokenType.Special),
                        (sln, line, l, r) =>
                        {
                            MISValue lt = (MISValue)l.type;
                            MISValue rt = (MISValue)r.type;
                            if ((lt & rt) == MISValue.Number)
                            {
                                return new DataStruct(MISValue.Number, BitConverter.GetBytes(l.Num * r.Num));
                            }
                            if (lt == MISValue.String && rt == MISValue.Number)
                            {
                                int mul = (int)r.Num;
                                byte[] data = new byte[l.data.Length * mul];
                                for (int i = 0; i < mul; i++)
                                    Array.Copy(l.data, 0, data, l.data.Length * i, l.data.Length);
                                return new DataStruct(MISValue.String, data);
                            }
                            throw new VaException(line, $"Invalid * operation between {lt} and {rt}.");
                        }
                    ),
                    (
                        new TokenStyle("/", TokenType.Special),
                        (sln, line, l, r) =>
                        {
                            MISValue lt = (MISValue)l.type;
                            MISValue rt = (MISValue)r.type;
                            if ((lt & rt) == MISValue.Number)
                            {
                                return new DataStruct(MISValue.Number, BitConverter.GetBytes(l.Num / r.Num));
                            }
                            throw new VaException(line, $"Invalid * operation between {lt} and {rt}.");
                        }
                    )
                }
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
                        new TokenStyle(TokenType.ToEnd)
                    },
                    (CompileStyleDelegate)((Solution sln, Token[] toks) =>
                    {
                        compileStyle.ParseLine(sln, toks[1..]);
                    })
                )
            );
        }

        private static void Parameter(Solution sln, Token[] toks)
        {
            int from = 0;
            while (toks[from].type == TokenType.Brace) from++;
            Solution array = new Solution(sln);
            Token[][] values = toks[(from+2)..].Split(new TokenStyle(",", TokenType.Special));
            for (int i = 0; i < values.Length; i++)
            {
                array.Add($"{i}", expressionStyle.Parse(sln, values[i]));
            }
            sln.Add(toks[from].Text, new DataStruct(MISValue.Parameter, array, BitConverter.GetBytes(values.Length)));
        }

        public static MISObject Generate(string path) =>
            Generate(Compiler.GetTokens(File.ReadAllText(path), false), path);

        public static MISObject GenerateVirtual(string text, string virtualPath) =>
            Generate(Compiler.GetTokens(text, false), virtualPath);

        private static MISObject Generate(Token[][] tokens, string path) =>
            GenerateFromSolution(GenerateSolution(tokens), path);

        private static MISObject GenerateFromSolution(Solution solution, string path)
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
                            MISValue.Object => GenerateFromSolution(arrData.sln, path),
                            MISValue.Array => GenerateArrayFromDataStruct(arrData, path),
                            MISValue.Non => null
                        };
                parameters[var.Key] = array;
            }
            return new MISObject(parameters, path);
        }

        private static MISArray GenerateArrayFromDataStruct(DataStruct dataStruct, string path)
        {
            object[] array = new object[BitConverter.ToInt32(dataStruct.data, 0)];
            for (int i = 0; i < array.Length; i++)
                if (dataStruct.sln.TryGet($"{i}", out DataStruct arrData))
                    array[i] = (MISValue)arrData.type switch
                    {
                        MISValue.Number => BitConverter.ToDouble(arrData.data),
                        MISValue.Range => new MISRange(BitConverter.ToDouble(arrData.data), BitConverter.ToDouble(arrData.data, 8)),
                        MISValue.String => Encoding.Unicode.GetString(arrData.data),
                        MISValue.Key => new MISKey(Encoding.Unicode.GetString(arrData.data)),
                        MISValue.Boolean => arrData.data[0] != 0,
                        MISValue.Object => GenerateFromSolution(arrData.sln, path),
                        MISValue.Array => GenerateArrayFromDataStruct(arrData, path),
                        MISValue.Non => null
                    };
            return new MISArray(array);
        }

        private static Solution GenerateSolution(Token[][] tokens, params Solution[] parents)
        {
            Solution solution = new Solution(parents);
            Compiler.ParseStyle(solution, compileStyle, tokens);
            return solution;
        }

        private static DataStruct GenerateArrayData(Token[] toks, params Solution[] parents)
        {
            Solution array = new Solution(parents);
            Token[][] values = toks.Split(new TokenStyle(",", TokenType.Special));
            byte[] data = BitConverter.GetBytes(values.Length);
            for (int i = 0; i < values.Length; i++)
                array.Add($"{i}", expressionStyle.Parse(array, values[i]));
            return new DataStruct(MISValue.Array, array, data);
        }

        private static DataStruct GetFromObject(Solution sln, Token[] toks)
        {
            DataStruct dataStruct = new DataStruct(MISValue.Non, sln);
            string path = "";
            Token next;
            string keyword;
            int offset = 0, i;
            while (offset < toks.Length)
            {
                next = toks[offset];
                switch ((MISValue)dataStruct.type)
                {
                    case MISValue.Non:
                        if (next.type == TokenType.Keyword)
                        {
                            keyword = next.Text;
                            if (dataStruct.sln.TryGet(keyword, out dataStruct)) path += $"{keyword}";
                            else throw new VaException(next.line, $"{path} doesn't include \"{keyword}\" parameter.");
                        }
                        else throw new VaException(next.line, "Object expected to getting element by name.");
                        break;
                    case MISValue.Array:
                    case MISValue.Parameter:
                        if (next.type == TokenType.Number)
                        {
                            i = (int)BitConverter.ToDouble(next.data);
                            if (dataStruct.sln.FlatTryGet($"{i}", out dataStruct)) path += $".{i}";
                            else throw new VaException(next.line, $"Index {i} is out {path} of bounds.");
                        }
                        else if (next.type == TokenType.Keyword)
                        {
                            dataStruct = dataStruct.sln.FlatGet($"0");
                            offset -= 2;
                        }
                        else throw new VaException(next.line, "Array expected to getting element by number.");
                        break;
                    case MISValue.Object:
                        if (next.type == TokenType.Keyword)
                        {
                            keyword = next.Text;
                            if (dataStruct.sln.FlatTryGet(keyword, out dataStruct)) path += $".{keyword}";
                            else throw new VaException(next.line, $"{path} doesn't include \"{keyword}\" parameter.");
                        }
                        else throw new VaException(next.line, "Object expected to getting element by name.");
                        break;
                    default:
                        throw new VaException(next.line, $"Trying to get value from {(MISValue)dataStruct.type}.");
                }
                if (++offset >= toks.Length) continue;
                next = toks[offset];
                if (next.type != TokenType.Special && next.Text != "." && next.Text != ":-")
                {
                    throw new VaException(next.line, $"After {path} invalid token, perhaps you mean '.' or ':-'?");
                }
                offset++;
            }
            if (dataStruct.type == (byte)MISValue.Parameter) dataStruct = dataStruct.sln.FlatGet($"0");
            return dataStruct;
        }

        public static MISValue ToMISType(this Type type)
        {
            if (type == typeof(double)) return MISValue.Number;
            if (type == typeof(MISRange)) return MISValue.Range;
            if (type == typeof(string)) return MISValue.String;
            if (type == typeof(MISKey)) return MISValue.Key;
            if (type == typeof(bool)) return MISValue.Boolean;
            if (type == typeof(MISObject)) return MISValue.Object;
            if (type == typeof(MISArray)) return MISValue.Array;
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

        public static void CompileTo(this object obj, ByteBuffer buffer)
        {
            MISValue type = obj.GetType().ToMISType();
            buffer.Append((byte)type);
            switch (type)
            {
                case MISValue.Object: ((MISObject)obj).CompileTo(buffer); break;
                case MISValue.Number: buffer.Append((double)obj); break;
                case MISValue.Range:
                    MISRange range = (MISRange)obj;
                    buffer.Append(range.from).Append(range.to);
                    break;
                case MISValue.String: buffer.Append((string)obj); break;
                case MISValue.Key: buffer.Append(((MISKey)obj).value); break;
                case MISValue.Boolean: buffer.Append((bool)obj); break;
                case MISValue.Array: ((MISArray)obj).CompileTo(buffer); break;
            }
        }

        public static object Decompile(ByteBuffer buffer)
        {
            MISValue type = (MISValue)buffer.ReadByte();
            switch (type)
            {
                case MISValue.Object: return new MISObject(buffer);
                case MISValue.Number: return buffer.ReadDouble();
                case MISValue.Range: return new MISRange(buffer.ReadDouble(), buffer.ReadDouble());
                case MISValue.String: return buffer.ReadString();
                case MISValue.Key: return new MISKey(buffer.ReadString());
                case MISValue.Boolean: return buffer.ReadBool();
                case MISValue.Array: return MISArray.Decompile(buffer);
            }
            return null;
        }

        public static MISObject Decompile(string path)
        {
            using FileStream stream = File.OpenRead(path);
            using LZ4Stream LZ4Stream = new LZ4Stream(stream, LZ4StreamMode.Decompress);
            return new MISObject(new ByteBuffer(LZ4Stream));
        }

        public static MISObject Decompile(byte[] data)
        {
            using MemoryStream stream = new MemoryStream(data);
            using LZ4Stream LZ4Stream = new LZ4Stream(stream, LZ4StreamMode.Decompress);
            return new MISObject(new ByteBuffer(LZ4Stream));
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
        private readonly Dictionary<string, object[]> parameters;

        public MISObject(Dictionary<string, object[]> parameters, string path)
        {
            this.parameters = parameters;
            string type = "";
            Check("type", (MISKey key) => type = key.value);
            this.type = type;
            this.path = $"{Path.GetDirectoryName(path)}{Path.DirectorySeparatorChar}";
            name = Path.GetFileNameWithoutExtension(path);
        }

        public MISObject(ByteBuffer buffer)
        {
            buffer.Read(out name).Read(out path);
            parameters = new Dictionary<string, object[]>();
            int count = buffer.ReadInt();
            int j;
            object[] objects;
            for (int i = 0; i < count; i++)
            {
                buffer.Read(out string key);
                buffer.Read(out int length);
                objects = new object[length];
                for (j = 0; j < length; j++)
                {
                    objects[j] = MIS.Decompile(buffer);
                }
                parameters.Add(key, objects);
            }
        }

        public byte[] Compile()
        {
            using MemoryStream stream = new MemoryStream();
            using LZ4Stream LZ4Stream = new LZ4Stream(stream, LZ4StreamMode.Compress);
            ByteBuffer buffer = new ByteBuffer(LZ4Stream).Append(name).Append(path).Append(parameters.Count);
            foreach (var item in parameters)
            {
                buffer.Append(item.Key);
                buffer.Append(item.Value.Length);
                foreach (var obj in item.Value)
                {
                    obj.CompileTo(buffer);
                }
            }
            return buffer;
        }

        public void CompileTo(Stream stream)
        {
            using LZ4Stream LZ4Stream = new LZ4Stream(stream, LZ4StreamMode.Compress);
            ByteBuffer buffer = new ByteBuffer(LZ4Stream).Append(name).Append(path).Append(parameters.Count);
            foreach (var item in parameters)
            {
                buffer.Append(item.Key);
                buffer.Append(item.Value.Length);
                foreach (var obj in item.Value)
                {
                    obj.CompileTo(buffer);
                }
            }
        }
        

        public byte[] CompileTo(ByteBuffer buffer)
        {
            buffer.Append(name).Append(path).Append(parameters.Count);
            foreach (var item in parameters)
            {
                buffer.Append(item.Key);
                buffer.Append(item.Value.Length);
                foreach (var obj in item.Value)
                {
                    obj.CompileTo(buffer);
                }
            }
            return buffer;
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

    public readonly struct MISArray
    {
        private readonly object[] elements;

        public MISArray(object[] elements)
        {
            this.elements = elements;
        }

        public void CompileTo(ByteBuffer buffer)
        {
            buffer.Append(Length);
            foreach (var item in elements)
            {
                item.CompileTo(buffer);
            }
        }

        public static object Decompile(ByteBuffer buffer)
        {
            object[] elements = new object[buffer.ReadInt()];
            for (int i = 0; i < elements.Length; i++)
            {
                elements[i] = MIS.Decompile(buffer);
            }
            return new MISArray(elements);
        }

        public int Length => elements.Length;

        public T Get<T>(int i) => (T)elements[i];

        public object get(int i) => elements[i];
    }
}
