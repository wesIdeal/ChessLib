using ChessLib.Data;
using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Validators;
using ChessLib.Validators.MoveValidation;

namespace ChessLib.Validators.MoveValidation.MoveRules
{
    public class DestinationNotOccupiedByActiveColor : IMoveRule
    {
        public MoveExceptionType? Validate(in BoardFENInfo boardInfo, in ulong[][] postMoveBoard, in MoveExt move)
        {
            if ((boardInfo.PiecePlacement.Occupancy(boardInfo.ActivePlayer) & move.DestinationValue) != 0)
            {
                return MoveExceptionType.ActiveColorPieceAtDestination;
            }

            return null;
        }
    }
}
