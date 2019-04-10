using ChessLib.Data.Exceptions;
using ChessLib.Data.MoveRepresentation;
using MagicBitboard;

namespace ChessLib.MagicBitboard.MoveValidation.CastlingRules
{
    public class KingNotInCheckBeforeMove : IMoveRule
    {
        public MoveExceptionType? Validate(in BoardInfo boardInfo, in ulong[][] postMoveBoard, in MoveExt move)
        {
            if (Bitboard.IsAttackedBy(boardInfo.OpponentColor, boardInfo.ActivePlayerKingIndex, boardInfo.PiecesOnBoard))
            {
                return MoveExceptionType.Castle_KingInCheck;
            }
            return null;
        }
    }
}
