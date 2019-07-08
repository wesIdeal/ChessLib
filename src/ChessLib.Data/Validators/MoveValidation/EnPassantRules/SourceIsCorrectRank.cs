using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types.Enums;
using ChessLib.Data.Types.Exceptions;
using ChessLib.Data.Types.Interfaces;

namespace ChessLib.Data.Validators.MoveValidation.EnPassantRules
{
    public class SourceIsCorrectRank : IMoveRule
    {
        public MoveError? Validate(in IBoard boardInfo, in ulong[][] postMoveBoard, in IMoveExt move)
        {
            var rank = move.SourceIndex / 8;
            var error = MoveError.EpWrongSourceRank;
            if ((boardInfo.ActivePlayer == Color.Black && rank == 3) ||
                boardInfo.ActivePlayer == Color.White && rank == 4)
                return null;
            return error;
        }
    }
}