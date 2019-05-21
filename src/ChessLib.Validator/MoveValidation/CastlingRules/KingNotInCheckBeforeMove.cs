using ChessLib.Data;
using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;

namespace ChessLib.Validators.MoveValidation.CastlingRules
{
    public class KingNotInCheckBeforeMove : IMoveRule
    {
        public MoveExceptionType? Validate(in IBoard boardInfo, in ulong[][] postMoveBoard, in MoveExt move)
        {
            if (Bitboard.IsSquareAttackedByColor(boardInfo.ActiveKingIndex(), boardInfo.OpponentColor(), boardInfo.PiecePlacement))
            {
                return MoveExceptionType.Castle_KingInCheck;
            }
            return null;
        }
    }
}
