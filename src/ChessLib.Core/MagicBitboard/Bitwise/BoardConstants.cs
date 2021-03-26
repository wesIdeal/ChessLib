using System.Collections.Generic;
using System.Linq;
using ChessLib.Core.Types.Enums;

namespace ChessLib.Core.MagicBitboard.Bitwise
{
    public static class BoardConstants
    {
        private static readonly ushort[] AllSquareIndexes;
        static BoardConstants()
        {
            AllSquareIndexes = Enumerable.Range(0, 64).Select(x => (ushort)x).ToArray();
        }

        /// <summary>
        ///     Contains boardIndex values for a boardIndex index
        /// </summary>
        public static readonly ulong[] IndividualSquares =
        {
            0x1, 0x2, 0x4, 0x8, 0x10, 0x20, 0x40, 0x80,
            0x100, 0x200, 0x400, 0x800, 0x1000, 0x2000, 0x4000,
            0x8000, 0x10000, 0x20000, 0x40000, 0x80000, 0x100000, 0x200000,
            0x400000, 0x800000, 0x1000000, 0x2000000, 0x4000000, 0x8000000, 0x10000000,
            0x20000000, 0x40000000, 0x80000000, 0x100000000, 0x200000000, 0x400000000, 0x800000000,
            0x1000000000, 0x2000000000, 0x4000000000, 0x8000000000, 0x10000000000, 0x20000000000, 0x40000000000,
            0x80000000000, 0x100000000000, 0x200000000000, 0x400000000000, 0x800000000000, 0x1000000000000,
            0x2000000000000,
            0x4000000000000, 0x8000000000000, 0x10000000000000, 0x20000000000000, 0x40000000000000, 0x80000000000000,
            0x100000000000000,
            0x200000000000000, 0x400000000000000, 0x800000000000000, 0x1000000000000000, 0x2000000000000000,
            0x4000000000000000, 0x8000000000000000
        };

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


        public const ulong NotRank1Mask = 0xFFFFFFFFFFFFFF00UL;
        public const ulong NotRank2Mask = 0xFFFFFFFFFFFF00FFUL;
        public const ulong NotRank7Mask = 0xFF00FFFFFFFFFFFFUL;
        public const ulong NotRank8Mask = 0xFFFFFFFFFFFFFFUL;
        public const ulong NotAFileMask = 0xFEFEFEFEFEFEFEFEUL;
        public const ulong NotBFileMask = 0xFDFDFDFDFDFDFDFDUL;
        public const ulong NotGFileMask = 0xBFBFBFBFBFBFBFBFUL;
        public const ulong NotHFileMask = 0x7F7F7F7F7F7F7F7FUL;

        /// <summary>
        ///     Contains set bits for each rank
        /// </summary>
        public static ulong[] RankMasks =
        {
            Rank1 , Rank2 , Rank3 , Rank4 , Rank5 , Rank6 , Rank7 , Rank8
        };

        /// <summary>
        ///     Contains set bits for each file
        /// </summary>
        public static ulong[] FileMasks =
        {
            AFile, BFile, CFile, DFile, EFile, FFile, GFile, HFile
        };

        public static ushort WhiteKingSquare = 4;
        public static ushort BlackKingSquare = 60;
        public const int Pawn = (int)Piece.Pawn;
        public const int Bishop = (int)Piece.Bishop;
        public const int Knight = (int)Piece.Knight;
        public const int Rook = (int)Piece.Rook;
        public const int Queen = (int)Piece.Queen;
        public const int King = (int)Piece.King;
        public const int White = (int)Color.White;

        public const int Black = (int)Color.Black;

        public static IEnumerable<ushort> AllSquares => AllSquareIndexes;
    }
}
