using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Validation.Validators.MoveValidation.EnPassantRules
{
    public class SourceIsCorrectRank : IMoveRule
    {
        public MoveError Validate(in Board boardInfo, in ulong[][] postMoveBoard, in IMove move)
        {
            var rank = move.SourceIndex / 8;
            var error = MoveError.EpWrongSourceRank;
            return (boardInfo.ActivePlayer == Color.Black && rank == 3) ||
                   boardInfo.ActivePlayer == Color.White && rank == 4
                ? MoveError.NoneSet
                : error;
        }
    }
}