using ChessLib.Data;
using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;

namespace ChessLib.Validators.MoveValidation.EnPassantRules
{
    public class EnPassantSquareIsAttackedBySource : IMoveRule
    {
        public MoveExceptionType? Validate(in IBoard boardInfo, in ulong[][] postMoveBoard, in IMoveExt move)
        {
            var pawnAttacksFromSquare = Bitboard.GetAttackedSquares(Piece.Pawn, move.SourceIndex,
                boardInfo.TotalOccupancy(), boardInfo.ActivePlayer);
            var isAttacked = (pawnAttacksFromSquare & move.DestinationValue) != 0;
            if (isAttacked) return null;
            return MoveExceptionType.EP_NotAttackedBySource;
        }
    }
}
