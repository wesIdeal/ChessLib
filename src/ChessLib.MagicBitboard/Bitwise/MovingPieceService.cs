#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ChessLib.Types.Enums;

#endregion

[assembly: InternalsVisibleTo("ChessLib.MagicBitboard.Bitwise.Tests")]

namespace ChessLib.MagicBitboard.Bitwise
{
    public class MovingPieceService
    {
        private static readonly ushort[] Index64 =
        {
            0, 47, 1, 56, 48, 27, 2, 60,
            57, 49, 41, 37, 28, 16, 3, 61,
            54, 58, 35, 52, 50, 42, 21, 44,
            38, 32, 29, 23, 17, 11, 4, 62,
            46, 55, 26, 59, 40, 36, 15, 53,
            34, 51, 20, 43, 31, 22, 10, 45,
            25, 39, 14, 33, 19, 30, 9, 24,
            13, 18, 8, 12, 7, 6, 5, 63
        };

        public MovingPieceService()
        {
            Initialize();
        }

        public static ulong GetPawnStartRankMask(Color color)
        {
            if (color == Color.Black)
            {
                return BoardConstants.Rank7;
            }

            return BoardConstants.Rank2;
        }

        /// <summary>
        ///     Gets a rank index from boardIndex
        ///     <param name="boardIndex">index</param>
        /// </summary>
        /// <param name="boardIndex"></param>
        /// <returns>Board rank (First rank: 0)</returns>
        /// <exception cref="ArgumentException">
        ///     if
        ///     <param name="boardIndex">index</param>
        ///     is out of range.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort RankFromIdx(ushort boardIndex)
        {
            return (ushort) (boardIndex / 8);
        }

        /// <summary>
        ///     Gets a file index from a boardIndex index
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort FileFromIdx(ushort idx)
        {
            return (ushort) (idx % 8);
        }

        /// <summary>
        ///     Gets an array of bit indexes set to '1'
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public static ushort[] GetSetBits(ulong u)
        {
            var rv = new List<ushort>(64); //Set max capacity to 64, since our 'array of bits' will be no larger.
            while (u != 0)
            {
                rv.Add(BitScanForward(u));
                u &= u - 1;
            }

            return rv.ToArray();
        }


        public static ulong ShiftN(ulong u)
        {
            return (u & BoardConstants.NotRank8Mask) << 8;
        }

        public static ulong AttackN(ulong u)
        {
            var shiftedValue = ShiftN(u);
            var rv = shiftedValue & BoardConstants.NotRank8Mask;
            return rv;
        }

        public static ulong ShiftE(ulong u)
        {
            return (u & BoardConstants.NotHFileMask) << 1;
        }

        public static ulong AttackE(ulong u)
        {
            var shiftedValue = ShiftE(u);
            return shiftedValue & BoardConstants.NotHFileMask;
        }

        public static ulong ShiftS(ulong u)
        {
            return (u & BoardConstants.NotRank1Mask) >> 8;
        }

        public static ulong AttackS(ulong u)
        {
            var shiftedValue = ShiftS(u);
            return shiftedValue & BoardConstants.NotRank1Mask;
        }

        public static ulong ShiftW(ulong u)
        {
            return (u & BoardConstants.NotAFileMask) >> 1;
        }

        public static ulong AttackW(ulong u)
        {
            var shiftedValue = ShiftW(u);
            return shiftedValue & BoardConstants.NotAFileMask;
        }

        public static ulong Shift2N(ulong u)
        {
            return (u & BoardConstants.NotRank7Mask & BoardConstants.NotRank8Mask) << 16;
        }

        public static ulong Shift2E(ulong u)
        {
            return (u & BoardConstants.NotGFileMask & BoardConstants.NotHFileMask) << 2;
        }

        public static ulong Shift2S(ulong u)
        {
            return (u & BoardConstants.NotRank1Mask & BoardConstants.NotRank2Mask) >> 16;
        }

        public static ulong Shift2W(ulong u)
        {
            return (u & BoardConstants.NotBFileMask & BoardConstants.NotAFileMask) >> 2;
        }

        public static ulong ShiftNE(ulong u)
        {
            return ShiftE(ShiftN(u));
        }

        public static ulong ShiftSE(ulong u)
        {
            return ShiftE(ShiftS(u));
        }

