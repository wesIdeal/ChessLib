using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Types.Helpers;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Validation.Validators.MoveValidation.MoveRules
{
    public class KingNotInCheckAfterMove : IMoveRule
    {
        public MoveError Validate(in IBoard boardInfo, in ulong[][] postMoveBoard, in IMove move)
        {
            var activeKingIndex = postMoveBoard[(int)boardInfo.ActivePlayer][(int)Piece.King].GetSetBits()[0];
            if (activeKingIndex.IsSquareAttackedByColor(boardInfo.OpponentColor(), postMoveBoard, activeKingIndex))
            {
                return MoveError.MoveLeavesKingInCheck;
            }
            return MoveError.NoneSet;
        }
    }
}
