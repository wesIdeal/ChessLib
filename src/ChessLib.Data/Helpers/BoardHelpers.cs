﻿using ChessLib.Data.Types;
using System;
using System.Runtime.CompilerServices;
namespace ChessLib.Data.Helpers
{
    public static class BoardHelpers
    {

        public static Color Toggle(this Color c) => c == Color.White ? Color.Black : Color.White;

        public static readonly ulong[,] IndividualSquares = new ulong[8, 8];
        public static ulong[] FileMasks = new ulong[8];
        public static ulong[] RankMasks = new ulong[8];
        private readonly static ulong[,] _inBetween = new ulong[64, 64];

        static BoardHelpers()
        {
            InitializeFileMasks();
            InitializRankMasks();
            InitializeIndividualSquares();
            InitializeInBetween();
        }

        public static void InitializeInBetween()
        {
            for (var f = 0; f < 64; f++)
            {
                for (var t = f; t < 64; t++)
                {
                    const long m1 = (-1);
                    const long a2a7 = (0x0001010101010100);
                    const long b2g7 = (0x0040201008040200);
                    const long h1b7 = (0x0002040810204080);
                    long btwn, line, rank, file;

                    btwn = (m1 << f) ^ (m1 << t);
                    file = (t & 7) - (f & 7);
                    rank = ((t | 7) - f) >> 3;
                    line = ((file & 7) - 1) & a2a7; /* a2a7 if same file */
                    line += 2 * (((rank & 7) - 1) >> 58); /* b1g1 if same rank */
                    line += (((rank - file) & 15) - 1) & b2g7; /* b2g7 if same diagonal */
                    line += (((rank + file) & 15) - 1) & h1b7; /* h1b7 if same antidiag */
                    line *= btwn & -btwn; /* mul acts like shift by smaller square */
                    _inBetween[f, t] = (ulong)(line & btwn);   /* return the bits on that line in-between */

                }
            }
        }

        public static ulong InBetween(int from, int to)
        {
            var square1 = Math.Min(from, to);
            var square2 = Math.Max(from, to);
            return _inBetween[square1, square2];
        }


        #region Initialization

        private static void InitializeFileMasks()
        {
            ulong start = 0x101010101010101;

            for (int f = 0; f <= 7; f++)
            {
                FileMasks[f] = start << f;
            }
        }

        private static void InitializRankMasks()
        {
            var start = (ulong)0xFF;
            for (int r = 0; r <= 7; r++)
            {
                RankMasks[r] = start << (r * 8);
            }
        }

        private static void InitializeIndividualSquares()
        {
            for (int i = 0; i < 64; i++)
            {
                IndividualSquares[i / 8, i % 8] = (ulong)1 << i;
            }
        }

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
            var r = square / 8;
            return (Rank)(square / 8);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint RankFromIdx(this uint idx) => idx / 8;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint FileFromIdx(this uint idx) => idx % 8;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort RankFromIdx(this ushort idx) => (ushort)(idx / 8);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort FileFromIdx(this ushort idx) => (ushort)(idx % 8);

        public static ushort RankCompliment(this ushort rank) => (ushort)Math.Abs(rank - 7);

        public static ushort IndexVerticalCompliment(ushort idx) => (ushort)((idx.RankFromIdx().RankCompliment() * 8) + idx.FileFromIdx());
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

        public static ulong RotateLeft(ulong x)
        {
            return FlipDiagA1H8(FlipVertically(x));
        }

        public static ulong FlipDiagA1H8(ulong x)
        {
            ulong t;
            const ulong k1 = 0x5500550055005500;
            const ulong k2 = 0x3333000033330000;
            const ulong k4 = 0x0f0f0f0f00000000;
            t = k4 & (x ^ (x << 28));
            x ^= t ^ (t >> 28);
            t = k2 & (x ^ (x << 14));
            x ^= t ^ (t >> 14);
            t = k1 & (x ^ (x << 7));
            x ^= t ^ (t >> 7);
            return x;
        }


    }
}
