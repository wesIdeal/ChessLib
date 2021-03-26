using System.Collections.Generic;
using System.Diagnostics;

namespace ChessLib.Core.Helpers
{
    public static class BitHelpers
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

        /// <summary>
        ///     Uses de Bruijn Sequences to find the least-significant index for the first '1' in a ulong
        /// </summary>
        /// <param name="boardRep">A ulong 'array of bits' representing a chess board.</param>
        /// <returns>index of the least-significant set bit in ulong</returns>
        public static ushort BitScanForward(ulong boardRep)
        {
            const ulong deBruijn = 0x03f79d71b4cb0a89ul;
            Debug.Assert(boardRep != 0);
            return Index64[((boardRep ^ (boardRep - 1)) * deBruijn) >> 58];
        }

        /// <summary>
        ///     Gets the board value of the index in order to compare boards with bitwise math.
        /// </summary>
        /// <param name="idx">The board index, 0(A1)->63(H8)</param>
        /// <returns>ulong representing a square's value on the board</returns>
        public static ulong GetBoardValueOfIndex(this ushort idx)
        {
            return (ulong) 1 << idx;
        }

        ///// <summary>
        /////     Flips a board about the 4th and 5th ranks
        ///// </summary>
        ///// <param name="board">bitboard representation</param>
        ///// <returns>A flipped board</returns>
        //public static ulong FlipVertically(in ulong board)
        //{
        //    var x = board;
        //    return (x << 56) |
        //           ((x << 40) & 0x00ff000000000000) |
        //           ((x << 24) & 0x0000ff0000000000) |
        //           ((x << 8) & 0x000000ff00000000) |
        //           ((x >> 8) & 0x00000000ff000000) |
        //           ((x >> 24) & 0x0000000000ff0000) |
        //           ((x >> 40) & 0x000000000000ff00) |
        //           (x >> 56);
        //}

        ///// <summary>
        /////     Gets the flipped index value, ie. A1 -> H1
        ///// </summary>
        ///// <param name="idx"></param>
        ///// <returns></returns>
        //public static ushort FlipIndexVertically(this ushort idx)
        //{
        //    var rank = idx.RankFromIdx();
        //    var file = idx.FileFromIdx();
        //    var rankCompliment = rank.RankCompliment();
        //    return (ushort) (rankCompliment * 8 + file);
        //}

   
        /// <summary>
        ///     Determines if a bit index is set in a ulong
        /// </summary>
        /// <param name="u"></param>
        /// <param name="bitIndex"></param>
        /// <returns></returns>
        public static bool IsBitSet(this ulong u, ushort bitIndex)
        {
            var comparison = bitIndex.GetBoardValueOfIndex();
            return (comparison & u) == comparison;
        }

        /// <summary>
        ///     Gets an array of bit indexes set to '1'
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public static ushort[] GetSetBits(this ulong u)
        {
            var rv = new List<ushort>(64); //Set max capacity to 64, since our 'array of bits' will be no larger.
            while (u != 0)
            {
                rv.Add(BitScanForward(u));
                u &= u - 1;
            }

            return rv.ToArray();
        }

        /// <summary>
        ///     Sets a bit (specified by <paramref name="bitIndex">bitIndex</paramref>) on a ulong by ORing the value with 1 SHL
        ///     bitIndex
        /// </summary>
        /// <param name="u">ulong board representation</param>
        /// <param name="bitIndex">index of bit to set</param>
        public static ulong SetBit(this ulong u, ushort bitIndex)
        {
            return u | (1ul << bitIndex);
        }




        /// <summary>
        ///     Gets a count of bits set in a ulong
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public static ushort CountSetBits(this ulong u)
        {
            ushort counter = 0;
            while (u != 0)
            {
                counter++;
                u &= u - 1;
            }
            return counter;
        }

        #region Directional Shifts

        #region Const File and Rank Masks

        private const ulong NotRank1Mask = 0xFFFFFFFFFFFFFF00UL;
        private const ulong NotRank2Mask = 0xFFFFFFFFFFFF00FFUL;
        private const ulong NotRank7Mask = 0xFF00FFFFFFFFFFFFUL;
        private const ulong NotRank8Mask = 0xFFFFFFFFFFFFFFUL;
        private const ulong NotFileAMask = 0xFEFEFEFEFEFEFEFEUL;
        private const ulong NotFileBMask = 0xFDFDFDFDFDFDFDFDUL;
        private const ulong NotFileGMask = 0xBFBFBFBFBFBFBFBFUL;
        private const ulong NotFileHMask = 0x7F7F7F7F7F7F7F7FUL;

        #endregion

        #region Shift Methods

        public static ulong ShiftN(this ulong u)
        {
            return (u & NotRank8Mask) << 8;
        }

        public static ulong ShiftE(this ulong u)
        {
            return (u & NotFileHMask) << 1;
        }

        public static ulong ShiftS(this ulong u)
        {
            return (u & NotRank1Mask) >> 8;
        }

        public static ulong ShiftW(this ulong u)
        {
            return (u & NotFileAMask) >> 1;
        }

        public static ulong Shift2N(this ulong u)
        {
            return (u & NotRank7Mask & NotRank8Mask) << 16;
        }

        public static ulong Shift2E(this ulong u)
        {
            return (u & NotFileGMask & NotFileHMask) << 2;
        }

        public static ulong Shift2S(this ulong u)
        {
            return (u & NotRank1Mask & NotRank2Mask) >> 16;
        }

        public static ulong Shift2W(this ulong u)
        {
            return (u & NotFileBMask & NotFileAMask) >> 2;
        }

        public static ulong ShiftNE(this ulong u)
        {
            return u.ShiftN().ShiftE();
        }

        public static ulong ShiftSE(this ulong u)
        {
            return u.ShiftS().ShiftE();
        }

        public static ulong ShiftSW(this ulong u)
        {
            return u.ShiftS().ShiftW();
        }

        public static ulong ShiftNW(this ulong u)
        {
            return u.ShiftN().ShiftW();
        }

        public static ulong ShiftNNE(this ulong u)
        {
            return u.Shift2N().ShiftE();
        }

        public static ulong ShiftENE(this ulong u)
        {
            return u.Shift2E().ShiftN();
        }

        public static ulong ShiftESE(this ulong u)
        {
            return u.ShiftS().Shift2E();
        }

        public static ulong ShiftSSE(this ulong u)
        {
            return u.Shift2S().ShiftE();
        }

        public static ulong ShiftSSW(this ulong u)
        {
            return u.Shift2S().ShiftW();
        }

        public static ulong ShiftWSW(this ulong u)
        {
            return u.ShiftS().Shift2W();
        }

        public static ulong ShiftWNW(this ulong u)
        {
            return u.ShiftN().Shift2W();
        }

        public static ulong ShiftNNW(this ulong u)
        {
            return u.Shift2N().ShiftW();
        }

        #endregion

        #endregion
    }
}