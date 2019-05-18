using System.Collections.Generic;
using System.Diagnostics;

namespace ChessLib.Data.Helpers
{
    using BoardRepresentation = System.UInt64;
    using PieceIndex = System.UInt16;
    public static class BitHelpers
    {
        static readonly ushort[] Index64 = {
                                    0, 47,  1, 56, 48, 27,  2, 60,
                                   57, 49, 41, 37, 28, 16,  3, 61,
                                   54, 58, 35, 52, 50, 42, 21, 44,
                                   38, 32, 29, 23, 17, 11,  4, 62,
                                   46, 55, 26, 59, 40, 36, 15, 53,
                                   34, 51, 20, 43, 31, 22, 10, 45,
                                   25, 39, 14, 33, 19, 30,  9, 24,
                                   13, 18,  8, 12,  7,  6,  5, 63
                                };

        /// <summary>
        /// Uses de Bruijn Sequences to find the least-significant index for the first '1' in a ulong
        /// </summary>
        /// <param name="boardRep">A ulong 'array of bits' representing a chess board.</param>
        /// <returns>index of the least-significant set bit in ulong</returns>
        public static ushort BitScanForward(BoardRepresentation boardRep)
        {
            const ulong deBruijn = (0x03f79d71b4cb0a89ul);
            Debug.Assert(boardRep != 0);
            return Index64[((boardRep ^ (boardRep - 1)) * deBruijn) >> 58];
        }

        /// <summary>
        /// Gets the board value of the index in order to compare boards with bitwise math.
        /// </summary>
        /// <param name="idx">The board index, 0(A1)->63(H8)</param>
        /// <returns>ulong representing a square's value on the board</returns>
        public static BoardRepresentation GetBoardValueOfIndex(this PieceIndex idx) => ((BoardRepresentation)1) << idx;

       

        public static bool IsBitSet(this BoardRepresentation u, PieceIndex bitIndex)
        {
            var comparison = bitIndex.GetBoardValueOfIndex();
            return (comparison & u) == comparison;
        }

        public static PieceIndex[] GetSetBits(this BoardRepresentation u)
        {
            var rv = new List<PieceIndex>(64); //Set max capacity to 64, since our 'array of bits' will be no larger.
            while (u != 0)
            {
                rv.Add(BitScanForward(u));
                u &= (u - 1);
            }
            return rv.ToArray();
        }

        /// <summary>
        /// Sets a bit (specified by <paramref name="bitIndex">bitIndex</paramref>) on a ulong by ORing the value with 1 SHL bitIndex 
        /// </summary>
        /// <param name="u">ulong board representation</param>
        /// <param name="bitIndex">index of bit to set</param>
        public static BoardRepresentation SetBit(this BoardRepresentation u, PieceIndex bitIndex) => u | (1ul << bitIndex);


        /// <summary>
        /// Sets a bit (specified by <paramref name="bitIndex">bitIndex</paramref>) on a ulong reference using <see cref="SetBit(ulong,ushort)">SetBit()</see>  
        /// </summary>
        /// <param name="u">ulong ref of a board representation</param>
        /// <param name="bitIndex">index of bit to set</param>
        public static void SetBit(ref BoardRepresentation u, PieceIndex bitIndex) => u = SetBit(u, bitIndex);

        /// <summary>
        /// Performs <see cref="ClearBit(ulong,int)">ClearBit()</see> on a ulong ref parameter.
        /// </summary>
        /// <param name="boardRep">A ulong reference, in this case representing a chessboard</param>
        /// <param name="bitIndex">Index of bit to clear</param>
        public static void ClearBit(ref BoardRepresentation boardRep, int bitIndex) =>
            boardRep = ClearBit(boardRep, bitIndex);

        /// <summary>
        /// Bitwise 'and' operation with the 'not' of the bit's index to clear.
        /// </summary>
        /// <remarks>Ex. boardRep = 7, clear bit index of 3: boardRep = 0111b; return boardRep ANDed with NOT((1 shl 3)= 0100b) or 0111b AND 1011b returns 0011b</remarks>
        /// <param name="boardRep">A ulong, in this case representing a chessboard</param>
        /// <param name="bitIndex">Index of bit to clear</param>
        /// <returns>ulong, <paramref name="boardRep">boardRep</paramref>, with the index of <paramref name="bitIndex">bitIndex</paramref> cleared.</returns>
        public static BoardRepresentation ClearBit(BoardRepresentation boardRep, int bitIndex) => boardRep & ~(1ul << bitIndex);

        public static PieceIndex CountSetBits(this BoardRepresentation u)
        {
            PieceIndex counter = 0;
            while (u != 0)
            {
                counter++;
                u &= (u - 1);
            }
            return counter;
        }



    }
}
