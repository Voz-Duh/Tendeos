using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using LZ4;
using Microsoft.VisualBasic;
using Tendeos.Utils;
//using Va;

namespace Tendeos.Modding
{
    /// <summary>
    /// Defines MIS value types.
    /// </summary>
    public enum MISValue
    {
        Object,
        Number,
        Range,
        String,
        Key,
        Boolean,
        Parameter,
        Array,
        Non
    }

    /// <summary>
    /// MIS manager class.
    /// Uses for parsing and generating MIS objects.
    /// </summary>
    public static class MIS
    {
        public static readonly (string, string, string, (string, string)[])[] Literals =
        {
            (
                "\"", "\"", "\\",
                new []
                {
                    ("n", "\n"),
                    ("t", "\t"),
                    ("a", "\a"),
                    ("r", "\r"),
                    ("b", "\b"),
                    ("v", "\v"),
                    ("f", "\f"),
                    ("\\", "\\")
                }
            )
        };

        public static readonly (string, string)[] Comments =
        {
            ("{", "}")
        };

        public static readonly (string, string)[] Groups =
        {
            ("[", "]")
        };
        
        /// <summary>
        /// Generates MIS object from file.
        /// </summary>
        /// <param name="path">Path to source file.</param>
        /// <returns>MIS object parsed from source file.</returns>
        public static MISObject Generate(string path) =>
            Generate(File.ReadAllText(path), path);

        /// <summary>
        /// Generates MIS object from virtual file.
        /// </summary>
        /// <param name="code">Source code text.</param>
        /// <param name="virtualPath">Virtual path to source file.</param>
        /// <returns>MIS object parsed from source text.</returns>
        public static MISObject GenerateVirtual(string code, string virtualPath) =>
            Generate(code, virtualPath);


        private static MISObject Generate(string code, string path)
        {
            TokenManager reader = new(TextParser.CreateParser(code, Literals, Comments, Groups));
            
            return ParseObject(reader, path, null);
        }

        private static object[] ParseValues(TokenManager reader, string path, Dictionary<string, object[]> globalParameters)
        {
            var parts = reader.Split(TokenType.SPECIAL, ",");
            object[] values = new object[parts.Count];
            for (int i = 0; i < values.Length; i++)
            {
                var (coline, tokens) = parts[i];
                values[i] = ParseValue(new TokenManager(reader, tokens), coline, path, globalParameters);
            }

            return values;
        }

        private static object ParseValue(TokenManager reader, (int, int) coline, string path, Dictionary<string, object[]> globalParameters)
        {
            object value = null;
            while (reader.IMove)
            {
                if (value != null)
                {
                    throw reader.CreateExceptionOnCurrent("Unexpected token");
                }

                if (reader.Current == TokenType.KEYWORD)
                {
                    if (globalParameters.TryGetValue(reader.Current.Value, out object[] objects))
                    {
                        if (objects.Length == 1)
                            value = objects[0];
                        else
                            value = objects;
                        
                        while (reader.IMove)
                        {
                            if (reader.Current == TokenType.INTEGER)
                            {
                                if (value is object[] objs)
                                    value = objs[int.Parse(reader.Current.Value)];
                                else if (value is MISArray mis)
                                    value = mis.get(int.Parse(reader.Current.Value));
                                else
                                    throw reader.CreateExceptionOnCurrent("Unexpected integer extraction");
                            }
                            else if (reader.Current == TokenType.KEYWORD)
                            {
                                if (value is MISObject obj)
                                {
                                    objects = obj.get(reader.Current.Value);
                                    if (objects.Length == 1)
                                        value = objects[0];
                                    else
                                        value = objects;
                                }
                                else
                                    throw reader.CreateExceptionOnCurrent("Unexpected keyword extraction");
                            }
                            else
                            {
                                _ = reader.IMoveBack;
                                break;
                            }
                        }

                        
                    }
                    else value = new MISKey(reader.Current.Value);
                }
                else if (reader.Current == TokenType.LITERAL)
                    value = reader.Current.Text;
                else if (reader.Current == TokenType.INTEGER)
                    value = double.Parse(reader.Current.Value);
                else if (reader.Current == TokenType.FLOATING)
                    value = double.Parse(reader.Current.Value);
                else if (reader.Current == (TokenType.GROUP, "["))
                {
                    TokenManager inside = new(reader, reader.Current.Tokens);
                    if (!inside.IMove) value = new MISArray(Array.Empty<object>());
                    else if (inside.IMove && inside.Current == (TokenType.SPECIAL, ":"))
                    {
                        inside.ToStart();
                        value = ParseObject(inside, path, globalParameters);
                    }
                    else
                    {
                        inside.ToStart();
                        value = new MISArray(ParseValues(inside, path, globalParameters));
                    }
                }
                else
                {
                    throw reader.CreateExceptionOnCurrent("Invalid value");
                }
            }
            
