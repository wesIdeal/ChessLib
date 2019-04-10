using ChessLib.Data.Exceptions;
using ChessLib.Data.MoveRepresentation;
using MagicBitboard;

namespace ChessLib.MagicBitboard.MoveValidation.MoveRules
{
    public class KingNotInCheckAfterMove : IMoveRule
    {
        public MoveExceptionType? Validate(in BoardInfo boardInfo, in ulong[][] postMoveBoard, in MoveExt move)
        {
            if (Bitboard.IsAttackedBy(boardInfo.OpponentColor, boardInfo.ActivePlayerKingIndex, postMoveBoard))
            {
                return MoveExceptionType.MoveLeavesKingInCheck;
            }
            return null;
        }
    }
}
