using System.Collections.Generic;
namespace MagicBitboard
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

    }
}
