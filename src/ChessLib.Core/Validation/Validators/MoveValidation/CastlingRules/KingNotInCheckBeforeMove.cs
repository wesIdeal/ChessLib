using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Validation.Validators.MoveValidation.CastlingRules
{
    public class KingNotInCheckBeforeMove : IMoveRule
    {
        public MoveError Validate(in IBoard boardInfo, in ulong[][] postMoveBoard, in IMove move)
        {
            var isOpponentInCheck = BoardHelpers.IsColorInCheck(boardInfo.Occupancy, boardInfo.OpponentColor());
            return  isOpponentInCheck ? MoveError.CastleKingInCheck : MoveError.NoneSet;
        }
    }
}
