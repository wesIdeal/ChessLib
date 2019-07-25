using ChessLib.Data.Helpers;
using ChessLib.Data.Magic;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types.Enums;
using ChessLib.Data.Types.Exceptions;
using ChessLib.Data.Types.Interfaces;

namespace ChessLib.Data.Validators.MoveValidation.EnPassantRules
{
    public class EnPassantSquareIsAttackedBySource : IMoveRule
    {
        public MoveError Validate(in IBoard boardInfo, in ulong[][] postMoveBoard, in IMoveExt move)
        {
            var pawnAttacksFromSquare = Bitboard.GetAttackedSquares(Piece.Pawn, move.SourceIndex,
                boardInfo.TotalOccupancy(), boardInfo.ActivePlayer);
            var isAttacked = (pawnAttacksFromSquare & move.DestinationValue) != 0;
            return isAttacked ? MoveError.NoneSet : MoveError.EpNotAttackedBySource;
        }
    }
}
