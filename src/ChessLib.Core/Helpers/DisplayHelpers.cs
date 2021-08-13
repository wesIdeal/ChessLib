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

        public static string GetHtmlRepresentation(char? fenChar)
        {
            if (!fenChar.HasValue) return "&nbsp;";
            var color = char.IsUpper(fenChar.Value) ? Color.White : Color.Black;
            Piece piece;
            switch (char.ToLower(fenChar.Value))
            {
                case 'p':
                    piece = Piece.Pawn;
                    break;
                case 'n':
                    piece = Piece.Knight;
                    break;
                case 'b':
                    piece = Piece.Bishop;
                    break;
                case 'r':
                    piece = Piece.Rook;
                    break;
                case 'q':
                    piece = Piece.Queen;
                    break;
                case 'k':
                    piece = Piece.King;
                    break;
                default: throw new Exception("Unexpected PremoveFEN char passed into method GetHtmlRepresentation()");
            }

            return HtmlPieceRepresentations[color][piece];
        }

        public static string MakeBoardTable(this ulong u, ushort pieceIndex, string header = "", string pieceRep = "^",
            string attackSquareRep = "*")
        {
            var boardBits = Convert.ToString((long) u, 2).PadLeft(64, '0');
            var sb = new StringBuilder("<table class=\"chessboard\">\r\n");
            if (string.IsNullOrWhiteSpace(header))
            {
                header = $"Piece at {pieceIndex.IndexToSquareDisplay()}";
            }

            sb.AppendLine($"<caption>{header}<br/>{boardBits}</caption>");
            const string squareFormat = "<td id=\"{1}{0}\" class=\"{3}\">{2}</td>";


            var replacementRank = pieceIndex / 8;
            var replacementFile = pieceIndex % 8;
            for (var r = 7; r >= 0; r--)
            {
                var shiftNumber = r * 8;
                var shifted = u >> shiftNumber;
                var rank = Convert.ToString((ushort) shifted & 0xff, 2).PadLeft(8, '0').Reverse();
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

        public static string PrintBoard(this ulong u, string header = "", char replaceOnesWith = '1')
        {
            if (!string.IsNullOrWhiteSpace(header))
                header = header + "\r\n";
            var sb = new StringBuilder(header + " - ");

            var str = Convert.ToString((long) u, 2).PadLeft(64, '0');
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

        public static string PrintBoardHtml(string htmlBoards)
        {
            var htmlFormat = string.Format(HtmlMain, HtmlStyles, htmlBoards);
            return htmlFormat;
        }

        public static string MakeBoardTable(this ulong u, string header = "", string pieceRep = "*")
        {
            var board = Convert.ToString((long) u, 2).PadLeft(64, '0');
            var sb = new StringBuilder("<table class=\"chessboard\">\r\n");
            if (!string.IsNullOrWhiteSpace(header))
            {
                sb.AppendLine($"<caption>{header}<br/>{board}</caption>");
            }

            const string squareFormat = "<td id=\"{1}{0}\" class=\"{3}\">{2}</td>";

            for (var r = Rank.R8; r >= Rank.R1; r--)
            {
                var rank = (ushort) r;
                sb.AppendLine($"<tr id=\"rank{rank}\">");

                for (var f = File.A; f <= File.H; f++)
                {
                    var file = (ushort) f;
                    var squareIndex = rank * 8 + file;
                    var strIndex = 63 - squareIndex;
                    var pieceAtSquare = board[strIndex] == '1' ? pieceRep : "&nbsp;";
                    sb.AppendFormat(squareFormat, f.ToString(), rank, pieceAtSquare,
                        board[rank * 8 + file] == '1' ? "altColor" : "");
                }

                sb.Append("\r\n</tr>\r\n");
            }

            sb.AppendLine("</table>");
            return sb.ToString();
        }

        public static string MakeBoardTableFromFEN(string fen, string header = "")
        {
            var sb = new StringBuilder("<table class=\"chessboard\">\r\n");
            if (!string.IsNullOrWhiteSpace(header))
            {
                sb.AppendLine($"<caption>{header}</caption>");
            }

            const string squareFormat = "<td id=\"{0}\">{1}</td>";

            for (var r = Rank.R8; r >= Rank.R1; r--)
            {
                var rank = ((ushort) r).RankCompliment();
                sb.AppendLine($"<tr id=\"rank{rank}\">");

                for (var f = File.A; f <= File.H; f++)
                {
                    var file = (int) f;
                    var piece = fen[((ushort) r).RankCompliment() * 8 + (ushort) f];
                    var pieceRep = GetHtmlRepresentation(piece);

                    sb.AppendFormat(squareFormat, f + r.ToString(), pieceRep ?? "&nbsp;");
                }

                sb.Append("\r\n</tr>\r\n");
            }

            sb.AppendLine("</table>");
            return sb.ToString();
        }

        public static string MakeBoardTable(this ulong[] uArr, ushort pieceIndex, string header = "",
            string pieceRep = "^", string attackSquareRep = "*")
        {
            var sb = new StringBuilder();
            uArr.ToList().ForEach(x => sb.AppendLine(x.MakeBoardTable(pieceIndex, header, pieceRep, attackSquareRep)));
            return PrintBoardHtml(sb.ToString());
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


        #region Html Strings

        private const string HtmlMain = @"<!DOCTYPE html>
<html>
    <head>
        <title>Chess Boards</title>
        {0}
    </head>
    <body>
    {1}
    </body>
</html>";

        private const string HtmlStyles = @"
<style>
    * 
    {
        margin: 0; 
        padding: 0; 
    }

    table { 
        border-collapse: collapse; 
        border-spacing: 0; 
    }

    .chessboard { 
        padding: 0px; 
        margin: 0 auto; 
        border: 2px solid #181818; 
    }

    .chessboard tr td {
        font-size: 44px;
        width: 60px; 
        height: 60px; 
        text-align: center;
        vertical-align: middle;
        
    }

    .chessboard tr:nth-child(2n) td:nth-child(2n+1) { 
        background: #9f9f9f; 
    }

    .chessboard tr:nth-child(2n+1) td:nth-child(2n) { 
        background: #9f9f9f; 
    } 
    

</style>
";

        #endregion
    }
}