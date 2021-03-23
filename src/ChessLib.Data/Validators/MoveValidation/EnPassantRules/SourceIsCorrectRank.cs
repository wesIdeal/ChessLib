using ChessLib.Data.MoveRepresentation;
using ChessLib.Types;
using ChessLib.Types.Enums;
using ChessLib.Types.Exceptions;
using ChessLib.Types.Interfaces;

namespace ChessLib.Data.Validators.MoveValidation.EnPassantRules
{
    public class SourceIsCorrectRank : IMoveRule
    {
        public MoveError Validate(in IBoard boardInfo, in ulong[][] postMoveBoard, in IMoveExt move)
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