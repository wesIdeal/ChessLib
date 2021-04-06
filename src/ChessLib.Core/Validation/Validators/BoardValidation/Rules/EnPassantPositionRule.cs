using System.Runtime.CompilerServices;
using ChessLib.Core.Helpers;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Interfaces;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace ChessLib.Core.Validation.Validators.BoardValidation.Rules
{
    public interface IEnPassantSquareRule : IBoardRule
    {
        BoardExceptionType Validate(IBoardState boardState);
    }

    /// <summary>
    ///     Validates that board parameters are correct in regards to en passant square
    /// </summary>
    public class EnPassantPositionRule : IBoardRule
    {
        public BoardExceptionType Validate(in IBoard boardInfo)
        {
            return ValidateEnPassantSquare(boardInfo.Occupancy, boardInfo.EnPassantIndex, boardInfo.ActivePlayer);
        }


        protected virtual BoardExceptionType ValidateEnPassantSquare(ulong[][] occupancy, ushort? enPassantSquare,
            Color activeColor)
        {
            if (enPassantSquare == null)
            {
                return BoardExceptionType.None;
            }

        

            if (occupancy != null && occupancy.Occupancy() != 0)    
            {
                var isPawnPresentNorthOfEnPassantSquare =
                    IsPawnPresentNorthOfEnPassantSquare(occupancy, enPassantSquare, activeColor);
                if (!isPawnPresentNorthOfEnPassantSquare)
                {
                    return BoardExceptionType.BadEnPassant;
                }
            }

            return BoardExceptionType.None;
        }
        
        private static bool IsPawnPresentNorthOfEnPassantSquare(ulong[][] occupancy, ushort? enPassantSquare,
            Color activeColor)
        {
            var possiblePawnLocation =
                activeColor == Color.White ? enPassantSquare - 8 : enPassantSquare + 8;
            var possiblePawnLocationValue = 1ul << possiblePawnLocation;
            var pawnsOnBoard = occupancy.Occupancy(activeColor.Toggle(), Piece.Pawn);
            var isPawnPresentNorthOfEnPassantSquare = (pawnsOnBoard & possiblePawnLocationValue) != 0;
            return isPawnPresentNorthOfEnPassantSquare;
        }
    }
}