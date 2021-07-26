using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace ChessLib.Graphics.TextDisplay
{
    public sealed class DisplayService : IDisplayService
    {
        /// <summary>Gets the hex display of a long (debugging/display) </summary>
        /// <param name="u">long to get display from</param>
        /// <param name="appendHexNotation">append '0x' to the representation</param>
        /// <param name="pad">pad length to a certain size</param>
        /// <param name="padSize">size to pad</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToHexDisplay(ulong u, bool appendHexNotation = true, bool pad = false,
            int padSize = 64)
        {
            var str = Convert.ToString((long)u, 16);
            if (pad) str = str.PadLeft(padSize, '0');
            if (appendHexNotation) str = "0x" + str;
            return str;
        }


        public string PrintBoard(ulong u, string header = "", char replaceOnesWith = '1')
        {
            if (!string.IsNullOrWhiteSpace(header))
                header = header + "\r\n";
            var sb = new StringBuilder(header + " - ");

            var str = Convert.ToString((long)u, 2).PadLeft(64, '0');
            sb.Append(str + "\r\n");


            var footerHeader = "";

            for (var c = 'a'; c <= 'h'; c++)
                footerHeader += "  " + c;
            var boardBorder = string.Concat(Enumerable.Repeat("-", footerHeader.Length + 3));
            footerHeader = " " + footerHeader;
            sb.AppendLine(footerHeader);
            sb.AppendLine(boardBorder);

            for (var i = 7; i >= 0; i--)
            {
                var rankString = (i + 1).ToString();
                var rank = str.Skip(i * 8).Take(8).Select(x => x.ToString().Replace('1', replaceOnesWith)).Reverse();
                sb.AppendLine(rankString + " | " + string.Join(" | ", rank) + " |");
            }

            sb.AppendLine(boardBorder);
            sb.AppendLine(footerHeader);
            return sb.ToString();
        }

        public static string MakeBoardTable(ulong u, ushort pieceIndex, string header = "", string pieceRep = "^",
            string attackSquareRep = "*")
        {
            var boardBits = Convert.ToString((long)u, 2).PadLeft(64, '0');
            var sb = new StringBuilder("<table class=\"chessboard\">\r\n");
            if (string.IsNullOrWhiteSpace(header))
            {
                header = $"Piece at {IndexToSquareDisplay(pieceIndex)}";
            }

            sb.AppendLine($"<caption>{header}<br/>{boardBits}</caption>");
            const string squareFormat = "<td id=\"{1}{0}\" class=\"{3}\">{2}</td>";

            var replacementRank = pieceIndex / 8;
            var replacementFile = pieceIndex % 8;
            for (var r = 7; r >= 0; r--)
            {
                var shiftNumber = r * 8;
                var shifted = u >> shiftNumber;
                var rank = Convert.ToString((ushort)shifted & 0xff, 2).PadLeft(8, '0').Reverse();
                var file = 0;

                foreach (var p in rank)
                {
                    var squareContents = "";
                    if (r == replacementRank && file == replacementFile)
                    {
                        squareContents = pieceRep;
                    }
                    else if (p == '1')
                    {
                        squareContents = attackSquareRep;
                    }

                    sb.AppendFormat(squareFormat, file, r, squareContents, "");
                    file++;
                }

                sb.Append("\r\n</tr>\r\n");
            }

            sb.AppendLine("</table>");
            return sb.ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char IndexToFileDisplay(ushort i)
        {
            return (char)('a' + i % 8);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char IndexToRankDisplay(ushort i)
        {
            return (char)('1' + i / 8);
        }

        /// <summary>
        ///     Gets a human-readable square display based on the board index.
        /// </summary>
        /// <param name="i">Index of square, from 0(A1) to 63(H8)</param>
        /// <returns>A square display; ex. a2, c4, f6</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string IndexToSquareDisplay(ushort i)
        {
            return $"{IndexToFileDisplay(i)}{IndexToRankDisplay(i)}";
        }

        /// <summary>
        ///     Gets a human-readable square display based on the board index.
        /// </summary>
        /// <param name="i">Index of square, from 0(A1) to 63(H8)</param>
        /// <returns>A square display; ex. a2, c4, f6</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string IndexToSquareDisplay(int i)
        {
            return $"{IndexToFileDisplay((ushort)i)}{IndexToRankDisplay((ushort)i)}";
        }
    }
}