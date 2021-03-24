using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Types.Helpers;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Validation.Validators.MoveValidation.CastlingRules
{
    public class KingNotInCheckBeforeMove : IMoveRule
    {
        public MoveError Validate(in IBoard boardInfo, in ulong[][] postMoveBoard, in IMove move)
        {
            if (boardInfo.ActiveKingIndex().IsSquareAttackedByColor(boardInfo.OpponentColor(), boardInfo.Occupancy, null))
            {
                return MoveError.CastleKingInCheck;
            }
            return MoveError.NoneSet;
        }
    }
}