            if (value == null)
            {
                throw reader.CreateException(coline, "Invalid value");
            }

            return value;
        }

        private static MISObject ParseObject(TokenManager reader, string path, Dictionary<string, object[]>? globalParameters)
        {
            Dictionary<string, object[]> parameters = new Dictionary<string, object[]>();
            globalParameters ??= parameters;
            
            while (reader.IMove)
            {
                if (reader.Current != TokenType.KEYWORD)
                    throw reader.CreateExceptionOnCurrent("Unexpected token");

                (int, int) nameColine = reader.Current.Coline;

                string name = reader.Current.Value;
                
                if (reader.INext != (TokenType.SPECIAL, ":"))
                    throw reader.CreateExceptionOnCurrent("Unexpected token, \":\" is missed");

                int from = reader.Caret;
                
                if (!reader.MoveTo(new Token(TokenType.SPECIAL, ";")))
                    throw reader.CreateException(reader[from].Coline, "Missed \";\" after");

                object[] values = ParseValues(new TokenManager(reader, reader[(from + 1)..reader.Caret]), path, globalParameters);

                if (!parameters.TryAdd(name, values))
                    throw reader.CreateException(nameColine, $"Parameter {name} is already exist");
            }

            return new MISObject(parameters, path);
        }
        
        public static MISValue ToMISType(this Type type)
        {
            if (type == typeof(double)) return MISValue.Number;
            if (type == typeof(float)) return MISValue.Number;
            if (type == typeof(Half)) return MISValue.Number;
            if (type == typeof(long)) return MISValue.Number;
            if (type == typeof(ulong)) return MISValue.Number;
            if (type == typeof(int)) return MISValue.Number;
            if (type == typeof(uint)) return MISValue.Number;
            if (type == typeof(byte)) return MISValue.Number;
            if (type == typeof(sbyte)) return MISValue.Number;
            if (type == typeof(MISRange)) return MISValue.Range;
            if (type == typeof(string)) return MISValue.String;
            if (type == typeof(MISKey)) return MISValue.Key;
            if (type == typeof(bool)) return MISValue.Boolean;
            if (type == typeof(MISObject)) return MISValue.Object;
            if (type == typeof(MISArray)) return MISValue.Array;
            throw new NotSupportedException();
        }

        /// <summary>
        /// Check if the parameter with the same values that the action arguments is in the dictionary and invoke the action.
        /// </summary>
        /// <param name="value">Dictionary where to search.</param>
        /// <param name="parameter">Parameter name.</param>
        /// <param name="action">Action to invoke if the parameter is in the dictionary.</param>
        /// <returns>Returns the dictionary for chaining.</returns> 
        /// <remarks>
        /// If the parameter is not in the dictionary, an exception is thrown.
        /// </remarks>
        public static Dictionary<string, object[]> MISRequire(this Dictionary<string, object[]> value, string parameter,
            Delegate action)
        {
            ParameterInfo[] args = action.Method.GetParameters();
            if (value.TryGetValue(parameter, out object[] values))
            {
                if (args.Length != values.Length) goto ERROR;
                for (int i = 0; i < args.Length; i++)
                    if (values[i].GetType().ToMISType() != args[i].ParameterType.ToMISType())
                        goto ERROR;
                action.DynamicInvoke(values);
                return value;
            }

            ERROR: ;
            StringBuilder builder = new();
            builder.Append("Parameter \"").Append(parameter).Append("\" with arguments \"");
            for (int j = 0; j < args.Length; j++)
            {
                if (j != 0) builder.Append(", ");
                builder.Append(args[j].ParameterType.ToMISType().ToString());
            }

            throw new NotImplementedException(builder.Append("\" is not implemented.").ToString());
        }