        public static ulong ShiftSW(ulong u)
        {
            return ShiftW(ShiftS(u));
        }

        public static ulong ShiftNW(ulong u)
        {
            return ShiftW(ShiftN(u));
        }

        public static ulong ShiftNNE(ulong u)
        {
            return ShiftE(Shift2N(u));
        }

        public static ulong ShiftENE(ulong u)
        {
            return ShiftN(Shift2E(u));
        }

        public static ulong ShiftESE(ulong u)
        {
            return Shift2E(ShiftS(u));
        }

        public static ulong ShiftSSE(ulong u)
        {
            return ShiftE(Shift2S(u));
        }

        public static ulong ShiftSSW(ulong u)
        {
            return ShiftW(Shift2S(u));
        }

        public static ulong ShiftWSW(ulong u)
        {
            return Shift2W(ShiftS(u));
        }

        public static ulong ShiftWNW(ulong u)
        {
            return Shift2W(ShiftN(u));
        }

        public static ulong ShiftNNW(ulong u)
        {
            return ShiftW(Shift2N(u));
        }

        /// <summary>
        ///     Uses de Bruijn Sequences to find the least-significant index for the first '1' in a ulong
        /// </summary>
        /// <param name="boardRep">A ulong 'array of bits' representing a chess board.</param>
        /// <returns>index of the least-significant set bit in ulong</returns>
        private static ushort BitScanForward(ulong boardRep)
        {
            const ulong deBruijn = 0x03f79d71b4cb0a89ul;
            return Index64[((boardRep ^ (boardRep - 1)) * deBruijn) >> 58];
        }

        /// <summary>
        ///     Gets the board value of the index in order to compare boards with bitwise math.
        /// </summary>
        /// <param name="idx">The board index, 0(A1)->63(H8)</param>
        /// <returns>ulong representing a square's value on the board</returns>
        public static ulong GetBoardValueOfIndex(ushort idx)
        {
            return (ulong) 1 << idx;
        }

       

        /// <summary>
        ///     Sets a bit (specified by <paramref name="boardIndex">bitIndex</paramref>) on a ulong by ORing the value with 1 SHL
        ///     bitIndex
        /// </summary>
        /// <param name="u">ulong board representation</param>
        /// <param name="boardIndex">index of square to set</param>
        public static ulong SetBit(ulong u, ushort boardIndex)
        {
            return u | (1ul << boardIndex);
        }

        /// <summary>
        ///     Bitwise 'and' operation with the 'not' of the bit's index to clear.
        /// </summary>
        /// <remarks>
        ///     Ex. boardRep = 7, clear bit index of 3: boardRep = 0111b; return boardRep ANDed with NOT((1 shl 3)= 0100b) or
        ///     0111b AND 1011b returns 0011b
        /// </remarks>
        /// <param name="boardRep">A ulong, in this case representing a chessboard</param>
        /// <param name="bitIndex">Index of bit to clear</param>
        /// <returns>
        ///     ulong, <paramref name="boardRep">boardRep</paramref>, with the index of
        ///     <paramref name="bitIndex">bitIndex</paramref> cleared.
        /// </returns>
        public static ulong ClearBit(ulong boardRep, int bitIndex)
        {
            return boardRep & ~(1ul << bitIndex);
        }
        
       
        private void Initialize()
        {
        }

        public static IEnumerable<ulong> GetAllPermutationsOfSetBits(ushort[] setBits, int idx, ulong value)
        {
            value = SetBit(value, setBits[idx]);
            yield return value;
            var index = idx + 1;
            if (index < setBits.Length)
            {
                using (var occupancyPermutations =
                    GetAllPermutationsOfSetBits(setBits, index, value).GetEnumerator())
                {
                    while (occupancyPermutations.MoveNext())
                    {
                        yield return occupancyPermutations.Current;
                    }
                }
            }

            value = ClearBit(value, setBits[idx]);
            yield return value;
            if (index < setBits.Length)
            {
                using (var occupancyPermutations =
                    GetAllPermutationsOfSetBits(setBits, index, value)
                        .GetEnumerator())
                {
                    while (occupancyPermutations.MoveNext())
                    {
                        yield return occupancyPermutations.Current;
                    }
                }
            }
        }
    }
}