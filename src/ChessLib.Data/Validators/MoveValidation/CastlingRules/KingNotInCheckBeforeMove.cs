using ChessLib.Data.Helpers;
using ChessLib.Data.Magic;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types.Exceptions;
using ChessLib.Data.Types.Interfaces;

namespace ChessLib.Data.Validators.MoveValidation.CastlingRules
{
    public class KingNotInCheckBeforeMove : IMoveRule
    {
        public MoveError Validate(in IBoard boardInfo, in ulong[][] postMoveBoard, in IMoveExt move)
        {
            if (boardInfo.ActiveKingIndex().IsSquareAttackedByColor(boardInfo.OpponentColor(), boardInfo.Occupancy, null))
            {
                return MoveError.CastleKingInCheck;
            }
            return MoveError.NoneSet;
        }
    }
}