        /// <summary>
        /// Check if the parameter with the same values that the action arguments is in the dictionary and invoke the action.
        /// </summary>
        /// <param name="value">Dictionary where to search.</param>
        /// <param name="parameter">Parameter name.</param>
        /// <param name="action">Action to invoke if the parameter is in the dictionary.</param>
        /// <returns>Returns the dictionary for chaining.</returns> 
        public static bool MISCheck(this Dictionary<string, object[]> value, string parameter,
            Delegate action)
        {
            if (value.TryGetValue(parameter, out object[] values))
            {
                ParameterInfo[] args = action.Method.GetParameters();
                if (args.Length != values.Length) return false;
                for (int i = 0; i < args.Length; i++)
                    if (values[i].GetType().ToMISType() != args[i].ParameterType.ToMISType())
                        return false;
                action.DynamicInvoke(values);
                return true;
            }

            return false;
        }

        internal static void CompileTo(this object obj, ByteBuffer buffer)
        {
            MISValue type = obj.GetType().ToMISType();
            buffer.Append((byte) type);
            switch (type)
            {
                case MISValue.Object:
                    ((MISObject) obj).CompileTo(buffer);
                    break;
                case MISValue.Number:
                    buffer.Append((double) obj);
                    break;
                case MISValue.Range:
                    MISRange range = (MISRange) obj;
                    buffer.Append(range.from).Append(range.to);
                    break;
                case MISValue.String:
                    buffer.Append((string) obj);
                    break;
                case MISValue.Key:
                    buffer.Append(((MISKey) obj).value);
                    break;
                case MISValue.Boolean:
                    buffer.Append((bool) obj);
                    break;
                case MISValue.Array:
                    ((MISArray) obj).CompileTo(buffer);
                    break;
            }
        }

        internal static object Decompile(ByteBuffer buffer)
        {
            MISValue type = (MISValue) buffer.ReadByte();
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

        /// <summary>
        /// Decompile a MISObject from a file on disk.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>The MISObject.</returns>
        public static MISObject Decompile(string path)
        {
            using FileStream stream = File.OpenRead(path);
            using LZ4Stream LZ4Stream = new LZ4Stream(stream, LZ4StreamMode.Decompress);
            return new MISObject(new ByteBuffer(LZ4Stream));
        }

        /// <summary>
        /// Decompile a MISObject from a byte array.
        /// </summary>
        /// <param name="data">The byte array.</param>
        /// <returns>The MISObject.</returns>
        public static MISObject Decompile(byte[] data)
        {
            using MemoryStream stream = new MemoryStream(data);
            using LZ4Stream LZ4Stream = new LZ4Stream(stream, LZ4StreamMode.Decompress);
            return new MISObject(new ByteBuffer(LZ4Stream));
        }
    }

    /// <summary>
    /// <b>MIS keyword.</b>
    /// <br><br>Example: <code>value: keyword;</code></br></br>
    /// </summary>
    public readonly struct MISKey
    {
        /// <summary>
        /// The value of the MISKey.
        /// </summary>
        /// <remarks>
        /// This is a read-only property and should not be modified directly.
        /// </remarks>
        public readonly string value;

        internal MISKey(string value)
        {
            this.value = value;
        }
    }

    /// <summary>
    /// <b>MIS range.</b>
    /// <br><br>Example: <code>value: 0::10;</code></br></br>
    /// </summary>
    public readonly struct MISRange
    {
        /// <summary>
        /// The start value of the range.
        /// </summary>
        /// <remarks>
        /// This is a read-only property and should not be modified directly.
        /// </remarks>
        public readonly double from;

        /// <summary>
        /// The end value of the range.
        /// </summary>
        /// <remarks>
        /// This is a read-only property and should not be modified directly.
        /// </remarks>
        public readonly double to;

        internal MISRange(double from, double to)
        {
            this.from = from;
            this.to = to;
        }
    }

