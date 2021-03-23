using ChessLib.Data.MoveRepresentation;
using ChessLib.Types;
using ChessLib.Types.Exceptions;
using ChessLib.Types.Interfaces;

namespace ChessLib.Data.Validators.MoveValidation.EnPassantRules
{
    public class EnPassantIsPossible : IMoveRule
    {
        public MoveError Validate(in IBoard boardInfo, in ulong[][] postMoveBoard, in IMoveExt move)
        {
            return (boardInfo.EnPassantSquare != move.DestinationIndex)
                ? MoveError.EpNotAvailable
                : MoveError.NoneSet;
        }
    }
}
