using ChessLib.Data;
using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Types.Interfaces;

namespace ChessLib.Validators.MoveValidation.MoveRules
{
    public class PieceMovingIsActiveColor : IMoveRule
    {
        public MoveExceptionType? Validate(in IBoard boardInfo, in ulong[][] postMoveBoard, in IMoveExt move)
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