using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types;
using MagicBitboard;

namespace ChessLib.MagicBitboard.MoveValidation.MoveRules
{
    public class KingNotInCheckAfterMove : IMoveRule
    {
        public MoveExceptionType? Validate(in BoardInfo boardInfo, in ulong[][] postMoveBoard, in MoveExt move)
        {
            var activeKingIndex = postMoveBoard[(int) boardInfo.ActivePlayerColor][(int) Piece.King].GetSetBits()[0];
            if (Bitboard.IsAttackedBy(boardInfo.OpponentColor, activeKingIndex, postMoveBoard))
            {
                return MoveExceptionType.MoveLeavesKingInCheck;
            }
            return null;
        }
    }
}
