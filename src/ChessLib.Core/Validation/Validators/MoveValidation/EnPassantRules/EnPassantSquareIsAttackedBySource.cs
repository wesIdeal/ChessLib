using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Validation.Validators.MoveValidation.EnPassantRules
{
    public class EnPassantSquareIsAttackedBySource : IMoveRule
    {
        public MoveError Validate(in Board boardInfo, in ulong[][] postMoveBoard, in IMove move)
        {
            var occupancy = boardInfo.Occupancy.Occupancy() | ( boardInfo.EnPassantIndex?.GetBoardValueOfIndex() ?? 0);
            var pawnAttacksFromSquare = Bitboard.Instance.GetAttackedSquares(Piece.Pawn, move.SourceIndex,
                occupancy, boardInfo.ActivePlayer);
            var isAttacked = (pawnAttacksFromSquare & move.DestinationValue) != 0;
            return isAttacked ? MoveError.NoneSet : MoveError.EpNotAttackedBySource;
        }
    }
}