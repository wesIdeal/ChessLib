using System.Runtime.CompilerServices;
using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Interfaces;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace ChessLib.Core.Validation.Validators.BoardValidation.Rules
{
    public interface IEnPassantSquareRule
    {
        BoardExceptionType ValidateEnPassantSquare(ulong[][] occupancy, ushort? enPassantSquare, Color activeColor);
        bool IsValidEnPassantSquare(ushort? enPassantSquare, Color activeColor);
    }

    /// <summary>
    /// Validates that board parameters are correct in regards to en passant square
    /// </summary>
    public class EnPassantSquareRule : IBoardRule, IEnPassantSquareRule
    {
        public BoardExceptionType Validate(in IBoard boardInfo)
        {
            return ValidateEnPassantSquare(boardInfo.Occupancy, boardInfo.EnPassantIndex, boardInfo.ActivePlayer);
        }

        public virtual BoardExceptionType ValidateEnPassantSquare(ulong[][] occupancy, ushort? enPassantSquare, Color activeColor)
        {
            if (enPassantSquare == null)
            {
                return BoardExceptionType.None;
            }

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

        public bool IsValidEnPassantSquare(ushort? enPassantSquare, Color activeColor)
        {
            if (enPassantSquare == null)
            {
                return true;
            }
            var enPassantRange = activeColor == Color.Black ? BoardConstants.Rank6 : BoardConstants.Rank3;


            var boardValueOfIndex = enPassantSquare.Value.GetBoardValueOfIndex();
            return (enPassantRange & boardValueOfIndex) == boardValueOfIndex;
        }
    }
}

