using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types.Exceptions;
using ChessLib.Data.Types.Interfaces;

namespace ChessLib.Data.Validators.MoveValidation.EnPassantRules
{
    public class EnPassantIsPossible : IMoveRule
    {
        public MoveError? Validate(in IBoard boardInfo, in ulong[][] postMoveBoard, in IMoveExt move)
        {
            return (boardInfo.EnPassantSquare != move.DestinationIndex)
                ? MoveError.EpNotAvailable
                : (MoveError?)null;
        }
    }
}
