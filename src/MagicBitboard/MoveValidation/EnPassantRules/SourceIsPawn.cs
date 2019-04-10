using ChessLib.Data.Exceptions;
using ChessLib.Data.MoveRepresentation;
using MagicBitboard;

namespace ChessLib.MagicBitboard.MoveValidation.EnPassantRules
{
    public class SourceIsPawn : IMoveRule
    {
        public MoveExceptionType? Validate(in BoardInfo boardInfo, in ulong[][] postMoveBoard, in MoveExt move)
        {
            var isPawn = (boardInfo.ActivePawnOccupancy & move.SourceValue) != 0;
            if (isPawn) return null;
            return MoveExceptionType.EP_SourceIsNotPawn;
        }
    }
}