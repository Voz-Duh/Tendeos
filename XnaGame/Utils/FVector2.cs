using AVector2 = nkast.Aether.Physics2D.Common.Vector2;
using XVector2 = Microsoft.Xna.Framework.Vector2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace XnaGame.Utils
{
    public struct FVector2
    {
        private static readonly FVector2 zeroVector = new FVector2(0f, 0f);

        private static readonly FVector2 unitVector = new FVector2(1f, 1f);

        private static readonly FVector2 unitXVector = new FVector2(1f, 0f);

        private static readonly FVector2 unitYVector = new FVector2(0f, 1f);

        [DataMember]
        public float X;

        [DataMember]
        public float Y;

        public static FVector2 Zero => zeroVector;

        public static FVector2 One => unitVector;

        public static FVector2 UnitX => unitXVector;

        public static FVector2 UnitY => unitYVector;

        internal string DebugDisplayString => X + "  " + Y;

        public FVector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public FVector2(float value)
        {
            X = value;
            Y = value;
        }

        public static implicit operator FVector2(System.Numerics.Vector2 value)
        {
            return new FVector2(value.X, value.Y);
        }

        public static FVector2 operator -(FVector2 value)
        {
            value.X = 0f - value.X;
            value.Y = 0f - value.Y;
            return value;
        }

        public static FVector2 operator +(FVector2 value1, FVector2 value2)
        {
            value1.X += value2.X;
            value1.Y += value2.Y;
            return value1;
        }

        public static FVector2 operator -(FVector2 value1, FVector2 value2)
        {
            value1.X -= value2.X;
            value1.Y -= value2.Y;
            return value1;
        }

        public static FVector2 operator *(FVector2 value1, FVector2 value2)
        {
            value1.X *= value2.X;
            value1.Y *= value2.Y;
            return value1;
        }

        public static FVector2 operator *(FVector2 value, float scaleFactor)
        {
            value.X *= scaleFactor;
            value.Y *= scaleFactor;
            return value;
        }

        public static FVector2 operator *(float scaleFactor, FVector2 value)
        {
            value.X *= scaleFactor;
            value.Y *= scaleFactor;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FVector2 operator /(FVector2 value1, FVector2 value2)
        {
            value1.X /= value2.X;
            value1.Y /= value2.Y;
            return value1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FVector2 operator /(FVector2 value1, float divider)
        {
            float num = 1f / divider;
            value1.X *= num;
            value1.Y *= num;
            return value1;
        }

        public static bool operator ==(FVector2 value1, FVector2 value2)
        {
            if (value1.X == value2.X)
            {
                return value1.Y == value2.Y;
            }

            return false;
        }

        public static bool operator !=(FVector2 value1, FVector2 value2)
        {
            if (value1.X == value2.X)
            {
                return value1.Y != value2.Y;
            }

            return true;
        }

        public static FVector2 Add(FVector2 value1, FVector2 value2)
        {
            value1.X += value2.X;
            value1.Y += value2.Y;
            return value1;
        }

        public static void Add(ref FVector2 value1, ref FVector2 value2, out FVector2 result)
        {
            result.X = value1.X + value2.X;
            result.Y = value1.Y + value2.Y;
        }

        public static FVector2 Barycentric(FVector2 value1, FVector2 value2, FVector2 value3, float amount1, float amount2)
        {
            return new FVector2(MathHelper.Barycentric(value1.X, value2.X, value3.X, amount1, amount2), MathHelper.Barycentric(value1.Y, value2.Y, value3.Y, amount1, amount2));
        }

        public static void Barycentric(ref FVector2 value1, ref FVector2 value2, ref FVector2 value3, float amount1, float amount2, out FVector2 result)
        {
            result.X = MathHelper.Barycentric(value1.X, value2.X, value3.X, amount1, amount2);
            result.Y = MathHelper.Barycentric(value1.Y, value2.Y, value3.Y, amount1, amount2);
        }

        public static FVector2 CatmullRom(FVector2 value1, FVector2 value2, FVector2 value3, FVector2 value4, float amount)
        {
            return new FVector2(MathHelper.CatmullRom(value1.X, value2.X, value3.X, value4.X, amount), MathHelper.CatmullRom(value1.Y, value2.Y, value3.Y, value4.Y, amount));
        }

        public static void CatmullRom(ref FVector2 value1, ref FVector2 value2, ref FVector2 value3, ref FVector2 value4, float amount, out FVector2 result)
        {
            result.X = MathHelper.CatmullRom(value1.X, value2.X, value3.X, value4.X, amount);
            result.Y = MathHelper.CatmullRom(value1.Y, value2.Y, value3.Y, value4.Y, amount);
        }

        public void Ceiling()
        {
            X = MathF.Ceiling(X);
            Y = MathF.Ceiling(Y);
        }

        public static FVector2 Ceiling(FVector2 value)
        {
            value.X = MathF.Ceiling(value.X);
            value.Y = MathF.Ceiling(value.Y);
            return value;
        }

        public static void Ceiling(ref FVector2 value, out FVector2 result)
        {
            result.X = MathF.Ceiling(value.X);
            result.Y = MathF.Ceiling(value.Y);
        }

        public static FVector2 Clamp(FVector2 value1, FVector2 min, FVector2 max)
        {
            return new FVector2(MathHelper.Clamp(value1.X, min.X, max.X), MathHelper.Clamp(value1.Y, min.Y, max.Y));
        }

        public static void Clamp(ref FVector2 value1, ref FVector2 min, ref FVector2 max, out FVector2 result)
        {
            result.X = MathHelper.Clamp(value1.X, min.X, max.X);
            result.Y = MathHelper.Clamp(value1.Y, min.Y, max.Y);
        }

        public static float Distance(FVector2 value1, FVector2 value2)
        {
            float num = value1.X - value2.X;
            float num2 = value1.Y - value2.Y;
            return MathF.Sqrt(num * num + num2 * num2);
        }

        public static void Distance(ref FVector2 value1, ref FVector2 value2, out float result)
        {
            float num = value1.X - value2.X;
            float num2 = value1.Y - value2.Y;
            result = MathF.Sqrt(num * num + num2 * num2);
        }

        public static float DistanceSquared(FVector2 value1, FVector2 value2)
        {
            float num = value1.X - value2.X;
            float num2 = value1.Y - value2.Y;
            return num * num + num2 * num2;
        }

        public static void DistanceSquared(ref FVector2 value1, ref FVector2 value2, out float result)
        {
            float num = value1.X - value2.X;
            float num2 = value1.Y - value2.Y;
            result = num * num + num2 * num2;
        }

        public static FVector2 Divide(FVector2 value1, FVector2 value2)
        {
            value1.X /= value2.X;
            value1.Y /= value2.Y;
            return value1;
        }

        public static void Divide(ref FVector2 value1, ref FVector2 value2, out FVector2 result)
        {
            result.X = value1.X / value2.X;
            result.Y = value1.Y / value2.Y;
        }

        public static FVector2 Divide(FVector2 value1, float divider)
        {
            float num = 1f / divider;
            value1.X *= num;
            value1.Y *= num;
            return value1;
        }

        public static void Divide(ref FVector2 value1, float divider, out FVector2 result)
        {
            float num = 1f / divider;
            result.X = value1.X * num;
            result.Y = value1.Y * num;
        }

        public static float Dot(FVector2 value1, FVector2 value2)
        {
            return value1.X * value2.X + value1.Y * value2.Y;
        }

        public static void Dot(ref FVector2 value1, ref FVector2 value2, out float result)
        {
            result = value1.X * value2.X + value1.Y * value2.Y;
        }

        public override bool Equals(object obj)
        {
            if (obj is FVector2)
            {
                return Equals((FVector2)obj);
            }

            return false;
        }

        public bool Equals(FVector2 other)
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

        public static FVector2 Floor(FVector2 value)
        {
            value.X = MathF.Floor(value.X);
            value.Y = MathF.Floor(value.Y);
            return value;
        }

        public static void Floor(ref FVector2 value, out FVector2 result)
        {
            result.X = MathF.Floor(value.X);
            result.Y = MathF.Floor(value.Y);
        }

        public override int GetHashCode()
        {
            return (X.GetHashCode() * 397) ^ Y.GetHashCode();
        }

        public static FVector2 Hermite(FVector2 value1, FVector2 tangent1, FVector2 value2, FVector2 tangent2, float amount)
        {
            return new FVector2(MathHelper.Hermite(value1.X, tangent1.X, value2.X, tangent2.X, amount), MathHelper.Hermite(value1.Y, tangent1.Y, value2.Y, tangent2.Y, amount));
        }

        public static void Hermite(ref FVector2 value1, ref FVector2 tangent1, ref FVector2 value2, ref FVector2 tangent2, float amount, out FVector2 result)
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

        public static FVector2 Lerp(FVector2 value1, FVector2 value2, float amount)
        {
            return new FVector2(MathHelper.Lerp(value1.X, value2.X, amount), MathHelper.Lerp(value1.Y, value2.Y, amount));
        }

        public static void Lerp(ref FVector2 value1, ref FVector2 value2, float amount, out FVector2 result)
        {
            result.X = MathHelper.Lerp(value1.X, value2.X, amount);
            result.Y = MathHelper.Lerp(value1.Y, value2.Y, amount);
        }

        public static FVector2 LerpPrecise(FVector2 value1, FVector2 value2, float amount)
        {
            return new FVector2(MathHelper.LerpPrecise(value1.X, value2.X, amount), MathHelper.LerpPrecise(value1.Y, value2.Y, amount));
        }

        public static void LerpPrecise(ref FVector2 value1, ref FVector2 value2, float amount, out FVector2 result)
        {
            result.X = MathHelper.LerpPrecise(value1.X, value2.X, amount);
            result.Y = MathHelper.LerpPrecise(value1.Y, value2.Y, amount);
        }

        public static FVector2 Max(FVector2 value1, FVector2 value2)
        {
            return new FVector2((value1.X > value2.X) ? value1.X : value2.X, (value1.Y > value2.Y) ? value1.Y : value2.Y);
        }

        public static void Max(ref FVector2 value1, ref FVector2 value2, out FVector2 result)
        {
            result.X = ((value1.X > value2.X) ? value1.X : value2.X);
            result.Y = ((value1.Y > value2.Y) ? value1.Y : value2.Y);
        }

        public static FVector2 Min(FVector2 value1, FVector2 value2)
        {
            return new FVector2((value1.X < value2.X) ? value1.X : value2.X, (value1.Y < value2.Y) ? value1.Y : value2.Y);
        }

        public static void Min(ref FVector2 value1, ref FVector2 value2, out FVector2 result)
        {
            result.X = ((value1.X < value2.X) ? value1.X : value2.X);
            result.Y = ((value1.Y < value2.Y) ? value1.Y : value2.Y);
        }

        public static FVector2 Multiply(FVector2 value1, FVector2 value2)
        {
            value1.X *= value2.X;
            value1.Y *= value2.Y;
            return value1;
        }

        public static void Multiply(ref FVector2 value1, ref FVector2 value2, out FVector2 result)
        {
            result.X = value1.X * value2.X;
            result.Y = value1.Y * value2.Y;
        }

        public static FVector2 Multiply(FVector2 value1, float scaleFactor)
        {
            value1.X *= scaleFactor;
            value1.Y *= scaleFactor;
            return value1;
        }

        public static void Multiply(ref FVector2 value1, float scaleFactor, out FVector2 result)
        {
            result.X = value1.X * scaleFactor;
            result.Y = value1.Y * scaleFactor;
        }

        public static FVector2 Negate(FVector2 value)
        {
            value.X = 0f - value.X;
            value.Y = 0f - value.Y;
            return value;
        }

        public static void Negate(ref FVector2 value, out FVector2 result)
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

        public static FVector2 Normalize(FVector2 value)
        {
            float num = 1f / MathF.Sqrt(value.X * value.X + value.Y * value.Y);
            value.X *= num;
            value.Y *= num;
            return value;
        }

        public static void Normalize(ref FVector2 value, out FVector2 result)
        {
            float num = 1f / MathF.Sqrt(value.X * value.X + value.Y * value.Y);
            result.X = value.X * num;
            result.Y = value.Y * num;
        }

        public static FVector2 Reflect(FVector2 vector, FVector2 normal)
        {
            float num = 2f * (vector.X * normal.X + vector.Y * normal.Y);
            FVector2 result = default(FVector2);
            result.X = vector.X - normal.X * num;
            result.Y = vector.Y - normal.Y * num;
            return result;
        }

        public static void Reflect(ref FVector2 vector, ref FVector2 normal, out FVector2 result)
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

        public static FVector2 Round(FVector2 value)
        {
            value.X = MathF.Round(value.X);
            value.Y = MathF.Round(value.Y);
            return value;
        }

        public static void Round(ref FVector2 value, out FVector2 result)
        {
            result.X = MathF.Round(value.X);
            result.Y = MathF.Round(value.Y);
        }

        public static FVector2 SmoothStep(FVector2 value1, FVector2 value2, float amount)
        {
            return new FVector2(MathHelper.SmoothStep(value1.X, value2.X, amount), MathHelper.SmoothStep(value1.Y, value2.Y, amount));
        }

        public static void SmoothStep(ref FVector2 value1, ref FVector2 value2, float amount, out FVector2 result)
        {
            result.X = MathHelper.SmoothStep(value1.X, value2.X, amount);
            result.Y = MathHelper.SmoothStep(value1.Y, value2.Y, amount);
        }

        public static FVector2 Subtract(FVector2 value1, FVector2 value2)
        {
            value1.X -= value2.X;
            value1.Y -= value2.Y;
            return value1;
        }

        public static void Subtract(ref FVector2 value1, ref FVector2 value2, out FVector2 result)
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

        public static FVector2 Transform(FVector2 position, Matrix matrix)
        {
            return new FVector2(position.X * matrix.M11 + position.Y * matrix.M21 + matrix.M41, position.X * matrix.M12 + position.Y * matrix.M22 + matrix.M42);
        }

        public static void Transform(ref FVector2 position, ref Matrix matrix, out FVector2 result)
        {
            float x = position.X * matrix.M11 + position.Y * matrix.M21 + matrix.M41;
            float y = position.X * matrix.M12 + position.Y * matrix.M22 + matrix.M42;
            result.X = x;
            result.Y = y;
        }

        public static FVector2 Transform(FVector2 value, Quaternion rotation)
        {
            Transform(ref value, ref rotation, out value);
            return value;
        }

        public static void Transform(ref FVector2 value, ref Quaternion rotation, out FVector2 result)
        {
            Vector3 vector = new Vector3(rotation.X + rotation.X, rotation.Y + rotation.Y, rotation.Z + rotation.Z);
            Vector3 vector2 = new Vector3(rotation.X, rotation.X, rotation.W);
            Vector3 vector3 = new Vector3(1f, rotation.Y, rotation.Z);
            Vector3 vector4 = vector * vector2;
            Vector3 vector5 = vector * vector3;
            FVector2 vector6 = default(FVector2);
            vector6.X = (float)((double)value.X * (1.0 - (double)vector5.Y - (double)vector5.Z) + (double)value.Y * ((double)vector4.Y - (double)vector4.Z));
            vector6.Y = (float)((double)value.X * ((double)vector4.Y + (double)vector4.Z) + (double)value.Y * (1.0 - (double)vector4.X - (double)vector5.Z));
            result.X = vector6.X;
            result.Y = vector6.Y;
        }

        public static void Transform(FVector2[] sourceArray, int sourceIndex, ref Matrix matrix, FVector2[] destinationArray, int destinationIndex, int length)
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
                FVector2 vector = sourceArray[sourceIndex + i];
                FVector2 vector2 = destinationArray[destinationIndex + i];
                vector2.X = vector.X * matrix.M11 + vector.Y * matrix.M21 + matrix.M41;
                vector2.Y = vector.X * matrix.M12 + vector.Y * matrix.M22 + matrix.M42;
                destinationArray[destinationIndex + i] = vector2;
            }
        }

        public static void Transform(FVector2[] sourceArray, int sourceIndex, ref Quaternion rotation, FVector2[] destinationArray, int destinationIndex, int length)
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
                FVector2 value = sourceArray[sourceIndex + i];
                FVector2 vector = destinationArray[destinationIndex + i];
                Transform(ref value, ref rotation, out var result);
                vector.X = result.X;
                vector.Y = result.Y;
                destinationArray[destinationIndex + i] = vector;
            }
        }

        public static void Transform(FVector2[] sourceArray, ref Matrix matrix, FVector2[] destinationArray)
        {
            Transform(sourceArray, 0, ref matrix, destinationArray, 0, sourceArray.Length);
        }

        public static void Transform(FVector2[] sourceArray, ref Quaternion rotation, FVector2[] destinationArray)
        {
            Transform(sourceArray, 0, ref rotation, destinationArray, 0, sourceArray.Length);
        }

        public static FVector2 TransformNormal(FVector2 normal, Matrix matrix)
        {
            return new FVector2(normal.X * matrix.M11 + normal.Y * matrix.M21, normal.X * matrix.M12 + normal.Y * matrix.M22);
        }

        public static void TransformNormal(ref FVector2 normal, ref Matrix matrix, out FVector2 result)
        {
            float x = normal.X * matrix.M11 + normal.Y * matrix.M21;
            float y = normal.X * matrix.M12 + normal.Y * matrix.M22;
            result.X = x;
            result.Y = y;
        }

        public static void TransformNormal(FVector2[] sourceArray, int sourceIndex, ref Matrix matrix, FVector2[] destinationArray, int destinationIndex, int length)
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
                FVector2 vector = sourceArray[sourceIndex + i];
                destinationArray[destinationIndex + i] = new FVector2(vector.X * matrix.M11 + vector.Y * matrix.M21, vector.X * matrix.M12 + vector.Y * matrix.M22);
            }
        }

        public static void TransformNormal(FVector2[] sourceArray, ref Matrix matrix, FVector2[] destinationArray)
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
                FVector2 vector = sourceArray[i];
                destinationArray[i] = new FVector2(vector.X * matrix.M11 + vector.Y * matrix.M21, vector.X * matrix.M12 + vector.Y * matrix.M22);
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

        public static implicit operator AVector2(FVector2 a) => new AVector2(a.X, a.Y);
        public static implicit operator XVector2(FVector2 a) => new XVector2(a.X, a.Y);
        public static implicit operator FVector2(AVector2 a) => new FVector2(a.X, a.Y);
        public static implicit operator FVector2(XVector2 a) => new FVector2(a.X, a.Y);
    }
}
