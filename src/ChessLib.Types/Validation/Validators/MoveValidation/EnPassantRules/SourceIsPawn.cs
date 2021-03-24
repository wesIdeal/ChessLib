using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Types.Helpers;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Validation.Validators.MoveValidation.EnPassantRules
{
    public class SourceIsPawn : IMoveRule
    {
        public MoveError Validate(in IBoard boardInfo, in ulong[][] postMoveBoard, in IMove move)
        {
            var isPawn = (boardInfo.Occupancy.Occupancy(boardInfo.ActivePlayer, Piece.Pawn) & move.SourceValue) != 0;
            return isPawn ? MoveError.NoneSet : MoveError.EpSourceIsNotPawn;
        }
    }
}