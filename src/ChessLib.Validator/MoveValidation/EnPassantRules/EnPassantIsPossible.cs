using ChessLib.Data;
using ChessLib.Data.Exceptions;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Types.Interfaces;
using ChessLib.Validators;
using ChessLib.Validators.MoveValidation;

namespace ChessLib.Validators.MoveValidation.EnPassantRules
{
    public class EnPassantIsPossible : IMoveRule
    {
        public MoveExceptionType? Validate(in IBoard boardInfo, in ulong[][] postMoveBoard, in IMoveExt move)
        {
            return (boardInfo.EnPassantSquare != move.DestinationIndex)
                ? MoveExceptionType.Ep_NotAvailalbe
                : (MoveExceptionType?)null;
        }
    }
}
