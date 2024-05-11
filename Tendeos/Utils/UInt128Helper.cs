using System;
using System.Runtime.CompilerServices;

namespace Tendeos.Utils
{
    public static class UInt128Helper
    {
        public static byte[] ToBytes(this UInt128 value)
        {
            UInt128Split split = Unsafe.As<UInt128, UInt128Split>(ref value);

            byte[] bytes = new byte[16];
            byte[] a = BitConverter.GetBytes(split.a);
            byte[] b = BitConverter.GetBytes(split.b);
            Array.Copy(a, bytes, 8);
            Array.Copy(b, 0, bytes, 0, 8);
            return bytes;
        }

        public static UInt128 ToUInt128(this byte[] value)
        {
            UInt128Split split = new UInt128Split
            {
                a = BitConverter.ToUInt64(value),
                b = BitConverter.ToUInt64(value, 4)
            };
            return Unsafe.As<UInt128Split, UInt128>(ref split);
        }

        private struct UInt128Split
        {
            public ulong a, b;
        }
    }
}