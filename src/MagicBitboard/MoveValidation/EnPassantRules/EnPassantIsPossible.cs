using ChessLib.Data.Exceptions;
using ChessLib.Data.MoveRepresentation;

namespace ChessLib.MagicBitboard.MoveValidation.EnPassantRules
{
    public class EnPassantIsPossible : IMoveRule
    {
        public MoveExceptionType? Validate(in BoardInfo boardInfo, in ulong[][] postMoveBoard, in MoveExt move)
        {
            return (boardInfo.EnPassantIndex != move.DestinationIndex)
                ? MoveExceptionType.Ep_NotAvailalbe
                : (MoveExceptionType?) null;
        }
    }
}
