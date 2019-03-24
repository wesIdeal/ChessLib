using System.Collections.Generic;
namespace ChessLib.Data.Helpers
{
    using PieceIndex = System.UInt16;
    using BoardRepresentation = System.UInt64;
    public static class BitHelpers
    {

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
