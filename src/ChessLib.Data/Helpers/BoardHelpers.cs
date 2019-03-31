using ChessLib.Data.Types;
using System;
using System.Runtime.CompilerServices;
namespace ChessLib.Data.Helpers
{
    public static class BoardHelpers
    {

        public static Color Toggle(this Color c) => c == Color.White ? Color.Black : Color.White;

        public static readonly Board IndividualSquares =
        new Board()
        {
            MoveBoard = new ulong[]{
            0x1, 0x2, 0x4, 0x8, 0x10, 0x20, 0x40, 0x80,
            0x100, 0x200, 0x400, 0x800, 0x1000, 0x2000, 0x4000,
            0x8000, 0x10000, 0x20000, 0x40000, 0x80000, 0x100000, 0x200000,
            0x400000, 0x800000, 0x1000000, 0x2000000, 0x4000000, 0x8000000, 0x10000000,
            0x20000000, 0x40000000, 0x80000000, 0x100000000, 0x200000000, 0x400000000, 0x800000000,
            0x1000000000, 0x2000000000, 0x4000000000, 0x8000000000, 0x10000000000, 0x20000000000, 0x40000000000,
            0x80000000000, 0x100000000000, 0x200000000000, 0x400000000000, 0x800000000000, 0x1000000000000, 0x2000000000000,
            0x4000000000000, 0x8000000000000, 0x10000000000000, 0x20000000000000, 0x40000000000000, 0x80000000000000, 0x100000000000000,
            0x200000000000000, 0x400000000000000, 0x800000000000000, 0x1000000000000000, 0x2000000000000000, 0x4000000000000000, 0x8000000000000000,
        }
        };

        public static ulong[] RankMasks = {
            0xff,               //R1
            0xff00,             //R2
            0xff0000,           //R3
            0xff000000,         //R4
            0xff00000000,       //R5
            0xff0000000000,     //R6
            0xff000000000000,   //R7
            0xff00000000000000  //R8
        };

        public static ulong[] FileMasks = {
            0x101010101010101,  //A
            0x202020202020202,  //B
            0x404040404040404,  //C
            0x808080808080808,  //D
            0x1010101010101010, //E
            0x2020202020202020, //F
            0x4040404040404040, //G
            0x8080808080808080  //H
        };

        private static readonly ulong[,] ArrInBetween = new ulong[64, 64];

        static BoardHelpers()
        {
            InitializeInBetween();
        }

        private static void InitializeInBetween()
        {
            for (var f = 0; f < 64; f++)
            {
                for (var t = f; t < 64; t++)
                {
                    const long m1 = (-1);
                    const long aFileBorder = (0x0001010101010100);
                    const long b2DiagonalBorder = (0x0040201008040200);
                    const long hFileBorder = (0x0002040810204080);

                    var between = (m1 << f) ^ (m1 << t);
                    long file = (t & 7) - (f & 7);
                    long rank = ((t | 7) - f) >> 3;
                    var line = ((file & 7) - 1) & aFileBorder;
                    line += 2 * (((rank & 7) - 1) >> 58); /* b1g1 if same rank */
                    line += (((rank - file) & 15) - 1) & b2DiagonalBorder; /* b2g7 if same diagonal */
                    line += (((rank + file) & 15) - 1) & hFileBorder; /* h1b7 if same anti-diagonal */
                    line *= between & -between; /* mul acts like shift by smaller square */
                    ArrInBetween[f, t] = (ulong)(line & between);   /* return the bits on that line in-between */
                }
            }
        }

        public static ulong InBetween(int from, int to)
        {
            var square1 = Math.Min(from, to);
            var square2 = Math.Max(from, to);
            return ArrInBetween[square1, square2];
        }


        #region Initialization

        //private static void InitializeIndividualSquares()
        //{
        //    for (int i = 0; i < 64; i++)
        //    {
        //        IndividualSquares[i / 8, i % 8] = (ulong)1 << i;
        //    }
        //}

        #endregion

        #region Enum ToInt() methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt(this Color c) => (int)c;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt(this Piece p) => (int)p;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt(this File f) => (int)f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt(this Rank r) => (int)r;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToHexDisplay(this ulong u, bool appendHexNotation = true, bool pad = false, int padSize = 64)
        {
            var str = Convert.ToString((long)u, 16);
            if (pad)
            {
                str = str.PadLeft(padSize, '0');
            }
            if (appendHexNotation)
            {
                str = "0x" + str;
            }
            return str;
        }
        #endregion

        #region Array Position to Friendly Position Helpers

        public static ushort? SquareTextToIndex(this string square)
        {
            if (square.Trim() == "-")
            {
                return null;
            }
            if (square.Length != 2)
            {
                throw new ArgumentException($"Square passed to SquareTextToIndex(), {square} has an invalid length.");
            }
            var file = char.ToLower(square[0]);
            var rank = ushort.Parse(square[1].ToString());
            if (!char.IsLetter(file) || file < 'a' || file > 'h')
            {
                throw new ArgumentException("File portion of square-text should be a letter, between 'a' and 'h'.");
            }
            if (rank < 1 || rank > 8)
            {
                throw new ArgumentException("Rank portion of square-text should be a digit with a value between 1 and 8.");
            }
            var rankMultiplier = rank - 1;
            return (ushort)((rankMultiplier * 8) + file - 'a');
        }

        public static ushort RankAndFileToIndex(ushort rank, ushort file)
        {
            return (ushort)((rank * 8) + file);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static File GetFile(this int square)
        {
            return (File)(square % 8);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rank GetRank(this int square)
        {
            return (Rank)(square / 8);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort RankFromIdx(this ushort idx) => (ushort)(idx / 8);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort FileFromIdx(this ushort idx) => (ushort)(idx % 8);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort RankCompliment(this ushort rank) => (ushort)Math.Abs(rank - 7);

        #endregion

        public static ulong FlipVertically(this ulong board)
        {
            var x = board;
            return (x << 56) |
                    ((x << 40) & 0x00ff000000000000) |
                    ((x << 24) & 0x0000ff0000000000) |
                    ((x << 8) & 0x000000ff00000000) |
                    ((x >> 8) & 0x00000000ff000000) |
                    ((x >> 24) & 0x0000000000ff0000) |
                    ((x >> 40) & 0x000000000000ff00) |
                    (x >> 56);
        }

        public static ushort FlipIndexVertically(this ushort idx)
        {
            var rank = idx.RankFromIdx();
            var file = idx.FileFromIdx();
            var rankCompliment = rank.RankCompliment();
            return (ushort)((rankCompliment * 8) + file);
        }
    }
}
