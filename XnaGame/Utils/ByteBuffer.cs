using System;
using System.IO;
using System.Reflection;
using System.Text;
using XnaGame.Utils.SaveSystem;

namespace XnaGame.Utils
{
    public class ByteBuffer
    {
        private byte[] data;
        private Stream stream;

        public int Position { get; set; }

        public ByteBuffer()
        {
            data = new byte[0];
            Position = 0;
        }

        public ByteBuffer(byte[] bytes)
        {
            data = bytes;
            Position = 0;
        }

        public ByteBuffer(Stream stream)
        {
            data = new byte[1024];
            this.stream = stream;
            Position = 0;
        }

        public ByteBuffer Append(byte data)
        {
            if (stream == null)
            {
                Array.Resize(ref this.data, this.data.Length + 1);
                this.data[^1] = data;
            }
            else stream.WriteByte(data);
            return this;
        }
        public ByteBuffer Append(short data) => Append(BitConverter.GetBytes(data));
        public ByteBuffer Append(int data) => Append(BitConverter.GetBytes(data));
        public ByteBuffer Append(long data) => Append(BitConverter.GetBytes(data));
        public ByteBuffer Append(ushort data) => Append(BitConverter.GetBytes(data));
        public ByteBuffer Append(uint data) => Append(BitConverter.GetBytes(data));
        public ByteBuffer Append(ulong data) => Append(BitConverter.GetBytes(data));
        public ByteBuffer Append(bool data) => Append(BitConverter.GetBytes(data));
        public ByteBuffer Append(Half data) => Append(BitConverter.GetBytes(data));
        public ByteBuffer Append(float data) => Append(BitConverter.GetBytes(data));
        public ByteBuffer Append(double data) => Append(BitConverter.GetBytes(data));
        public ByteBuffer Append(string data)
        {
            Append(Encoding.Unicode.GetBytes(data));
            Append((byte)0x00);
            Append((byte)0x00);
            return this;
        }
        public ByteBuffer Append(byte[] data)
        {
            if (stream == null)
            {
                Array.Resize(ref this.data, this.data.Length + data.Length);
                for (int i = 0; i < data.Length; i++)
                    this.data[^(data.Length - i)] = data[i];
            }
            else stream.Write(data);
            return this;
        }
        public void Append<T>(T data) => Append(data, typeof(T));
        public void Append(object data, Type type)
        {
            foreach (FieldInfo field in type.GetFields())
            {
                ToByteAttribute save = field.GetCustomAttribute<ToByteAttribute>();
                if (save != null)
                {
                    Type fieldType = field.FieldType;
                    object value = field.GetValue(data);
                    if (fieldType == typeof(byte)) Append((byte)value);
                    else if (fieldType == typeof(short)) Append((short)value);
                    else if (fieldType == typeof(int)) Append((int)value);
                    else if (fieldType == typeof(long)) Append((long)value);
                    else if (fieldType == typeof(bool)) Append((bool)value);
                    else if (fieldType == typeof(Half)) Append((Half)value);
                    else if (fieldType == typeof(float)) Append((float)value);
                    else if (fieldType == typeof(double)) Append((double)value);
                    else if (fieldType == typeof(string)) Append((string)value);
                    else Append(value, fieldType);
                }
            }
            foreach (PropertyInfo property in type.GetProperties())
            {
                if (property.CanWrite && property.CanRead)
                {
                    ToByteAttribute save = property.GetCustomAttribute<ToByteAttribute>();
                    if (save != null)
                    {
                        Type propertyType = property.PropertyType;
                        object value = property.GetValue(data);
                        if (propertyType == typeof(byte)) Append((byte)value);
                        else if (propertyType == typeof(short)) Append((short)value);
                        else if (propertyType == typeof(int)) Append((int)value);
                        else if (propertyType == typeof(long)) Append((long)value);
                        else if (propertyType == typeof(bool)) Append((bool)value);
                        else if (propertyType == typeof(Half)) Append((Half)value);
                        else if (propertyType == typeof(float)) Append((float)value);
                        else if (propertyType == typeof(double)) Append((double)value);
                        else if (propertyType == typeof(string)) Append((string)value);
                        else Append(value, propertyType);
                    }
                }
            }
            foreach (MethodInfo method in type.GetMethods())
            {
                ToByteAttribute save = method.GetCustomAttribute<ToByteAttribute>();
                if (save != null)
                {
                    ParameterInfo[] args = method.GetParameters();
                    if (args.Length != 1) throw new SaveException("To byte method must have only one argument.");
                    if (args[0].ParameterType != typeof(ByteBuffer)) throw new SaveException("The argument of to byte method must be ByteBuffer.");
                    method.Invoke(data, new object[] { this });
                }
            }
        }

