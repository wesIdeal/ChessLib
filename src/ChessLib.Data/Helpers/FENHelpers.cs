using EnumsNET;
using System;
using System.Linq;
using System.Text;
using ChessLib.Data.Types.Enums;
using ChessLib.Data.Types.Exceptions;
using ChessLib.Data.Validators.FENValidation;

namespace ChessLib.Data.Helpers
{
    public static class FENHelpers
    {
        /// <summary>
        /// Initial PremoveFEN - starting position of a chess game
        /// </summary>
        public const string FENInitial = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        internal static readonly char[] ValidCastlingStringChars = { 'k', 'K', 'q', 'Q', '-' };
        internal static readonly char[] ValidFENChars = { '/', 'p', 'P', 'n', 'N', 'b', 'B', 'r', 'R', 'q', 'Q', 'k', 'K', '1', '2', '3', '4', '5', '6', '7', '8' };

        internal static string GetPiecePlacement(this ulong[][] piecesOnBoard)
        {
            var pieceSection = new char[64];
            for (var iColor = 0; iColor < 2; iColor++)
                for (var iPiece = 0; iPiece < 6; iPiece++)
                {
                    var pieceArray = piecesOnBoard[iColor][iPiece];
                    var charRepForPieceOfColor = PieceHelpers.GetFENCharPieceRepresentation((Color)iColor, (Piece)iPiece);
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
                    var paChar = pieceSection[(rank * 8) + file];
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
        /// Gets a rank from a validated premoveFen string
        /// </summary>
        /// <param name="fen">PremoveFEN string</param>
        /// <param name="rank">Board Rank (*not bitboard index rank*)</param>
        /// <returns></returns>
        internal static string GetRankFromFen(this string fen, int rank)
        {
            var r = Math.Abs(rank - 8);
            var ranks = GetRanksFromFen(fen);
            return ranks[rank];
        }

        /// <summary>
        /// Gets all ranks from premoveFen, in PremoveFEN-rank order
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
        /// Gets one of the six pieces from a PremoveFEN string
        /// </summary>
        /// <param name="fen">PremoveFEN string</param>
        /// <param name="piece">the PremoveFEN piece to return</param>
        /// <returns>The string of the specified piece</returns>
        internal static string GetFENPiece(this string fen, FENPieces piece)
        {
            var fenPieces = fen.Split(' ');
            return fenPieces[(int)piece];
        }

        /// <summary>
        /// Gets CastingAvailability flags from the provided PremoveFEN castle availability piece
        /// </summary>
        /// <param name="castleAvailability">the Castling Availability piece of the PremoveFEN</param>
        /// <returns>Flags representing who can castle where</returns>
        private static CastlingAvailability GetCastlingFromString(string castleAvailability)
        {
            var rv = 0;
            if (castleAvailability == "-") return CastlingAvailability.NoCastlingAvailable;
            if (castleAvailability.Contains('k')) { rv |= (int)CastlingAvailability.BlackKingside; }
            if (castleAvailability.Contains('K')) { rv |= (int)CastlingAvailability.WhiteKingside; }
            if (castleAvailability.Contains('q')) { rv |= (int)CastlingAvailability.BlackQueenside; }
            if (castleAvailability.Contains('Q')) { rv |= (int)CastlingAvailability.WhiteQueenside; }
            return (CastlingAvailability)rv;
        }

        /// <summary>
        /// Gets the Active Color type from the corresponding PremoveFEN section
        /// </summary>
        /// <param name="v">The Active Color piece of the PremoveFEN</param>
        /// <returns>A Color object representing the active Color</returns>
        internal static Color GetActiveColor(string v)
        {
            switch (v)
            {
                case "w": return Color.White;
                case "b": return Color.Black;
                default: throw new FENException(v, FENError.InvalidActiveColor);
            }
        }


        /// <summary>
        /// Creates a castling availability string given a set of CastlingAvailability flags
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

        /// <summary>
        /// Converts a board Index (0 = 1st Rank) to the PremoveFEN Index (7 = 1st Rank)
        /// </summary>
        /// <param name="idx">The square index</param>
        /// <returns>the corresponding board index</returns>
        internal static int BoardIndexToFENIndex(ushort idx)
        {
            var rankOffset = ((ushort)(idx / 8)).RankCompliment();
            return (rankOffset * 8) + (idx % 8);
        }

        /// <summary>
        /// Gets a piece representation from PremoveFEN, indexed by [color][piece]
        /// </summary>
        /// <param name="fen">validated PremoveFEN</param>
        /// <returns>Board representation corresponding to PremoveFEN</returns>
        internal static ulong[][] BoardFromFen(in string fen)
        {
            if (fen == FENInitial)
            {
                return BoardHelpers.InitialBoard;
            }
            uint pieceIndex = 0;
            var pieces = new ulong[2][];
            pieces[(int)Color.White] = new ulong[6];
            pieces[(int)Color.Black] = new ulong[6];
            var fenPieces = fen.Split(' ');
            var piecePlacement = fenPieces[(int)FENPieces.PiecePlacement];
            var ranks = piecePlacement.Split('/').Reverse();
            foreach (var rank in ranks)
            {
                foreach (var f in rank)
                {
                    switch (char.IsDigit(f))
                    {
                        case true:
                            var emptySquares = uint.Parse(f.ToString());
                            pieceIndex += emptySquares;
                            break;
                        case false:
                            var pieceOfColor = PieceHelpers.GetPieceOfColor(f);
                            pieces[(int)pieceOfColor.Color][(int)pieceOfColor.Piece] |= 1ul << (int)pieceIndex;
                            pieceIndex++;
                            break;
                    }
                }
            }
           
            return pieces;
        }

        /// <summary>
        /// Gets a board-state representation from a PremoveFEN
        /// </summary>
        /// <param name="fen"></param>
        /// <param name="activePlayer"></param>
        /// <param name="castlingAvailability"></param>
        /// <param name="enPassantSquareIndex"></param>
        /// <param name="halfmoveClock"></param>
        /// <param name="fullmoveClock"></param>
        /// <param name="validate"></param>
        /// <returns></returns>
        internal static ulong[][] BoardFromFen(this string fen, out Color activePlayer, out CastlingAvailability castlingAvailability, out ushort? enPassantSquareIndex, out ushort halfmoveClock, out ushort fullmoveClock, bool validate = true)
        {
            var fenPieces = fen.Split(' ');
            if (validate)
            {
                var validator = new FENValidator(fen);
                var results = validator.Validate();
                if (results != FENError.None)
                {
                    throw new FENException(fen, results);
                }
            }

            var pieces = BoardFromFen(fen);
            activePlayer = GetActiveColor(fenPieces[(int)FENPieces.ActiveColor]);
            castlingAvailability = GetCastlingFromString(fen.GetFENPiece(FENPieces.CastlingAvailability));
            enPassantSquareIndex = fenPieces[(int)FENPieces.EnPassantSquare].SquareTextToIndex();
            halfmoveClock = ushort.Parse(fenPieces[(int)FENPieces.HalfmoveClock]);
            fullmoveClock = ushort.Parse(fenPieces[(int)FENPieces.FullMoveCounter]);
            return pieces;
        }

        internal static Color GetActiveColorFromFENString(string fen)
        {
            return GetActiveColor(fen.GetFENPiece(FENPieces.ActiveColor));
        }
    }
}
