using ChessLib.Core.Helpers;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Validation.Validators.MoveValidation.CastlingRules
{
    public class KingNotInCheckBeforeMove : IMoveRule
    {
        public MoveError Validate(in Board boardInfo, in ulong[][] postMoveBoard, in IMove move)
        {
            var isOpponentInCheck = BoardHelpers.IsColorInCheck(boardInfo.Occupancy, boardInfo.ActivePlayer);
            return isOpponentInCheck ? MoveError.CastleKingInCheck : MoveError.NoneSet;
        }
    }
}