        public byte ReadByte()
        {
            if (stream == null) return data[(Position += 1) - 1];
            return (byte)stream.ReadByte();
        }
        public short ReadShort()
        {
            if (stream == null) return BitConverter.ToInt16(data, (Position += 2) - 2);
            stream.Read(data, 0, 2);
            return BitConverter.ToInt16(data);
        }
        public int ReadInt()
        {
            if (stream == null) return BitConverter.ToInt32(data, (Position += 4) - 4);
            stream.Read(data, 0, 4);
            return BitConverter.ToInt32(data);
        }
        public long ReadLong()
        {
            if (stream == null) return BitConverter.ToInt64(data, (Position += 8) - 8);
            stream.Read(data, 0, 8);
            return BitConverter.ToInt64(data);
        }
        public ushort ReadUShort()
        {
            if (stream == null) return BitConverter.ToUInt16(data, (Position += 2) - 2);
            stream.Read(data, 0, 2);
            return BitConverter.ToUInt16(data);
        }
        public uint ReadUInt()
        {
            if (stream == null) return BitConverter.ToUInt32(data, (Position += 4) - 4);
            stream.Read(data, 0, 4);
            return BitConverter.ToUInt32(data);
        }
        public ulong ReadULong()
        {
            if (stream == null) return BitConverter.ToUInt64(data, (Position += 8) - 8);
            stream.Read(data, 0, 8);
            return BitConverter.ToUInt64(data);
        }
        public bool ReadBool() => ReadByte() > 0;
        public Half ReadHalf()
        {
            if (stream == null) return BitConverter.ToHalf(data, (Position += 2) - 2);
            stream.Read(data, 0, 2);
            return BitConverter.ToHalf(data);
        }
        public float ReadFloat()
        {
            if (stream == null) return BitConverter.ToSingle(data, (Position += 4) - 4);
            stream.Read(data, 0, 4);
            return BitConverter.ToSingle(data);
        }
        public double ReadDouble()
        {
            if (stream == null) return BitConverter.ToDouble(data, (Position += 8) - 8);
            stream.Read(data, 0, 8);
            return BitConverter.ToDouble(data);
        }
        public string ReadString()
        {
            int i = 0;
            while (true)
            {
                i += 2;
                if ((data[i - 2] = ReadByte()) == 0x00 & (data[i - 1] = ReadByte()) == 0x00) break;
            }

            if (stream == null) return Encoding.Unicode.GetString(data, Position - i, i - 2);
            return Encoding.Unicode.GetString(data, 0, i - 2);
        }
        public byte[] Read(int length)
        {
            if (stream == null) return data[Position..(Position += length)];
            stream.Read(data, 0, length);
            return data[..length];
        }
        public void Read<T>(T to) => Read(to, typeof(T));
        public void Read(object to, Type type)
        {
            foreach (FieldInfo field in type.GetFields())
            {
                ToByteAttribute save = field.GetCustomAttribute<ToByteAttribute>();
                if (save != null)
                {
                    Type fieldType = field.FieldType;
                    if (fieldType == typeof(byte)) field.SetValue(to, ReadByte());
                    else if (fieldType == typeof(short)) field.SetValue(to, ReadShort());
                    else if (fieldType == typeof(int)) field.SetValue(to, ReadInt());
                    else if (fieldType == typeof(long)) field.SetValue(to, ReadLong());
                    else if (fieldType == typeof(bool)) field.SetValue(to, ReadBool());
                    else if (fieldType == typeof(Half)) field.SetValue(to, ReadHalf());
                    else if (fieldType == typeof(float)) field.SetValue(to, ReadFloat());
                    else if (fieldType == typeof(double)) field.SetValue(to, ReadDouble());
                    else if (fieldType == typeof(string)) field.SetValue(to, ReadString());
                    else Read(field.GetValue(to), fieldType);
                }
            }
            foreach (PropertyInfo property in type.GetProperties())
            {
                if (property.CanWrite && property.CanRead)
                {
                    ToByteAttribute save = property.GetCustomAttribute<ToByteAttribute>();
                    if (save != null)
                    {
                        Type propertyType = property.PropertyType;
                        if (propertyType == typeof(byte)) property.SetValue(to, ReadByte());
                        else if (propertyType == typeof(short)) property.SetValue(to, ReadShort());
                        else if (propertyType == typeof(int)) property.SetValue(to, ReadInt());
                        else if (propertyType == typeof(long)) property.SetValue(to, ReadLong());
                        else if (propertyType == typeof(bool)) property.SetValue(to, ReadBool());
                        else if (propertyType == typeof(Half)) property.SetValue(to, ReadHalf());
                        else if (propertyType == typeof(float)) property.SetValue(to, ReadFloat());
                        else if (propertyType == typeof(double)) property.SetValue(to, ReadDouble());
                        else if (propertyType == typeof(string)) property.SetValue(to, ReadString());
                        else Read(property.GetValue(to), propertyType);
                    }
                }
            }
            foreach (MethodInfo method in type.GetMethods())
            {
                FromByteAttribute load = method.GetCustomAttribute<FromByteAttribute>();
                if (load != null)
                {
                    ParameterInfo[] args = method.GetParameters();
                    if (args.Length != 1) throw new SaveException("From byte method must have only one argument.");
                    if (args[0].ParameterType != typeof(ByteBuffer)) throw new SaveException("The argument of from byte method must be ByteBuffer.");
                    method.Invoke(to, new object[] { this });
                }
            }
        }

        public void SetStream(byte[] stream) => data = stream;
        public void SetStream(Stream stream) => this.stream = stream;

        public static implicit operator byte[](ByteBuffer buffer) => buffer.data;
    }
}
