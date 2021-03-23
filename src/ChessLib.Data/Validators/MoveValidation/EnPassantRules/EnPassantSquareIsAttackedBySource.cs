using ChessLib.Data.Helpers;
using ChessLib.Data.Magic;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Types;
using ChessLib.Types.Enums;
using ChessLib.Types.Exceptions;
using ChessLib.Types.Interfaces;

namespace ChessLib.Data.Validators.MoveValidation.EnPassantRules
{
    public class EnPassantSquareIsAttackedBySource : IMoveRule
    {
        public MoveError Validate(in IBoard boardInfo, in ulong[][] postMoveBoard, in IMoveExt move)
        {
            var pawnAttacksFromSquare = Bitboard.GetAttackedSquares(Piece.Pawn, move.SourceIndex,
                boardInfo.Occupancy.Occupancy(), boardInfo.ActivePlayer);
            var isAttacked = (pawnAttacksFromSquare & move.DestinationValue) != 0;
            return isAttacked ? MoveError.NoneSet : MoveError.EpNotAttackedBySource;
        }
    }
}
