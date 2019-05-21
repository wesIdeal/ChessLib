using ChessLib.Data;
using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types;

namespace ChessLib.Validators.MoveValidation.EnPassantRules
{
    public class SourceIsPawn : IMoveRule
    {
        public MoveExceptionType? Validate(in IBoard boardInfo, in ulong[][] postMoveBoard, in MoveExt move)
        {
            var isPawn = (boardInfo.PiecePlacement.Occupancy(boardInfo.ActivePlayer, Piece.Pawn) & move.SourceValue) != 0;
            if (isPawn) return null;
            return MoveExceptionType.EP_SourceIsNotPawn;
        }
    }
}