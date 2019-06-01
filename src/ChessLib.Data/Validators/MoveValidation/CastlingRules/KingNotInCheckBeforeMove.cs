using ChessLib.Data;
using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Types.Interfaces;

namespace ChessLib.Validators.MoveValidation.CastlingRules
{
    public class KingNotInCheckBeforeMove : IMoveRule
    {
        public MoveExceptionType? Validate(in IBoard boardInfo, in ulong[][] postMoveBoard, in IMoveExt move)
        {
            if (Bitboard.IsSquareAttackedByColor(boardInfo.ActiveKingIndex(), boardInfo.OpponentColor(), boardInfo.GetPiecePlacement()))
            {
                return MoveExceptionType.Castle_KingInCheck;
            }
            return null;
        }
    }
}
