using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChessLib.MagicBitboard.Bitwise
{
    public static class BoardConstants
    {
        private static readonly ushort[] _allSquares;
        static BoardConstants()
        {
            _allSquares = Enumerable.Range(0, 64).Select(x => (ushort)x).ToArray();
        }
        public const ulong AFile = 0x0101010101010101;
        public const ulong BFile = 0x0202020202020202;
        public const ulong CFile = 0x0404040404040404;
        public const ulong DFile = 0x0808080808080808;
        public const ulong EFile = 0x1010101010101010;
        public const ulong FFile = 0x2020202020202020;
        public const ulong GFile = 0x4040404040404040;
        public const ulong HFile = 0x8080808080808080;

        public const ulong Rank1 = 0xff;
        public const ulong Rank2 = 0xff00;
        public const ulong Rank3 = 0xff0000;
        public const ulong Rank4 = 0xff000000;
        public const ulong Rank5 = 0xff00000000;
        public const ulong Rank6 = 0xff0000000000;
        public const ulong Rank7 = 0xff000000000000;
        public const ulong Rank8 = 0xff00000000000000;

        public const ulong NotRank1Mask = ~(ulong)0xff;
        public const ulong NotRank2Mask = ~(ulong)0xff00;
        public const ulong NotRank7Mask = ~(ulong)0xff000000000000;
        public const ulong NotRank8Mask = ~(ulong)0xff00000000000000;
        public const ulong NotAFileMask = ~(ulong)0x101010101010101;
        public const ulong NotBFileMask = ~(ulong)0x202020202020202;
        public const ulong NotGFileMask = ~(ulong)0x4040404040404040;
        public const ulong NotHFileMask = ~(ulong)0x8080808080808080;

        

        public static readonly ulong[] Files = new[] { AFile, BFile, CFile, DFile, EFile, FFile, GFile, HFile };

        public static IEnumerable<ushort> AllSquares => _allSquares;
    }
}
