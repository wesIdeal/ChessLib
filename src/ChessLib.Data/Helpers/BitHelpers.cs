using System.Collections.Generic;
using System.Diagnostics;

namespace ChessLib.Data.Helpers
{
    using PieceIndex = System.UInt16;
    using BoardRepresentation = System.UInt64;
    public static class BitHelpers
    {
        readonly static ushort[] index64 = new ushort[64] {
                                    0, 47,  1, 56, 48, 27,  2, 60,
                                   57, 49, 41, 37, 28, 16,  3, 61,
                                   54, 58, 35, 52, 50, 42, 21, 44,
                                   38, 32, 29, 23, 17, 11,  4, 62,
                                   46, 55, 26, 59, 40, 36, 15, 53,
                                   34, 51, 20, 43, 31, 22, 10, 45,
                                   25, 39, 14, 33, 19, 30,  9, 24,
                                   13, 18,  8, 12,  7,  6,  5, 63
                                };
        public static ushort BitScanForward(ulong bb)
        {
            const ulong debruijn64 = (0x03f79d71b4cb0a89ul);
            Debug.Assert(bb != 0);
            return index64[((bb ^ (bb - 1)) * debruijn64) >> 58];
        }

        public static bool IsBitSet(this BoardRepresentation u, PieceIndex bitIndex)
        {
            var comparrisson = ((BoardRepresentation)1 << bitIndex);
            return (comparrisson & u) == comparrisson;
        }

        public static PieceIndex[] GetSetBits(this BoardRepresentation u)
        {
            var rv = new List<PieceIndex>();
            for (PieceIndex i = 0; i < 64; i++)
            {
                if (IsBitSet(u, i)) rv.Add(i);
            }
            return rv.ToArray();
        }


        public static BoardRepresentation SetBit(this BoardRepresentation u, PieceIndex bitIndex)
        {
            SetBit(ref u, bitIndex);
            return u;
        }

        public static void SetBit(ref BoardRepresentation u, int bitIndex)
        {
            var bitValue = (BoardRepresentation)1 << bitIndex;
            u |= bitValue;
        }

        public static void ClearBit(ref BoardRepresentation u, int bitIndex) => u &= ~((BoardRepresentation)1 << bitIndex);

        public static BoardRepresentation PopLSB(this BoardRepresentation u) => u & (u - 1);

        public static PieceIndex CountSetBits(this BoardRepresentation u)
        {
            PieceIndex counter = 0;
            while (u != 0)
            {
                u = u.PopLSB();
                counter++;
            }
            return counter;
        }



    }
}
