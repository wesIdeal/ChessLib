using System;
using System.Collections.Generic;
namespace MagicBitboard.Helpers
{
    public static class BitHelpers
    {

        public static bool IsBitSet(this ulong u, int bitIndex)
        {
            var comparrisson = ((ulong)1 << bitIndex);
            return (comparrisson & u) == comparrisson;
        }

        public static int[] GetSetBits(this ulong u)
        {
            var rv = new List<int>();
            for (var i = 0; i < 64; i++)
            {
                if (IsBitSet(u, i)) rv.Add(i);
            }
            return rv.ToArray();
        }

        public static int[] GetSetBits2(this ulong u)
        {
            var idx = 0;
            var setBits = new int[u.CountSetBits()];
            for (var i = 0; i < 64; i++)
            {
                if (IsBitSet(u, i)) setBits[idx++] = i;
            }
            return setBits;
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

        public static void ClearBit(ref ulong u, int bitIndex) => u &= ~((ulong)1 << bitIndex);

        public static ulong PopLSB(this ulong u) => u & (u - 1);

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

        public static ushort[] GetSetBitIndexes(this ulong u)
        {
            ushort counter = 1;
            if (u == 0) return new ushort[0];
            var rv = new ushort[u.CountSetBits()];
            var idx = 0;
            var compare = 0ul;
            while (counter < 64 && ((compare = (1ul << counter)) < u))
            {
                if ((u & compare) != 0)
                {
                    rv[idx++] = (ushort)(counter);
                }
                counter++;
            }
            return rv;
        }
    }
}
