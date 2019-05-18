using ChessLib.Data;
using ChessLib.Data.Exceptions;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types;

namespace ChessLib.Validators.MoveValidation.EnPassantRules
{
    public class EnPassantSquareIsAttackedBySource : IMoveRule
    {
        public MoveExceptionType? Validate(in BoardFENInfo boardInfo, in ulong[][] postMoveBoard, in MoveExt move)
        {
            var pawnAttacksFromSquare = Bitboard.GetAttackedSquares(Piece.Pawn, move.SourceIndex,
                boardInfo.TotalOccupancy, boardInfo.ActivePlayer);
            var isAttacked = (pawnAttacksFromSquare & move.DestinationValue) != 0;
            if (isAttacked) return null;
            return MoveExceptionType.EP_NotAttackedBySource;
        }
    }
}
