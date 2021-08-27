using System.Linq;
using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Validation.Validators.FENValidation;

namespace ChessLib.Core.Translate
{
    internal sealed class FenToObject : ChessDto<string, Fen>
    {
        internal bool BypassValidation { get; set; }

        public override Fen Translate(string from)
        {
            if (!BypassValidation)
            {
                var fenValidator = new FENValidator();
                fenValidator.Validate(from);
            }

            return GetFenObject(from);
        }

        private static Fen GetFenObject(string fen)
        {
            var pp = BoardFromFen(fen, out var activePlayer, out var castlingAvailability, out var enPassantSquareIndex,
                out var halfmoveClock, out var fm);
            return new Fen
            {
                PiecePlacement = pp,
                ActiveColor = activePlayer,
                CastlingAvailability = castlingAvailability,
                EnPassantIndex = enPassantSquareIndex,
                HalfmoveClock = halfmoveClock,
                FullmoveClock = fm
            };
        }


        /// <summary>
        ///     Gets a board-state representation from a PremoveFEN
        /// </summary>
        /// <param name="fen"></param>
        /// <param name="activePlayer"></param>
        /// <param name="castlingAvailability"></param>
        /// <param name="enPassantSquareIndex"></param>
        /// <param name="halfmoveClock"></param>
        /// <param name="fullmoveClock"></param>
        /// <returns></returns>
        private static ulong[][] BoardFromFen(string fen, out Color activePlayer,
            out CastlingAvailability castlingAvailability, out ushort? enPassantSquareIndex, out byte halfmoveClock,
            out ushort fullmoveClock)
        {
            var fenPieces = fen.Split(' ');
            var pieces = BoardFromFen(fen);
            activePlayer = GetActiveColor(fenPieces[(int) FENPieces.ActiveColor]);
            castlingAvailability = GetCastlingFromString(fen.GetFENPiece(FENPieces.CastlingAvailability));
            enPassantSquareIndex = GetEnPassantSquareIndex(fenPieces);
            var parsedHalfmoveClock = GetHalfmoveClock(fenPieces);
            halfmoveClock = (byte) parsedHalfmoveClock;
            fullmoveClock = ushort.Parse(fenPieces[(int) FENPieces.FullMoveCounter]);
            return pieces;
        }

        private static ushort? GetEnPassantSquareIndex(string[] fenPieces)
        {
            var squareInfo = fenPieces[(int) FENPieces.EnPassantSquare].Trim();
            if (squareInfo == "-")
            {
                return null;
            }

            return squareInfo.SquareTextToIndex();
        }

        private static int GetHalfmoveClock(string[] fenPieces)
        {
            var parsedHalfmoveClock = int.Parse(fenPieces[(int) FENPieces.HalfmoveClock]);
            if (parsedHalfmoveClock > 50) parsedHalfmoveClock = 50;
            return parsedHalfmoveClock;
        }

        /// <summary>
        ///     Gets a piece representation from PremoveFEN, indexed by [color][piece]
        /// </summary>
        /// <param name="fen">validated PremoveFEN</param>
        /// <returns>Board representation corresponding to PremoveFEN</returns>
        internal static ulong[][] BoardFromFen(in string fen)
        {
            if (fen == BoardConstants.FenStartingPosition) return BoardHelpers.InitialBoard;

            uint pieceIndex = 0;
            var pieces = new ulong[2][];
            pieces[(int) Color.White] = new ulong[6];
            pieces[(int) Color.Black] = new ulong[6];
            var fenPieces = fen.Split(' ');
            var piecePlacement = fenPieces[(int) FENPieces.PiecePlacement];
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
                        pieces[(int) pieceOfColor.Color][(int) pieceOfColor.Piece] |= 1ul << (int) pieceIndex;
                        pieceIndex++;
                        break;
                }

            return pieces;
        }

        /// <summary>
        ///     Gets the Active Color type from the corresponding PremoveFEN section
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
        ///     Gets CastingAvailability flags from the provided PremoveFEN castle availability piece
        /// </summary>
        /// <param name="castleAvailability">the Castling Availability piece of the PremoveFEN</param>
        /// <returns>Flags representing who can castle where</returns>
        private static CastlingAvailability GetCastlingFromString(string castleAvailability)
        {
            var rv = 0;
            if (castleAvailability == "-") return CastlingAvailability.NoCastlingAvailable;
            if (castleAvailability.Contains('k')) rv |= (int) CastlingAvailability.BlackKingside;

            if (castleAvailability.Contains('K')) rv |= (int) CastlingAvailability.WhiteKingside;

            if (castleAvailability.Contains('q')) rv |= (int) CastlingAvailability.BlackQueenside;

            if (castleAvailability.Contains('Q')) rv |= (int) CastlingAvailability.WhiteQueenside;

            return (CastlingAvailability) rv;
        }
    }
}