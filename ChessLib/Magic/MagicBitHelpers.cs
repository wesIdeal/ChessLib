using System;

namespace ChessLib.Magic
{
    static class MagicBitHelpers
    {
        public static ulong LSB(this ulong u)
        {
            var castedVal = (long)u;
            return (ulong)(castedVal & -castedVal);
        }

        public static int LSB(this int u)
        {
            var castedVal = (long)u;
            return (u & -u);
        }

        public static ulong PopLSB(this ulong u)
        {
            return u & (u - 1);
        }

        public static int PopLSB(this int u)
        {
            return u & (u - 1);
        }

        public static int GetLSBIndex(this ulong u)
        {
            return (int)((Math.Log10((long)u & -((long)u)))
              / Math.Log10(2));
        }

    }
}
