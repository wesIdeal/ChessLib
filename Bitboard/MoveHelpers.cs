using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MagicBitboard.Enums;
namespace MagicBitboard
{
    public static class MoveHelpers
    {
        public readonly static Dictionary<Color, Dictionary<Piece, string>> HtmlPieceRepresentations;
        public static ulong[] FileMasks = new ulong[8];
        public static ulong[] RankMasks = new ulong[8];
        public static ulong[,] IndividualSquares = new ulong[8, 8];
        static MoveHelpers()
        {
            HtmlPieceRepresentations = new Dictionary<Color, Dictionary<Piece, string>>();
            InitializeFileMasks();
            InitializRankMasks();
            InitializeIndividualSquares();
            InitializeHtmlPieceRepresentations();
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
        public static int ToInt(this Color c) => (int)c;

        public static int ToInt(this Piece p) => (int)p;

        public static int ToInt(this File f) => (int)f;

        public static int ToInt(this Rank r) => (int)r;
        #endregion

        #region Array Position to Friendly Position Helpers

        public static int GetBitArrayIndex(Rank rank, File file)
        {
            var r = rank.ToInt();
            var f = file.ToInt();
            return (r * 8) + f;
        }

        public static File GetFile(this int square)
        {
            return (File)(square % 8);
        }

        public static Rank GetRank(this int square)
        {
            var r = square / 8;
            return (Rank)(square / 8);
        }
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

        public static string DisplayBits(this ulong u)
        {
            var str = Convert.ToString((long)u, 2).PadLeft(64, '0');
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 8; i++)
            {
                sb.AppendLine(new string(str.Skip(i * 8).Take(8).Reverse().ToArray()));
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

        public static string MakeBoardTable(this ulong u, Rank pieceRank, File pieceFile, string header = "", string pieceRep = "*", string attackSquareRep = "^")
        {
            string boardBits = Convert.ToString((long)u, 2).PadLeft(64, '0');
            var board = new List<string>();

            var sb = new StringBuilder("<table class=\"chessboard\">\r\n");
            if (header != string.Empty)
            {
                sb.AppendLine($"<caption>{header}<br/>{board}</caption>");
            }
            const string squareFormat = "<td id=\"{1}{0}\" class=\"{3}\">{2}</td>";

            var array = new List<string>();
            var replacementRank = pieceRank.ToInt();
            var replacementFile = pieceFile.ToInt();
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
            //for (Rank r = Enums.Rank.R8; r >= Enums.Rank.R1; r--)
            //{
            //    var rank = Math.Abs(r.ToInt() - 7);
            //    sb.AppendLine($"<tr id=\"rank{rank}\">");

            //    for (File f = File.A; f <= File.H; f++)
            //    {
            //        var file = f.ToInt();
            //        var temp = board[(rank * 8) + file];
            //        var pieceAtSquare = (f == pieceFile && r == pieceRank) ? pieceRep.ToString() : board[(rank * 8) + file] == '1' ? attackSquareRep.ToString() : "&nbsp;";
            //        sb.AppendFormat(squareFormat, f.ToString(), rank, pieceAtSquare, board[(rank * 8) + file] == '1' ? "altColor" : "");
            //    }
            //    
            //}
            sb.AppendLine("</table>");
            return sb.ToString();
        }

        public static string MakeBoardTable(this BoardRepresentation rep)
        {
            var sb = new StringBuilder("<table class=\"chessboard\">\r\n");
            const string squareFormat = "<td >{0}</td>";
            var charArrayRep = rep.GetCharacterArrayRepresntation();
            var rankHtml = new char?[8];
            var rankIdx = 0;
            while ((rankHtml = charArrayRep.Skip(rankIdx * 8).Take(8).ToArray()).Any())
            {
                sb.Append("<tr>");
                rankHtml = rankHtml.Reverse().ToArray();
                var rankString = string.Join("\r\n", rankHtml.Select(x => string.Format(squareFormat, PieceOfColor.GetHtmlRepresentation(x))));
                sb.AppendLine(rankString);
                sb.Append("</tr>");
                rankIdx++;
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
            //if (highlightRank.HasValue && highlightFile.HasValue)
            //{
            //    var r = highlightFile.Value.ToInt();
            //    r = 8 - r;
            //    var f = highlightFile.Value.ToInt();
            //    var position = (r * 8) + f;
            //    if (position == 0)
            //    {
            //        str = "*" + str.Substring(1);
            //    }
            //    else { str = str.Substring(0, position - 1) + "*" + str.Substring(position); }
            //}
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

        public static ulong[][] GenerateSlidingPieceOccupancyBoards(out ulong[,] rookAttackMask, out ulong[,] bishopAttackMask)
        {
            int i;
            int bitRef;
            ulong mask;
            var occupancyMask = new ulong[2][] { new ulong[64], new ulong[64] };
            var occupancyMaskRook = occupancyMask[0];
            var occupancyMaskBishop = occupancyMask[1];
            rookAttackMask = new ulong[8, 8];
            bishopAttackMask = new ulong[8, 8];
            for (bitRef = 0; bitRef < 64; bitRef++)
            {
                mask = 0;
                var rank = GetRank(bitRef);
                var file = GetFile(bitRef);
                //NORTH
                for (i = bitRef + 8; i < 56; i += 8) { mask |= ((ulong)1) << i; }
                //SOUTH
                for (i = bitRef - 8; i >= 8; i -= 8) mask |= ((ulong)1) << i;
                //EAST
                for (i = bitRef + 1; i < 64 && i % 8 != 7; i++) mask |= ((ulong)1) << i;
                //WEST
                for (i = bitRef - 1; i > 0 && i % 8 != 0; i--) mask |= ((ulong)1) << i;
                rookAttackMask[rank.ToInt(), file.ToInt()] = mask;

                mask = 0;
                //NE
                for (i = bitRef + 9; i % 8 != 7 && i % 8 != 0 && i <= 55; i += 9) mask |= ((ulong)1) << i;
                //SW
                for (i = bitRef - 9; i % 8 != 7 && i % 8 != 0 && i >= 8; i -= 9) mask |= ((ulong)1) << i;
                //NW
                for (i = bitRef + 7; i % 8 != 7 && i % 8 != 0 && i <= 55; i += 7) mask |= ((ulong)1) << i;
                //SE
                for (i = bitRef - 7; i % 8 != 7 && i % 8 != 0 && i >= 8; i -= 7) mask |= ((ulong)1) << i;
                bishopAttackMask[rank.ToInt(), file.ToInt()] = mask;
            }
            return occupancyMask;
        }
    }
}
