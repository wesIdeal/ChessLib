using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Validation.Validators.MoveValidation.EnPassantRules
{
    public class EnPassantSquareIsAttackedBySource : IMoveRule
    {
        public MoveError Validate(in IBoard boardInfo, in ulong[][] postMoveBoard, in IMove move)
        {
            var pawnAttacksFromSquare = Bitboard.Instance.GetAttackedSquares(Piece.Pawn, move.SourceIndex,
                boardInfo.Occupancy.Occupancy(), boardInfo.ActivePlayer);
            var isAttacked = (pawnAttacksFromSquare & move.DestinationValue) != 0;
            return isAttacked ? MoveError.NoneSet : MoveError.EpNotAttackedBySource;
        }
    }
}
