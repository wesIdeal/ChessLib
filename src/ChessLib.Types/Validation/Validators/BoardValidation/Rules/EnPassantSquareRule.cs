using System.Runtime.CompilerServices;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Helpers;
using ChessLib.Core.Types.Interfaces;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace ChessLib.Core.Validation.Validators.BoardValidation.Rules
{
    public class EnPassantSquareRule : IBoardRule
    {
        public BoardExceptionType Validate(in IBoard boardInfo)
        {
            return ValidateEnPassantSquare(boardInfo.Occupancy, boardInfo.EnPassantSquare, boardInfo.ActivePlayer);
        }

        internal virtual BoardExceptionType ValidateEnPassantSquare(ulong[][] occupancy, ushort? enPassantSquare, Color activeColor)
        {

            if (enPassantSquare == null) return BoardExceptionType.None;
            if (IsValidEnPassantSquare(enPassantSquare, activeColor))
            {
                return BoardExceptionType.BadEnPassant;
            }
            var isPawnPresentNorthOfEnPassantSquare = IsPawnPresentNorthOfEnPassantSquare(occupancy, enPassantSquare, activeColor);

            return isPawnPresentNorthOfEnPassantSquare ? BoardExceptionType.None : BoardExceptionType.BadEnPassant;
        }

        private static bool IsPawnPresentNorthOfEnPassantSquare(ulong[][] occupancy, ushort? enPassantSquare, Color activeColor)
        {
            var possiblePawnLocation =
                activeColor == Color.White ? enPassantSquare - 8 : enPassantSquare + 8;
            var possiblePawnLocationValue = 1ul << possiblePawnLocation;
            var pawnsOnBoard = occupancy.Occupancy(activeColor.Toggle(), Piece.Pawn);
            var isPawnPresentNorthOfEnPassantSquare = (pawnsOnBoard & possiblePawnLocationValue) != 0;
            return isPawnPresentNorthOfEnPassantSquare;
        }

        private static bool IsValidEnPassantSquare(ushort? enPassantSquare, Color activeColor)
        {
            if (activeColor == Color.White)
            {
                return enPassantSquare < 40 || enPassantSquare > 47;
            }
            return enPassantSquare < 16 || enPassantSquare > 23;
        }
    }
}

