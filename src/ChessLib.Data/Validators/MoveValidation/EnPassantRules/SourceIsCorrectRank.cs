using ChessLib.Data.Exceptions;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;

namespace ChessLib.Validators.MoveValidation.EnPassantRules
{
    public class SourceIsCorrectRank : IMoveRule
    {
        public MoveExceptionType? Validate(in IBoard boardInfo, in ulong[][] postMoveBoard, in IMoveExt move)
        {
            var rank = move.SourceIndex / 8;
            var error = MoveExceptionType.EP_WrongSourceRank;
            if ((boardInfo.ActivePlayer == Color.Black && rank == 3) ||
                boardInfo.ActivePlayer == Color.White && rank == 4)
                return null;
            return error;
        }
    }
}