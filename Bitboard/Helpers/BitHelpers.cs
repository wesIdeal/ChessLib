using System;
using System.Collections.Generic;
namespace MagicBitboard.Helpers
{
    public static class BitHelpers
    {
        public static bool IsBitSet(ulong u, int bitIndex)
        {
            var comparrisson = ((ulong)1 << bitIndex);
            return (comparrisson & u) == comparrisson;
        }

        public static List<int> GetSetBits(this ulong u)
        {
            var rv = new List<int>();
            for (var i = 0; i < 64; i++)
            {
                if (IsBitSet(u, i)) rv.Add(i);
            }
            return rv;
        }

        public static ulong SetBit(this ulong u, ushort bitIndex)
        {
            SetBit(ref u, bitIndex);
            return u;
        }

        public static void SetBit(ref ulong u, int bitIndex)
        {
            var bitValue = (ulong)1 << bitIndex;
            u |= bitValue;
        }

        public static void ClearBit(ref ulong u, int bitIndex)
        {
            var notBitValue = ~((ulong)1 << bitIndex);
            u &= notBitValue;
        }

        public static ulong PopLSB(this ulong u)
        {
            return u & (u - 1);
        }

        public static ushort CountSetBits(this ulong u)
        {
            ushort counter = 0;
            while (u != 0)
            {
                u = u.PopLSB();
                counter++;
            }
            return counter;
        }
    }
}
