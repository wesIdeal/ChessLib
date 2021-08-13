using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Validation.Validators.MoveValidation.MoveRules
{
    public class KingNotInCheckAfterMove : IMoveRule
    {
        public MoveError Validate(in Board boardInfo, in ulong[][] postMoveBoard, in IMove move)
        {
            var activeKingIndex = postMoveBoard[(int) boardInfo.ActivePlayer][(int) Piece.King].GetSetBits()[0];
            var isKingSquareAttacked =
                Bitboard.Instance.IsSquareAttackedByColor(activeKingIndex, boardInfo.ActivePlayer.Toggle(),
                    postMoveBoard);
            return isKingSquareAttacked ? MoveError.MoveLeavesKingInCheck : MoveError.NoneSet;
        }
    }
}