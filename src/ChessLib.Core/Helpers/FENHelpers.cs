using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using ChessLib.Core.Types.Enums;
using EnumsNET;

[assembly: InternalsVisibleTo("ChessLib.Core.Tests")]

namespace ChessLib.Core.Helpers
{
    public static class FENHelpers
    {
        internal static readonly char[] ValidFENChars =
            {'/', 'p', 'P', 'n', 'N', 'b', 'B', 'r', 'R', 'q', 'Q', 'k', 'K', '1', '2', '3', '4', '5', '6', '7', '8'};


        internal static string FENFromBoard(this Board board)
        {
            return
                $"{board.Occupancy.GetPiecePlacement()} {board.ActivePlayer.GetFENSideToMoveStrRepresentation()} {MakeCastlingAvailabilityStringFromBitFlags(board.CastlingAvailability)} {GetFENEnPassantString(board.EnPassantIndex)} {board.HalfMoveClock} {board.FullMoveCounter}";
        }

        internal static string GetFENSideToMoveStrRepresentation(this Color sideToMove)
        {
            return sideToMove == Color.Black ? "b" : "w";
        }

        internal static string GetFENEnPassantString(this ushort? enPassantValue)
        {
            return enPassantValue == null ? "-" : enPassantValue.Value.IndexToSquareDisplay();
        }


        /// <summary>
        ///     Gets all ranks from premoveFen, in PremoveFEN-rank order
        /// </summary>
        /// <param name="fen">PremoveFEN String</param>
        /// <returns>An array of ranks, where [0] corresponds with rank 8</returns>
        public static string[] GetRanksFromFen(this string fen)
        {
            var pp = GetFENPiece(fen, FENPieces.PiecePlacement);
            var ranks = pp.Split('/');
            return ranks;
        }


        /// <summary>
        ///     Converts a board Index (0 = 1st Rank) to the PremoveFEN Index (7 = 1st Rank)
        /// </summary>
        /// <param name="idx">The square index</param>
        /// <returns>the corresponding board index</returns>
        internal static int BoardIndexToFENIndex(ushort idx)
        {
            var rankOffset = ((ushort) (idx / 8)).RankCompliment();
            return rankOffset * 8 + idx % 8;
        }


        /// <summary>
        ///     Gets one of the six pieces from a PremoveFEN string
        /// </summary>
        /// <param name="fen">PremoveFEN string</param>
        /// <param name="piece">the PremoveFEN piece to return</param>
        /// <returns>The string of the specified piece</returns>
        internal static string GetFENPiece(this string fen, FENPieces piece)
        {
            var fenPieces = fen.Split(' ');
            return fenPieces[(int) piece];
        }


        internal static string GetPiecePlacement(this ulong[][] piecesOnBoard)
        {
            var pieceSection = new char[64];
            for (var iColor = 0; iColor < 2; iColor++)
            for (var iPiece = 0; iPiece < 6; iPiece++)
            {
                var pieceArray = piecesOnBoard[iColor][iPiece];
                var charRepForPieceOfColor = PieceHelpers.GetFENCharPieceRepresentation((Color) iColor, (Piece) iPiece);
                while (pieceArray != 0)
                {
                    var squareIndex = BitHelpers.BitScanForward(pieceArray);
                    var fenIndex = BoardIndexToFENIndex(squareIndex);
                    pieceSection[fenIndex] = charRepForPieceOfColor;
                    pieceArray &= pieceArray - 1;
                }
            }

            var sb = new StringBuilder();
            for (var rank = 0; rank < 8; rank++) //start at PremoveFEN Rank of zero -> 7
            {
                var emptyCount = 0;
                for (var file = 0; file < 8; file++)
                {
                    var paChar = pieceSection[rank * 8 + file];
                    if (paChar == 0)
                    {
                        emptyCount++;
                    }
                    else
                    {
                        if (emptyCount != 0)
                        {
                            sb.Append(emptyCount.ToString());
                            emptyCount = 0;
                        }

                        sb.Append(paChar);
                    }
                }

                if (emptyCount != 0) sb.Append(emptyCount);
                if (rank != 7) sb.Append('/');
            }

            return sb.ToString();
        }


        /// <summary>
        ///     Creates a castling availability string given a set of CastlingAvailability flags
        /// </summary>
        /// <param name="caBitFlags">flags for castling availability</param>
        /// <returns>a PremoveFEN string for the castling availability piece</returns>
        internal static string MakeCastlingAvailabilityStringFromBitFlags(CastlingAvailability caBitFlags)
        {
            var s = "";
            if (caBitFlags == 0) s = CastlingAvailability.NoCastlingAvailable.AsString(EnumFormat.Description);
            else
            {
                foreach (var caFlag in caBitFlags.GetFlags().OrderBy(x => x))
                {
                    s += caFlag.AsString(EnumFormat.Description);
                }
            }

            return s;
        }


        public static string SanitizeFenString(string fen)
        {
            fen = fen.Trim();
            var regex = new Regex("[ ]{2,}", RegexOptions.None);
            fen = regex.Replace(fen, " ");
            return fen;
        }
    }
}