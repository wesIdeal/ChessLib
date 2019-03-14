using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using MagicBitboard.Enums;
namespace MagicBitboard.Helpers
{
    public static class BoardHelpers
    {
        public readonly static Dictionary<Color, Dictionary<Piece, string>> HtmlPieceRepresentations;
        public static Color Toggle(this Color c) => c == Color.White ? Color.Black : Color.White;
        public static ulong[] FileMasks = new ulong[8];
        public static ulong[] RankMasks = new ulong[8];
        public static ulong[,] IndividualSquares = new ulong[8, 8];
        static BoardHelpers()
        {
            HtmlPieceRepresentations = new Dictionary<Color, Dictionary<Piece, string>>();
            InitializeFileMasks();
            InitializRankMasks();
            InitializeIndividualSquares();
            InitializeHtmlPieceRepresentations();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char IndexToFileDisplay(this int i) => (char)('a' + (i % 8));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char IndexToRankDisplay(this int i) => (char)('1' + (i / 8));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string IndexToSquareDisplay(this int i) => $"{i.IndexToFileDisplay()}{i.IndexToRankDisplay()}";


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
            if (!char.IsLetter(file) || (file < 'a' || file > 'h'))
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
        #region Initialization
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetBitArrayIndex(Rank rank, File file)
        {
            var r = rank.ToInt();
            var f = file.ToInt();
            return (r * 8) + f;
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
        #endregion

        #region Shift Helpers
        public static ulong Not(this ulong u) => ~u;



        #endregion

        public static readonly ulong[,] IndividialSquares = new ulong[8, 8];

        public static void InitializeIndividualSquares()
        {
            for (int i = 0; i < 64; i++)
            {
                IndividialSquares[(i / 8), i % 8] = (ulong)1 << i;
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

        #region Board Print Representation
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

        public static string MakeBoardTable(this ulong[] uArr, int pieceIndex, string header = "", string pieceRep = "^", string attackSquareRep = "*")
        {
            StringBuilder sb = new StringBuilder();
            uArr.ToList().ForEach(x => sb.AppendLine(MakeBoardTable(x, pieceIndex, header, pieceRep, attackSquareRep)));
            return PrintBoardHtml(sb.ToString());
        }

        public static string MakeBoardTable(this ulong u, int pieceIndex, string header = "", string pieceRep = "^", string attackSquareRep = "*")
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

        #endregion

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
