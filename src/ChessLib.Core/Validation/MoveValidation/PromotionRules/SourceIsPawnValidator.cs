using ChessLib.Core.Helpers;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Validation.MoveValidation.PromotionRules
{
    internal class SourceIsPawnValidator : IMoveRule
    {
        public MoveError Validate(in Board boardInfo, in ulong[][] postMoveBoard, in IMove move)
        {
            var activePawnOccupancy = boardInfo.Occupancy.Occupancy(boardInfo.ActivePlayer, Piece.Pawn);
            var isSourcePawn = (move.SourceValue & activePawnOccupancy) == move.SourceValue;
            return isSourcePawn ? MoveError.NoneSet : MoveError.SourceIsNotPawn;
        }
    }
}
