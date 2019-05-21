using ChessLib.Data;
using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;

namespace ChessLib.Validators.MoveValidation.MoveRules
{
    public class PieceMovingIsActiveColor : IMoveRule
    {
        public MoveExceptionType? Validate(in IBoard boardInfo, in ulong[][] postMoveBoard, in MoveExt move)
        {
            var activePieces = boardInfo.ActiveOccupancy();
            if ((activePieces & move.SourceValue) == 0)
            {
                return MoveExceptionType.ActivePlayerHasNoPieceOnSourceSquare;
            }
            return null;
        }
    }
}