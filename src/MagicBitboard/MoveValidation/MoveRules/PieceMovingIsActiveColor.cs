using ChessLib.Data.Exceptions;
using ChessLib.Data.MoveRepresentation;

namespace ChessLib.MagicBitboard.MoveValidation.MoveRules
{
    public class PieceMovingIsActiveColor : IMoveRule
    {
        public MoveExceptionType? Validate(in BoardInfo boardInfo, in ulong[][] postMoveBoard, in MoveExt move)
        {
            var activePieces = boardInfo.ActiveTotalOccupancy;
            if ((activePieces & move.SourceValue) == 0)
            {
                return MoveExceptionType.ActivePlayerHasNoPieceOnSourceSquare;
            }
            return null;
        }
    }
}