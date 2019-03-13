using System;
using System.Collections.Generic;
using System.Text;

namespace MagicBitboard.Helpers
{
    public static class ShiftHelpers
    {
        #region Const File and Rank Masks
        private const ulong NotRank1Mask = ~(ulong)0xff;
        private const ulong NotRank2Mask = ~(ulong)0xff00;
        private const ulong NotRank7Mask = ~(ulong)0xff000000000000;
        private const ulong NotRank8Mask = ~(ulong)0xff00000000000000;
        private const ulong NotFileAMask = ~(ulong)0x101010101010101;
        private const ulong NotFileBMask = ~(ulong)0x202020202020202;
        private const ulong NotFileGMask = ~(ulong)0x4040404040404040;
        private const ulong NotFileHMask = ~(ulong)0x8080808080808080;
        #endregion

        public static ulong ShiftN(this ulong u) { return (u & NotRank8Mask) << (8); }
        public static ulong ShiftE(this ulong u) { return (u & NotFileHMask) << 1; }
        public static ulong ShiftS(this ulong u) { return (u & NotRank1Mask) >> 8; }
        public static ulong ShiftW(this ulong u) { return (u & NotFileAMask) >> 1; }

        public static ulong Shift2N(this ulong u) { return (u & (NotRank7Mask & NotRank8Mask)) << 16; }
        public static ulong Shift2E(this ulong u) { return (u & (NotFileGMask & NotFileHMask)) << 2; }
        public static ulong Shift2S(this ulong u) { return (u & (NotRank1Mask & NotRank2Mask)) >> 16; }
        public static ulong Shift2W(this ulong u) { return (u & NotFileBMask & NotFileAMask) >> 2; }

        public static ulong ShiftNE(this ulong u) { return u.ShiftN().ShiftE(); }
        public static ulong ShiftSE(this ulong u) { return u.ShiftS().ShiftE(); }
        public static ulong ShiftSW(this ulong u) { return u.ShiftS().ShiftW(); }
        public static ulong ShiftNW(this ulong u) { return u.ShiftN().ShiftW(); }

        public static ulong ShiftNNE(this ulong u) { return u.Shift2N().ShiftE(); }
        public static ulong ShiftENE(this ulong u) { return u.Shift2E().ShiftN(); }
        public static ulong ShiftESE(this ulong u) { return u.ShiftS().Shift2E(); }
        public static ulong ShiftSSE(this ulong u) { return u.Shift2S().ShiftE(); }
        public static ulong ShiftSSW(this ulong u) { return u.Shift2S().ShiftW(); }
        public static ulong ShiftWSW(this ulong u) { return u.ShiftS().Shift2W(); }
        public static ulong ShiftWNW(this ulong u) { return u.ShiftN().Shift2W(); }
        public static ulong ShiftNNW(this ulong u) { return u.Shift2N().ShiftW(); }
    }
}
