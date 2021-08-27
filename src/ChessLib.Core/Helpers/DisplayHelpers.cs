using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using ChessLib.Core.Types.Enums;

namespace ChessLib.Core.Helpers
{
    public static class DisplayHelpers
    {
        public static readonly Dictionary<Color, Dictionary<Piece, string>> HtmlPieceRepresentations =
            new Dictionary<Color, Dictionary<Piece, string>>();

        static DisplayHelpers()
        {
            InitializeHtmlPieceRepresentations();
        }

        private static void InitializeHtmlPieceRepresentations()
        {
            HtmlPieceRepresentations.Add(Color.White, new Dictionary<Piece, string>());
            HtmlPieceRepresentations.Add(Color.Black, new Dictionary<Piece, string>());
            var whiteStart = 9817;
            var blackStart = 9823;
            foreach (var p in (Piece[]) Enum.GetValues(typeof(Piece)))
            {
                HtmlPieceRepresentations[Color.White].Add(p, $"&#{whiteStart};");
                whiteStart--;
            }

            foreach (var p in (Piece[]) Enum.GetValues(typeof(Piece)))
            {
                HtmlPieceRepresentations[Color.Black].Add(p, $"&#{blackStart};");
                blackStart--;
            }
        }

        public static string GetDisplayBits(this ulong u)
        {
            var str = Convert.ToString((long) u, 2).PadLeft(64, '0');
            var sb = new StringBuilder();
            for (var i = 0; i < 8; i++)
            {
                sb.AppendLine(string.Join(" ", str.Skip(i * 8).Take(8).Reverse().ToArray()));
            }

            return sb.ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char IndexToFileDisplay(this ushort i)
        {
            return (char) ('a' + i % 8);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char IndexToRankDisplay(this ushort i)
        {
            return (char) ('1' + i / 8);
        }

        /// <summary>
        ///     Gets a human-readable square display based on the board index.
        /// </summary>
        /// <param name="i">Index of square, from 0(A1) to 63(H8)</param>
        /// <returns>A square display; ex. a2, c4, f6</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string IndexToSquareDisplay(this ushort i)
        {
            return $"{i.IndexToFileDisplay()}{i.IndexToRankDisplay()}";
        }

        /// <summary>
        ///     Gets a human-readable square display based on the board index.
        /// </summary>
        /// <param name="i">Index of square, from 0(A1) to 63(H8)</param>
        /// <returns>A square display; ex. a2, c4, f6</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string IndexToSquareDisplay(this int i)
        {
            return $"{((ushort) i).IndexToFileDisplay()}{((ushort) i).IndexToRankDisplay()}";
        }

    }
}