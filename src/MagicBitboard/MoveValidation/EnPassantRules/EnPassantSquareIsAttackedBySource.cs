using ChessLib.Data;
using ChessLib.Data.Exceptions;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types;

namespace ChessLib.MagicBitboard.MoveValidation.EnPassantRules
{
    public class EnPassantSquareIsAttackedBySource : IMoveRule
    {
        public MoveExceptionType? Validate(in BoardInfo boardInfo, in ulong[][] postMoveBoard, in MoveExt move)
        {
            var pawnAttacksFromSquare = Bitboard.GetAttackedSquares(Piece.Pawn, move.SourceIndex,
                boardInfo.TotalOccupancy, boardInfo.ActivePlayerColor);
            var isAttacked = (pawnAttacksFromSquare & move.DestinationValue) != 0;
            if (isAttacked) return null;
            return MoveExceptionType.EP_NotAttackedBySource;
        }
    }
}
