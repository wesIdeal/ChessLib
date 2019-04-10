using ChessLib.Data.Exceptions;
using ChessLib.Data.MoveRepresentation;
using MagicBitboard;

namespace ChessLib.MagicBitboard.MoveValidation.MoveRules
{
    public class DestinationNotOccupiedByActiveColor : IMoveRule
    {
        public MoveExceptionType? Validate(in BoardInfo boardInfo, in ulong[][] postMoveBoard, in MoveExt move)
        {
            
            if ((boardInfo.ActiveTotalOccupancy & move.DestinationValue) != 0)
            {
                return MoveExceptionType.ActiveColorPieceAtDestination;
            }

            return null;
        }
    }
}
