using ChessLib.Data.Exceptions;
using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;
using EnumsNET;
using System;
using System.Linq;
using ChessLib.Data.Boards;
using ChessLib.Validators.BoardValidators.Rules;
using ChessLib.Validators.FENValidation;

namespace ChessLib.Data.Helpers
{
    public static class FENHelpers
    {
        /// <summary>
        /// Initial FEN - starting position of a chess game
        /// </summary>
        public const string FENInitial = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        public static readonly char[] ValidCastlingStringChars = new char[] { 'k', 'K', 'q', 'Q', '-' };
        public static readonly char[] ValidFENChars = new char[] { '/', 'p', 'P', 'n', 'N', 'b', 'B', 'r', 'R', 'q', 'Q', 'k', 'K', '1', '2', '3', '4', '5', '6', '7', '8' };

        /// <summary>
        /// Gets a rank from a validated fen string
        /// </summary>
        /// <param name="fen">FEN string</param>
        /// <param name="rank">Board Rank (*not bitboard index rank*)</param>
        /// <returns></returns>
        public static string GetRankFromFen(this string fen, int rank)
        {
            var r = Math.Abs(rank - 8);
            var ranks = GetRanksFromFen(fen);
            return ranks[rank];
        }

        /// <summary>
        /// Gets all ranks from fen, in FEN-rank order
        /// </summary>
        /// <param name="fen">FEN String</param>
        /// <returns>An array of ranks, where [0] corresponds with rank 8</returns>
        public static string[] GetRanksFromFen(this string fen)
        {
            var pp = GetFENPiece(fen, FENPieces.PiecePlacement);
            var ranks = pp.Split('/');
            return ranks;
        }

        /// <summary>
        /// Gets one of the six pieces from a FEN string
        /// </summary>
        /// <param name="fen">FEN string</param>
        /// <param name="piece">the FEN piece to return</param>
        /// <returns>The string of the specified piece</returns>
        public static string GetFENPiece(this string fen, FENPieces piece)
        {
            var fenPieces = fen.Split(' ');
            return fenPieces[(int)piece];
        }

        /// <summary>
        /// Gets CastingAvailability flags from the provided FEN castle availability piece
        /// </summary>
        /// <param name="castleAvailability">the Castling Availability piece of the FEN</param>
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
        /// Gets the Active Color type from the corresponding FEN section
        /// </summary>
        /// <param name="v">The Active Color piece of the FEN</param>
        /// <returns>A Color object representing the actice Color</returns>
        public static Color GetActiveColor(string v)
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
        /// <returns>a FEN string for the castling availability piece</returns>
        public static string MakeCastlingAvailabilityStringFromBitFlags(CastlingAvailability caBitFlags)
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
        /// Converts a board Index (0 = 1st Rank) to the FEN Index (7 = 1st Rank)
        /// </summary>
        /// <param name="idx">The square index</param>
        /// <returns>the corresponding board index</returns>
        public static int BoardIndexToFENIndex(ushort idx)
        {
            var rankOffset = ((ushort)(idx / 8)).RankCompliment();
            return (rankOffset * 8) + (idx % 8);
        }

        /// <summary>
        /// Gets a piece representation from FEN, indexed by [color][piece]
        /// </summary>
        /// <param name="fen">validated FEN</param>
        /// <returns>Board representation corresponding to FEN</returns>
        public static ulong[][] BoardFromFen(in string fen)
        {
            uint pieceIndex = 0;
            var pieces = new ulong[2][];
            pieces[(int)Color.White] = new ulong[6];
            pieces[(int)Color.Black] = new ulong[6];
            var fenPieces = fen.Split(' ');
            var piecePlacement = fenPieces[(int)FENPieces.PiecePlacement];
            var ranks = piecePlacement.Split('/').Reverse();
            foreach (var rank in ranks)
                foreach (var f in rank)
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
            return pieces;
        }

        /// <summary>
        /// Gets a board-state representation from a FEN
        /// </summary>
        /// <param name="fen"></param>
        /// <param name="activePlayer"></param>
        /// <param name="castlingAvailability"></param>
        /// <param name="enPassantSquareIndex"></param>
        /// <param name="halfmoveClock"></param>
        /// <param name="fullmoveClock"></param>
        /// <param name="validate"></param>
        /// <returns></returns>
        public static ulong[][] BoardFromFen(this string fen, out Color activePlayer, out CastlingAvailability castlingAvailability, out ushort? enPassantSquareIndex, out uint halfmoveClock, out uint fullmoveClock, bool validate = true)
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
            halfmoveClock = uint.Parse(fenPieces[(int)FENPieces.HalfmoveClock]);
            fullmoveClock = uint.Parse(fenPieces[(int)FENPieces.FullMoveCounter]);
            return pieces;
        }
    }
}
