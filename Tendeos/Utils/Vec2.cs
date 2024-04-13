using Microsoft.Xna.Framework;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Tendeos.Utils
{
    public struct Vec2
    {
        private static readonly Vec2 zeroVector = new Vec2(0f, 0f);

        private static readonly Vec2 unitVector = new Vec2(1f, 1f);

        private static readonly Vec2 unitXVector = new Vec2(1f, 0f);

        private static readonly Vec2 unitYVector = new Vec2(0f, 1f);

        [DataMember]
        public float X;

        [DataMember]
        public float Y;

        public static Vec2 Zero => zeroVector;

        public static Vec2 One => unitVector;

        public static Vec2 UnitX => unitXVector;

        public static Vec2 UnitY => unitYVector;

        internal string DebugDisplayString => X + "  " + Y;

        public Vec2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Vec2(float value)
        {
            X = value;
            Y = value;
        }

        public static Vec2 UpOf(float angle)
        {
            angle = MathHelper.ToRadians(angle);
            return new Vec2(-MathF.Sin(angle), MathF.Cos(angle));
        }
        public static Vec2 DownOf(float angle) => -UpOf(angle);

        public static Vec2 RightOf(float angle)
        {
            angle = MathHelper.ToRadians(angle);
            return new Vec2(MathF.Cos(angle), MathF.Sin(angle));
        }
        public static Vec2 LeftOf(float angle) => -RightOf(angle);

        public static Vec2 RadUpOf(float angle)
        {
            angle = MathHelper.ToRadians(angle);
            return new Vec2(-MathF.Sin(angle), MathF.Cos(angle));
        }
        public static Vec2 RadDownOf(float angle) => -RadUpOf(angle);

        public static Vec2 RadRightOf(float angle)
        {
            angle = MathHelper.ToRadians(angle);
            return new Vec2(MathF.Cos(angle), MathF.Sin(angle));
        }
        public static Vec2 RadLeftOf(float angle) => -RadRightOf(angle);


        public static implicit operator Vec2(System.Numerics.Vector2 value)
        {
            return new Vec2(value.X, value.Y);
        }

        public static Vec2 operator -(Vec2 value)
        {
            value.X = 0f - value.X;
            value.Y = 0f - value.Y;
            return value;
        }

        public static Vec2 operator +(Vec2 value1, Vec2 value2)
        {
            value1.X += value2.X;
            value1.Y += value2.Y;
            return value1;
        }

        public static Vec2 operator -(Vec2 value1, Vec2 value2)
        {
            value1.X -= value2.X;
            value1.Y -= value2.Y;
            return value1;
        }

        public static Vec2 operator *(Vec2 value1, Vec2 value2)
        {
            value1.X *= value2.X;
            value1.Y *= value2.Y;
            return value1;
        }

        public static Vec2 operator *(Vec2 value, float scaleFactor)
        {
            value.X *= scaleFactor;
            value.Y *= scaleFactor;
            return value;
        }

        public static Vec2 operator *(float scaleFactor, Vec2 value)
        {
            value.X *= scaleFactor;
            value.Y *= scaleFactor;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec2 operator /(Vec2 value1, Vec2 value2)
        {
            value1.X /= value2.X;
            value1.Y /= value2.Y;
            return value1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec2 operator /(Vec2 value1, float divider)
        {
            float num = 1f / divider;
            value1.X *= num;
            value1.Y *= num;
            return value1;
        }

        public static bool operator ==(Vec2 value1, Vec2 value2)
        {
            if (value1.X == value2.X)
            {
                return value1.Y == value2.Y;
            }

            return false;
        }

        public static bool operator !=(Vec2 value1, Vec2 value2)
        {
            if (value1.X == value2.X)
            {
                return value1.Y != value2.Y;
            }

            return true;
        }

        public static Vec2 Add(Vec2 value1, Vec2 value2)
        {
            value1.X += value2.X;
            value1.Y += value2.Y;
            return value1;
        }

        public static void Add(ref Vec2 value1, ref Vec2 value2, out Vec2 result)
        {
            result.X = value1.X + value2.X;
            result.Y = value1.Y + value2.Y;
        }

        public static Vec2 Barycentric(Vec2 value1, Vec2 value2, Vec2 value3, float amount1, float amount2)
        {
            return new Vec2(MathHelper.Barycentric(value1.X, value2.X, value3.X, amount1, amount2), MathHelper.Barycentric(value1.Y, value2.Y, value3.Y, amount1, amount2));
        }

        public static void Barycentric(ref Vec2 value1, ref Vec2 value2, ref Vec2 value3, float amount1, float amount2, out Vec2 result)
        {
            result.X = MathHelper.Barycentric(value1.X, value2.X, value3.X, amount1, amount2);
            result.Y = MathHelper.Barycentric(value1.Y, value2.Y, value3.Y, amount1, amount2);
        }

        public static Vec2 CatmullRom(Vec2 value1, Vec2 value2, Vec2 value3, Vec2 value4, float amount)
        {
            return new Vec2(MathHelper.CatmullRom(value1.X, value2.X, value3.X, value4.X, amount), MathHelper.CatmullRom(value1.Y, value2.Y, value3.Y, value4.Y, amount));
        }

        public static void CatmullRom(ref Vec2 value1, ref Vec2 value2, ref Vec2 value3, ref Vec2 value4, float amount, out Vec2 result)
        {
            result.X = MathHelper.CatmullRom(value1.X, value2.X, value3.X, value4.X, amount);
            result.Y = MathHelper.CatmullRom(value1.Y, value2.Y, value3.Y, value4.Y, amount);
        }

        public void Ceiling()
        {
            X = MathF.Ceiling(X);
            Y = MathF.Ceiling(Y);
        }

        public static Vec2 Ceiling(Vec2 value)
        {
            value.X = MathF.Ceiling(value.X);
            value.Y = MathF.Ceiling(value.Y);
            return value;
        }

        public static void Ceiling(ref Vec2 value, out Vec2 result)
        {
            result.X = MathF.Ceiling(value.X);
            result.Y = MathF.Ceiling(value.Y);
        }

        public static Vec2 Clamp(Vec2 value1, Vec2 min, Vec2 max)
        {
            return new Vec2(MathHelper.Clamp(value1.X, min.X, max.X), MathHelper.Clamp(value1.Y, min.Y, max.Y));
        }

        public static void Clamp(ref Vec2 value1, ref Vec2 min, ref Vec2 max, out Vec2 result)
        {
            result.X = MathHelper.Clamp(value1.X, min.X, max.X);
            result.Y = MathHelper.Clamp(value1.Y, min.Y, max.Y);
        }

        public static float Distance(Vec2 value1, Vec2 value2)
        {
            float num = value1.X - value2.X;
            float num2 = value1.Y - value2.Y;
            return MathF.Sqrt(num * num + num2 * num2);
        }

        public static void Distance(ref Vec2 value1, ref Vec2 value2, out float result)
        {
            float num = value1.X - value2.X;
            float num2 = value1.Y - value2.Y;
            result = MathF.Sqrt(num * num + num2 * num2);
        }

        public static float DistanceSquared(Vec2 value1, Vec2 value2)
        {
            float num = value1.X - value2.X;
            float num2 = value1.Y - value2.Y;
            return num * num + num2 * num2;
        }

        public static void DistanceSquared(ref Vec2 value1, ref Vec2 value2, out float result)
        {
            float num = value1.X - value2.X;
            float num2 = value1.Y - value2.Y;
            result = num * num + num2 * num2;
        }

        public static Vec2 Divide(Vec2 value1, Vec2 value2)
        {
            value1.X /= value2.X;
            value1.Y /= value2.Y;
            return value1;
        }

        public static void Divide(ref Vec2 value1, ref Vec2 value2, out Vec2 result)
        {
            result.X = value1.X / value2.X;
            result.Y = value1.Y / value2.Y;
        }

        public static Vec2 Divide(Vec2 value1, float divider)
        {
            float num = 1f / divider;
            value1.X *= num;
            value1.Y *= num;
            return value1;
        }

        public static void Divide(ref Vec2 value1, float divider, out Vec2 result)
        {
            float num = 1f / divider;
            result.X = value1.X * num;
            result.Y = value1.Y * num;
        }

        public static float Dot(Vec2 value1, Vec2 value2)
        {
            return value1.X * value2.X + value1.Y * value2.Y;
        }

        public static void Dot(ref Vec2 value1, ref Vec2 value2, out float result)
        {
            result = value1.X * value2.X + value1.Y * value2.Y;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vec2)
            {
                return Equals((Vec2)obj);
            }

            return false;
        }

        public bool Equals(Vec2 other)
        {
            if (X == other.X)
            {
                return Y == other.Y;
            }

            return false;
        }

        public void Floor()
        {
            X = MathF.Floor(X);
            Y = MathF.Floor(Y);
        }

        public static Vec2 Floor(Vec2 value)
        {
            value.X = MathF.Floor(value.X);
            value.Y = MathF.Floor(value.Y);
            return value;
        }

        public static void Floor(ref Vec2 value, out Vec2 result)
        {
            result.X = MathF.Floor(value.X);
            result.Y = MathF.Floor(value.Y);
        }

        public override int GetHashCode()
        {
            return (X.GetHashCode() * 397) ^ Y.GetHashCode();
        }

        public static Vec2 Hermite(Vec2 value1, Vec2 tangent1, Vec2 value2, Vec2 tangent2, float amount)
        {
            return new Vec2(MathHelper.Hermite(value1.X, tangent1.X, value2.X, tangent2.X, amount), MathHelper.Hermite(value1.Y, tangent1.Y, value2.Y, tangent2.Y, amount));
        }

        public static void Hermite(ref Vec2 value1, ref Vec2 tangent1, ref Vec2 value2, ref Vec2 tangent2, float amount, out Vec2 result)
        {
            result.X = MathHelper.Hermite(value1.X, tangent1.X, value2.X, tangent2.X, amount);
            result.Y = MathHelper.Hermite(value1.Y, tangent1.Y, value2.Y, tangent2.Y, amount);
        }

        public float Length()
        {
            return MathF.Sqrt(X * X + Y * Y);
        }

        public float LengthSquared()
        {
            return X * X + Y * Y;
        }

        public static Vec2 Lerp(Vec2 value1, Vec2 value2, float amount)
        {
            return new Vec2(MathHelper.Lerp(value1.X, value2.X, amount), MathHelper.Lerp(value1.Y, value2.Y, amount));
        }

        public static void Lerp(ref Vec2 value1, ref Vec2 value2, float amount, out Vec2 result)
        {
            result.X = MathHelper.Lerp(value1.X, value2.X, amount);
            result.Y = MathHelper.Lerp(value1.Y, value2.Y, amount);
        }

        public static Vec2 LerpPrecise(Vec2 value1, Vec2 value2, float amount)
        {
            return new Vec2(MathHelper.LerpPrecise(value1.X, value2.X, amount), MathHelper.LerpPrecise(value1.Y, value2.Y, amount));
        }

        public static void LerpPrecise(ref Vec2 value1, ref Vec2 value2, float amount, out Vec2 result)
        {
            result.X = MathHelper.LerpPrecise(value1.X, value2.X, amount);
            result.Y = MathHelper.LerpPrecise(value1.Y, value2.Y, amount);
        }

        public static Vec2 Max(Vec2 value1, Vec2 value2)
        {
            return new Vec2((value1.X > value2.X) ? value1.X : value2.X, (value1.Y > value2.Y) ? value1.Y : value2.Y);
        }

        public static void Max(ref Vec2 value1, ref Vec2 value2, out Vec2 result)
        {
            result.X = ((value1.X > value2.X) ? value1.X : value2.X);
            result.Y = ((value1.Y > value2.Y) ? value1.Y : value2.Y);
        }

        public static Vec2 Min(Vec2 value1, Vec2 value2)
        {
            return new Vec2((value1.X < value2.X) ? value1.X : value2.X, (value1.Y < value2.Y) ? value1.Y : value2.Y);
        }

        public static void Min(ref Vec2 value1, ref Vec2 value2, out Vec2 result)
        {
            result.X = ((value1.X < value2.X) ? value1.X : value2.X);
            result.Y = ((value1.Y < value2.Y) ? value1.Y : value2.Y);
        }

        public static Vec2 Multiply(Vec2 value1, Vec2 value2)
        {
            value1.X *= value2.X;
            value1.Y *= value2.Y;
            return value1;
        }

        public static void Multiply(ref Vec2 value1, ref Vec2 value2, out Vec2 result)
        {
            result.X = value1.X * value2.X;
            result.Y = value1.Y * value2.Y;
        }

        public static Vec2 Multiply(Vec2 value1, float scaleFactor)
        {
            value1.X *= scaleFactor;
            value1.Y *= scaleFactor;
            return value1;
        }

        public static void Multiply(ref Vec2 value1, float scaleFactor, out Vec2 result)
        {
            result.X = value1.X * scaleFactor;
            result.Y = value1.Y * scaleFactor;
        }

        public static Vec2 Negate(Vec2 value)
        {
            value.X = 0f - value.X;
            value.Y = 0f - value.Y;
            return value;
        }

        public static void Negate(ref Vec2 value, out Vec2 result)
        {
            result.X = 0f - value.X;
            result.Y = 0f - value.Y;
        }

        public void Normalize()
        {
            float num = 1f / MathF.Sqrt(X * X + Y * Y);
            X *= num;
            Y *= num;
        }

        public static Vec2 Normalize(Vec2 value)
        {
            float num = 1f / MathF.Sqrt(value.X * value.X + value.Y * value.Y);
            value.X *= num;
            value.Y *= num;
            return value;
        }

        public static void Normalize(ref Vec2 value, out Vec2 result)
        {
            float num = 1f / MathF.Sqrt(value.X * value.X + value.Y * value.Y);
            result.X = value.X * num;
            result.Y = value.Y * num;
        }

        public static Vec2 Reflect(Vec2 vector, Vec2 normal)
        {
            float num = 2f * (vector.X * normal.X + vector.Y * normal.Y);
            Vec2 result = default(Vec2);
            result.X = vector.X - normal.X * num;
            result.Y = vector.Y - normal.Y * num;
            return result;
        }

        public static void Reflect(ref Vec2 vector, ref Vec2 normal, out Vec2 result)
        {
            float num = 2f * (vector.X * normal.X + vector.Y * normal.Y);
            result.X = vector.X - normal.X * num;
            result.Y = vector.Y - normal.Y * num;
        }

        public void Round()
        {
            X = MathF.Round(X);
            Y = MathF.Round(Y);
        }

        public static Vec2 Round(Vec2 value)
        {
            value.X = MathF.Round(value.X);
            value.Y = MathF.Round(value.Y);
            return value;
        }

        public static void Round(ref Vec2 value, out Vec2 result)
        {
            result.X = MathF.Round(value.X);
            result.Y = MathF.Round(value.Y);
        }

        public static Vec2 SmoothStep(Vec2 value1, Vec2 value2, float amount)
        {
            return new Vec2(MathHelper.SmoothStep(value1.X, value2.X, amount), MathHelper.SmoothStep(value1.Y, value2.Y, amount));
        }

        public static void SmoothStep(ref Vec2 value1, ref Vec2 value2, float amount, out Vec2 result)
        {
            result.X = MathHelper.SmoothStep(value1.X, value2.X, amount);
            result.Y = MathHelper.SmoothStep(value1.Y, value2.Y, amount);
        }

        public static Vec2 Subtract(Vec2 value1, Vec2 value2)
        {
            value1.X -= value2.X;
            value1.Y -= value2.Y;
            return value1;
        }

        public static void Subtract(ref Vec2 value1, ref Vec2 value2, out Vec2 result)
        {
            result.X = value1.X - value2.X;
            result.Y = value1.Y - value2.Y;
        }

        public override string ToString()
        {
            return "{X:" + X + " Y:" + Y + "}";
        }

        public Point ToPoint()
        {
            return new Point((int)X, (int)Y);
        }

        public static Vec2 Transform(Vec2 position, Matrix matrix)
        {
            return new Vec2(position.X * matrix.M11 + position.Y * matrix.M21 + matrix.M41, position.X * matrix.M12 + position.Y * matrix.M22 + matrix.M42);
        }

        public static void Transform(ref Vec2 position, ref Matrix matrix, out Vec2 result)
        {
            float x = position.X * matrix.M11 + position.Y * matrix.M21 + matrix.M41;
            float y = position.X * matrix.M12 + position.Y * matrix.M22 + matrix.M42;
            result.X = x;
            result.Y = y;
        }

        public static Vec2 Transform(Vec2 value, Quaternion rotation)
        {
            Transform(ref value, ref rotation, out value);
            return value;
        }

        public static void Transform(ref Vec2 value, ref Quaternion rotation, out Vec2 result)
        {
            Vector3 vector = new Vector3(rotation.X + rotation.X, rotation.Y + rotation.Y, rotation.Z + rotation.Z);
            Vector3 vector2 = new Vector3(rotation.X, rotation.X, rotation.W);
            Vector3 vector3 = new Vector3(1f, rotation.Y, rotation.Z);
            Vector3 vector4 = vector * vector2;
            Vector3 vector5 = vector * vector3;
            Vec2 vector6 = default(Vec2);
            vector6.X = (float)((double)value.X * (1.0 - (double)vector5.Y - (double)vector5.Z) + (double)value.Y * ((double)vector4.Y - (double)vector4.Z));
            vector6.Y = (float)((double)value.X * ((double)vector4.Y + (double)vector4.Z) + (double)value.Y * (1.0 - (double)vector4.X - (double)vector5.Z));
            result.X = vector6.X;
            result.Y = vector6.Y;
        }

        public static void Transform(Vec2[] sourceArray, int sourceIndex, ref Matrix matrix, Vec2[] destinationArray, int destinationIndex, int length)
        {
            if (sourceArray == null)
            {
                throw new ArgumentNullException("sourceArray");
            }

            if (destinationArray == null)
            {
                throw new ArgumentNullException("destinationArray");
            }

            if (sourceArray.Length < sourceIndex + length)
            {
                throw new ArgumentException("Source array length is lesser than sourceIndex + length");
            }

            if (destinationArray.Length < destinationIndex + length)
            {
                throw new ArgumentException("Destination array length is lesser than destinationIndex + length");
            }

            for (int i = 0; i < length; i++)
            {
                Vec2 vector = sourceArray[sourceIndex + i];
                Vec2 vector2 = destinationArray[destinationIndex + i];
                vector2.X = vector.X * matrix.M11 + vector.Y * matrix.M21 + matrix.M41;
                vector2.Y = vector.X * matrix.M12 + vector.Y * matrix.M22 + matrix.M42;
                destinationArray[destinationIndex + i] = vector2;
            }
        }

        public static void Transform(Vec2[] sourceArray, int sourceIndex, ref Quaternion rotation, Vec2[] destinationArray, int destinationIndex, int length)
        {
            if (sourceArray == null)
            {
                throw new ArgumentNullException("sourceArray");
            }

            if (destinationArray == null)
            {
                throw new ArgumentNullException("destinationArray");
            }

            if (sourceArray.Length < sourceIndex + length)
            {
                throw new ArgumentException("Source array length is lesser than sourceIndex + length");
            }

            if (destinationArray.Length < destinationIndex + length)
            {
                throw new ArgumentException("Destination array length is lesser than destinationIndex + length");
            }

            for (int i = 0; i < length; i++)
            {
                Vec2 value = sourceArray[sourceIndex + i];
                Vec2 vector = destinationArray[destinationIndex + i];
                Transform(ref value, ref rotation, out var result);
                vector.X = result.X;
                vector.Y = result.Y;
                destinationArray[destinationIndex + i] = vector;
            }
        }

        public static void Transform(Vec2[] sourceArray, ref Matrix matrix, Vec2[] destinationArray)
        {
            Transform(sourceArray, 0, ref matrix, destinationArray, 0, sourceArray.Length);
        }

        public static void Transform(Vec2[] sourceArray, ref Quaternion rotation, Vec2[] destinationArray)
        {
            Transform(sourceArray, 0, ref rotation, destinationArray, 0, sourceArray.Length);
        }

        public static Vec2 TransformNormal(Vec2 normal, Matrix matrix)
        {
            return new Vec2(normal.X * matrix.M11 + normal.Y * matrix.M21, normal.X * matrix.M12 + normal.Y * matrix.M22);
        }

        public static void TransformNormal(ref Vec2 normal, ref Matrix matrix, out Vec2 result)
        {
            float x = normal.X * matrix.M11 + normal.Y * matrix.M21;
            float y = normal.X * matrix.M12 + normal.Y * matrix.M22;
            result.X = x;
            result.Y = y;
        }

        public static void TransformNormal(Vec2[] sourceArray, int sourceIndex, ref Matrix matrix, Vec2[] destinationArray, int destinationIndex, int length)
        {
            if (sourceArray == null)
            {
                throw new ArgumentNullException("sourceArray");
            }

            if (destinationArray == null)
            {
                throw new ArgumentNullException("destinationArray");
            }

            if (sourceArray.Length < sourceIndex + length)
            {
                throw new ArgumentException("Source array length is lesser than sourceIndex + length");
            }

            if (destinationArray.Length < destinationIndex + length)
            {
                throw new ArgumentException("Destination array length is lesser than destinationIndex + length");
            }

            for (int i = 0; i < length; i++)
            {
                Vec2 vector = sourceArray[sourceIndex + i];
                destinationArray[destinationIndex + i] = new Vec2(vector.X * matrix.M11 + vector.Y * matrix.M21, vector.X * matrix.M12 + vector.Y * matrix.M22);
            }
        }

        public static void TransformNormal(Vec2[] sourceArray, ref Matrix matrix, Vec2[] destinationArray)
        {
            if (sourceArray == null)
            {
                throw new ArgumentNullException("sourceArray");
            }

            if (destinationArray == null)
            {
                throw new ArgumentNullException("destinationArray");
            }

            if (destinationArray.Length < sourceArray.Length)
            {
                throw new ArgumentException("Destination array length is lesser than source array length");
            }

            for (int i = 0; i < sourceArray.Length; i++)
            {
                Vec2 vector = sourceArray[i];
                destinationArray[i] = new Vec2(vector.X * matrix.M11 + vector.Y * matrix.M21, vector.X * matrix.M12 + vector.Y * matrix.M22);
            }
        }

        public void Deconstruct(out float x, out float y)
        {
            x = X;
            y = Y;
        }

        public System.Numerics.Vector2 ToNumerics()
        {
            return new System.Numerics.Vector2(X, Y);
        }

        public static Vec2 Abs(Vec2 value) => new Vec2(Math.Abs(value.X), Math.Abs(value.Y));

        public static implicit operator Vector2(Vec2 a) => new Vector2(a.X, a.Y);
        public static implicit operator Vec2(Vector2 a) => new Vec2(a.X, a.Y);
        public static implicit operator Vector3(Vec2 a) => new Vector3(a.X, a.Y, 0);
        public static implicit operator Vec2(Vector3 a) => new Vec2(a.X, a.Y);
    }
}
