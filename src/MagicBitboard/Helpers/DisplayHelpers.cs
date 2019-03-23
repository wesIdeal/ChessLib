using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using MagicBitboard.Enums;
namespace MagicBitboard.Helpers
{
    public static class DisplayHelpers
    {
        public readonly static Dictionary<Color, Dictionary<Piece, string>> HtmlPieceRepresentations;

        static DisplayHelpers()
        {
            HtmlPieceRepresentations = new Dictionary<Color, Dictionary<Piece, string>>();
            InitializeHtmlPieceRepresentations();
        }

        private static void InitializeHtmlPieceRepresentations()
        {
            HtmlPieceRepresentations.Add(Color.White, new Dictionary<Piece, string>());
            HtmlPieceRepresentations.Add(Color.Black, new Dictionary<Piece, string>());
            var whiteStart = 9817;
            var blackStart = 9823;
            foreach (var p in (Piece[])Enum.GetValues(typeof(Piece)))
            {
                HtmlPieceRepresentations[Color.White].Add(p, $"&#{whiteStart};");
                whiteStart--;
            }
            foreach (var p in (Piece[])Enum.GetValues(typeof(Piece)))
            {
                HtmlPieceRepresentations[Color.Black].Add(p, $"&#{blackStart};");
                blackStart--;
            }
        }

        public static string GetDisplayBits(this ulong u)
        {
            var str = Convert.ToString((long)u, 2).PadLeft(64, '0');
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 8; i++)
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
                case 'p': piece = Piece.Pawn; break;
                case 'n': piece = Piece.Knight; break;
                case 'b': piece = Piece.Bishop; break;
                case 'r': piece = Piece.Rook; break;
                case 'q': piece = Piece.Queen; break;
                case 'k': piece = Piece.King; break;
                default: throw new Exception("Unexpected FEN char passed into method GetHtmlRepresentation()");
            }
            return HtmlPieceRepresentations[color][piece];
        }

        public static string MakeBoardTable(this ulong u, ushort pieceIndex, string header = "", string pieceRep = "^", string attackSquareRep = "*")
        {
            string boardBits = Convert.ToString((long)u, 2).PadLeft(64, '0');
            var board = new List<string>();

            var sb = new StringBuilder("<table class=\"chessboard\">\r\n");
            if (header == string.Empty)
            {
                header = $"Piece at {pieceIndex.IndexToSquareDisplay()}";
            }

            sb.AppendLine($"<caption>{header}<br/>{boardBits}</caption>");
            const string squareFormat = "<td id=\"{1}{0}\" class=\"{3}\">{2}</td>";

            var array = new List<string>();
            var replacementRank = pieceIndex / 8;
            var replacementFile = pieceIndex % 8;
            for (int r = 7; r >= 0; r--)
            {

                var shiftNumber = r * 8;
                var shifted = u >> shiftNumber;
                var rank = Convert.ToString((ushort)shifted & 0xff, 2).PadLeft(8, '0').Reverse();
                var file = 0;

                foreach (var p in rank)
                {
                    string squareContents = "";
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

            var str = Convert.ToString((long)u, 2).PadLeft(64, '0');
            sb.Append(str + "\r\n");

            var lRanks = new List<string>();
            var footerHeader = "";

            for (char c = 'a'; c <= 'h'; c++)
                footerHeader += "  " + c.ToString();
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
            var htmlFormat = string.Format(htmlMain, htmlStyles, htmlBoards);
            return htmlFormat;
        }

        public static string MakeBoardTable(this ulong u, string header = "", string pieceRep = "*")
        {
            string board = Convert.ToString((long)u, 2).PadLeft(64, '0');
            var sb = new StringBuilder("<table class=\"chessboard\">\r\n");
            if (header != string.Empty)
            {
                sb.AppendLine($"<caption>{header}<br/>{board}</caption>");
            }
            const string squareFormat = "<td id=\"{1}{0}\" class=\"{3}\">{2}</td>";

            for (Rank r = Enums.Rank.R8; r >= Enums.Rank.R1; r--)
            {
                var rank = Math.Abs(r.ToInt() - 7);
                sb.AppendLine($"<tr id=\"rank{rank}\">");

                for (File f = File.A; f <= File.H; f++)
                {
                    var file = f.ToInt();

                    var pieceAtSquare = board[(rank * 8) + file] == '1' ? pieceRep : "&nbsp;";
                    sb.AppendFormat(squareFormat, f.ToString(), rank, pieceAtSquare, board[(rank * 8) + file] == '1' ? "altColor" : "");
                }
                sb.Append("\r\n</tr>\r\n");
            }
            sb.AppendLine("</table>");
            return sb.ToString();
        }

        public static string MakeBoardTable(this ulong[] uArr, ushort pieceIndex, string header = "", string pieceRep = "^", string attackSquareRep = "*")
        {
            StringBuilder sb = new StringBuilder();
            uArr.ToList().ForEach(x => sb.AppendLine(x.MakeBoardTable(pieceIndex, header, pieceRep, attackSquareRep)));
            return PrintBoardHtml(sb.ToString());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char IndexToFileDisplay(this ushort i) => (char)('a' + (i % 8));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char IndexToRankDisplay(this ushort i) => (char)('1' + (i / 8));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string IndexToSquareDisplay(this ushort i) => $"{i.IndexToFileDisplay()}{i.IndexToRankDisplay()}";


        #region Html Strings
        const string htmlMain = @"<!DOCTYPE html>
<html>
    <head>
        <title>Chess Boards</title>
        {0}
    </head>
    <body>
    {1}
    </body>
</html>";
        private static string htmlStyles = @"
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