    /// <summary>
    /// <b>MIS object.</b>
    /// <br><br>Example: <code>value: [<br> value: something;<br>];</br></br></code></br></br>
    /// </summary>
    public readonly struct MISObject
    {
        public readonly string type, path, name;
        private readonly Dictionary<string, object[]> parameters;

        internal MISObject(Dictionary<string, object[]> parameters, string path)
        {
            this.parameters = parameters;

            this.path = $"{Path.GetDirectoryName(path)}{Path.DirectorySeparatorChar}";

            name = Path.GetFileNameWithoutExtension(path);

            string type = this.type = "";
            Check("type", (MISKey key) => type = key.value);
            this.type = type;
        }

        internal MISObject(ByteBuffer buffer)
        {
            type = "";
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

        /// <summary>
        /// Compiles the object into a byte array.
        /// </summary>
        /// <returns>The compiled byte array.</returns>
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

        /// <summary>
        /// Compiles the object into a byte array and writes it to the provided stream.
        /// </summary>
        /// <param name="stream">The stream to write the compiled data to.</param>
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

        /// <summary>
        /// Compiles the object into a byte array using the provided ByteBuffer.
        /// </summary>
        /// <param name="buffer">The ByteBuffer to append the compiled data to.</param>
        /// <returns>The compiled byte array.</returns>
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

        /// <summary>
        /// Check if the parameter with the same values that the array types is in the object and invoke the method.
        /// </summary>
        /// <param name="parameter">Parameter name.</param>
        /// <param name="method">Method to invoke if the parameter is in the object.</param>
        /// <param name="types"> Parameter values types. </param>
        /// <returns>Returns the object for chaining.</returns> 
        /// <remarks>
        /// If the parameter is not in the object, an exception is thrown.
        /// </remarks>
        public MISObject require(string parameter, IModMethod method, MISValue[] types)
        {
            if (parameters.TryGetValue(parameter, out object[] values))
            {
                if (types.Length != values.Length) goto ERROR;
                for (int i = 0; i < types.Length; i++)
                    if (values[i].GetType().ToMISType() != types[i])
                        goto ERROR;
                method.call(values);
                return this;
            }

            ERROR: ;
            StringBuilder builder = new StringBuilder();
            builder.Append("Parameter \"").Append(parameter).Append("\" with arguments \"");
            for (int j = 0; j < types.Length; j++)
            {
                if (j != 0) builder.Append(", ");
                builder.Append(types[j].ToString());
            }

            throw new NotImplementedException(builder.Append("\" is not implemented.").ToString());
        }

        /// <summary>
        /// Check if the parameter with the same values that the array types is in the object and invoke the method.
        /// </summary>
        /// <param name="parameter">Parameter name.</param>
        /// <param name="method">Method to invoke if the parameter is in the object.</param>
        /// <param name="types"> Parameter values types. </param>
        /// <returns>Returns the object for chaining.</returns> 
        public MISObject check(string parameter, IModMethod method, MISValue[] types)
        {
            if (parameters.TryGetValue(parameter, out object[] values))
            {
                if (types.Length != values.Length) return this;
                for (int i = 0; i < types.Length; i++)
                    if (values[i].GetType().ToMISType() != types[i])
                        return this;
                method.call(values);
            }

            return this;
        }

        /// <summary>
        /// Check if the object has a parameter.
        /// </summary>
        /// <param name="parameter"> Specified parameter name </param>
        /// <returns> <c>true</c> if the object has the parameter; otherwise, <c>false</c>. </returns>
        public bool has(string parameter) => parameters.ContainsKey(parameter);

        /// <summary>
        /// Get parameter from object.
        /// </summary>
        /// <param name="parameter"> Specified parameter name. </param>
        /// <returns> Array of parameter values. </returns>
        public object[] get(string parameter) => parameters[parameter];

        public ChainInstance Chain() => new(this);

        /// <summary>
        /// Check if the parameter with the same values that the action arguments is in the object and invoke the action.
        /// </summary>
        /// <param name="parameter">Parameter name.</param>
        /// <param name="action">Action to invoke if the parameter is in the object.</param>
        /// <returns>Returns the object for chaining.</returns> 
        /// <remarks>
        /// If the parameter is not in the object, an exception is thrown.
        /// </remarks>
        public MISObject Require(string parameter, Delegate action)
        {
            parameters.MISRequire(parameter, action);
            return this;
        }

        /// <summary>
        /// Check if the parameter with the same values that the action arguments is in the object and invoke the action.
        /// </summary>
        /// <param name="parameter">Parameter name.</param>
        /// <param name="action">Action to invoke if the parameter is in the object.</param>
        /// <returns>Returns the object for chaining.</returns> 
        public bool Check(string parameter, Delegate action)
        {
            return parameters.MISCheck(parameter, action);
        }

        private (string, object[])[] GetAllParametersAs(Type[] types) =>
            parameters.Select(e =>
                {
                    if (e.Value.Length != types.Length)
                        throw new InvalidDataException($"Invalid value type for parameter '{e.Key}'. Excepted {string.Join(", ", types.Select(e => e.ToMISType()))} instead of {string.Join(", ", e.Value.Select(e => e.GetType().ToMISType()))}");

                    object[] values = new object[types.Length];
                    for (int i = 0; i < types.Length; i++)
                        if (e.Value[i].GetType().ToMISType() != types[i].ToMISType())
                            throw new InvalidDataException($"Invalid value type for parameter '{e.Key}'. Excepted {string.Join(", ", types.Select(e => e.ToMISType()))} instead of {string.Join(", ", e.Value.Select(e => e.GetType().ToMISType()))}");
                        else
                        {
                            if (types[i].ToMISType() == MISValue.Number)
                                values[i] = Convert.ChangeType(e.Value[i], types[i]);
                            else
                                values[i] = e.Value[i];
                        }

                    return (e.Key, values);
                }
            ).ToArray();

        public (string, T)[] GetAllParametersAs<T>() =>
            GetAllParametersAs(new[] { typeof(T) })
                .Select(e => (e.Item1, (T)e.Item2[0]))
                .ToArray();

        public (string, T0, T1)[] GetAllParametersAs<T0, T1>() =>
            GetAllParametersAs(new[] { typeof(T0), typeof(T1) })
                .Select(e => (e.Item1, (T0)e.Item2[0], (T1)e.Item2[1]))
                .ToArray();

        public (string, T0, T1, T2)[] GetAllParametersAs<T0, T1, T2>() =>
            GetAllParametersAs(new[] { typeof(T0), typeof(T1), typeof(T2) })
                .Select(e => (e.Item1, (T0)e.Item2[0], (T1)e.Item2[1], (T2)e.Item2[2]))
                .ToArray();

        public (string, T0, T1, T2, T3)[] GetAllParametersAs<T0, T1, T2, T3>() =>
            GetAllParametersAs(new[] { typeof(T0), typeof(T1), typeof(T2), typeof(T3) })
                .Select(e => (e.Item1, (T0)e.Item2[0], (T1)e.Item2[1], (T2)e.Item2[2], (T3)e.Item2[3]))
                .ToArray();

        public (string, T0, T1, T2, T3, T4)[] GetAllParametersAs<T0, T1, T2, T3, T4>() =>
            GetAllParametersAs(new[] { typeof(T0), typeof(T1), typeof(T2), typeof(T3), typeof(T4) })
                .Select(e => (e.Item1, (T0)e.Item2[0], (T1)e.Item2[1], (T2)e.Item2[2], (T3)e.Item2[3], (T4)e.Item2[4]))
                .ToArray();

        public (string, T0, T1, T2, T3, T4, T5)[] GetAllParametersAs<T0, T1, T2, T3, T4, T5>() =>
            GetAllParametersAs(new[] { typeof(T0), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) })
                .Select(e => (e.Item1, (T0)e.Item2[0], (T1)e.Item2[1], (T2)e.Item2[2], (T3)e.Item2[3], (T4)e.Item2[4], (T5)e.Item2[5]))
                .ToArray();

        public (string, T0, T1, T2, T3, T4, T5, T6)[] GetAllParametersAs<T0, T1, T2, T3, T4, T5, T6>() =>
            GetAllParametersAs(new[] { typeof(T0), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) })
                .Select(e => (e.Item1, (T0)e.Item2[0], (T1)e.Item2[1], (T2)e.Item2[2], (T3)e.Item2[3], (T4)e.Item2[4], (T5)e.Item2[5], (T6)e.Item2[6]))
                .ToArray();

