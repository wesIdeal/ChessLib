using ChessLib.Data;
using ChessLib.Data.Exceptions;
using ChessLib.Data.MoveRepresentation;

namespace ChessLib.Validators.MoveValidation.CastlingRules
{
    public class KingNotInCheckBeforeMove : IMoveRule
    {
        public MoveExceptionType? Validate(in BoardFENInfo boardInfo, in ulong[][] postMoveBoard, in MoveExt move)
        {
            if (Bitboard.IsAttackedBy(boardInfo.ActivePlayerKingIndex, boardInfo.OpponentColor, boardInfo.PiecePlacement))
            {
                return MoveExceptionType.Castle_KingInCheck;
            }
            return null;
        }
    }
}
