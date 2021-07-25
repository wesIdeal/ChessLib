using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Validation.Validators.MoveValidation.EnPassantRules
{
    public class EnPassantIsPossible : IMoveRule
    {
        public MoveError Validate(in Board boardInfo, in ulong[][] postMoveBoard, in IMove move)
        {
            return (boardInfo.EnPassantIndex != move.DestinationIndex)
                ? MoveError.EpNotAvailable
                : MoveError.NoneSet;
        }
    }
}