        public (string, T0, T1, T2, T3, T4, T5, T6, T7)[] GetAllParametersAs<T0, T1, T2, T3, T4, T5, T6, T7>() =>
            GetAllParametersAs(new[] { typeof(T0), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7) })
                .Select(e => (e.Item1, (T0)e.Item2[0], (T1)e.Item2[1], (T2)e.Item2[2], (T3)e.Item2[3], (T4)e.Item2[4], (T5)e.Item2[5], (T6)e.Item2[6], (T7)e.Item2[7]))
                .ToArray();

        public (string, T0, T1, T2, T3, T4, T5, T6, T7, T8)[] GetAllParametersAs<T0, T1, T2, T3, T4, T5, T6, T7, T8>() =>
            GetAllParametersAs(new[] { typeof(T0), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8) })
                .Select(e => (e.Item1, (T0)e.Item2[0], (T1)e.Item2[1], (T2)e.Item2[2], (T3)e.Item2[3], (T4)e.Item2[4], (T5)e.Item2[5], (T6)e.Item2[6], (T7)e.Item2[7], (T8)e.Item2[8]))
                .ToArray();

        public (string, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9)[] GetAllParametersAs<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>() =>
            GetAllParametersAs(new[] { typeof(T0), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9) })
                .Select(e => (e.Item1, (T0)e.Item2[0], (T1)e.Item2[1], (T2)e.Item2[2], (T3)e.Item2[3], (T4)e.Item2[4], (T5)e.Item2[5], (T6)e.Item2[6], (T7)e.Item2[7], (T8)e.Item2[8], (T9)e.Item2[9]))
                .ToArray();

        public struct ChainInstance
        {
            private MISObject obj;
            
            internal ChainInstance(MISObject obj) => this.obj = obj;
            
            
            /// <summary>
            /// Check if the parameter with the same values that the action arguments is in the object and invoke the action.
            /// </summary>
            /// <param name="parameter">Parameter name.</param>
            /// <param name="action">Action to invoke if the parameter is in the object.</param>
            /// <returns>Returns the object for chaining.</returns> 
            /// <remarks>
            /// If the parameter is not in the object, an exception is thrown.
            /// </remarks>
            public ChainInstance Require(string parameter, Delegate action)
            {
                obj.parameters.MISRequire(parameter, action);
                return this;
            }

            /// <summary>
            /// Check if the parameter with the same values that the action arguments is in the object and invoke the action.
            /// </summary>
            /// <param name="parameter">Parameter name.</param>
            /// <param name="action">Action to invoke if the parameter is in the object.</param>
            /// <returns>Returns the object for chaining.</returns> 
            public ChainInstance Check(string parameter, Delegate action)
            {
                obj.parameters.MISCheck(parameter, action);
                return this;
            }
        }
    }


    /// <summary>
    /// <b>MIS array.</b>
    /// <br><br>Example: <code>value: [ first, second ];</code></br></br>
    /// </summary>
    public readonly struct MISArray
    {
        private readonly object[] elements;

        internal MISArray(object[] elements)
        {
            this.elements = elements;
        }

        internal void CompileTo(ByteBuffer buffer)
        {
            buffer.Append(Length);
            foreach (var item in elements)
            {
                item.CompileTo(buffer);
            }
        }

        internal static object Decompile(ByteBuffer buffer)
        {
            object[] elements = new object[buffer.ReadInt()];
            for (int i = 0; i < elements.Length; i++)
            {
                elements[i] = MIS.Decompile(buffer);
            }

            return new MISArray(elements);
        }

        /// <summary>
        /// Gets the number of elements in the MISArray.
        /// </summary>
        /// <value>The length of the MISArray.</value>
        public int Length => elements.Length;

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <typeparam name="T">The type of the element.</typeparam>
        /// <param name="i">The index of the element.</param>
        /// <returns>The element at the specified index.</returns>
        public T Get<T>(int i) => (T) elements[i];

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="i">The index of the element.</param>
        /// <returns>The element at the specified index.</returns>
        public object get(int i) => elements[i];
    }
}