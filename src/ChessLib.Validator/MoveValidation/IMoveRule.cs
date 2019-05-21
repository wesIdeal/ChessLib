using ChessLib.Data;
using ChessLib.Data.Exceptions;
using ChessLib.Data.MoveRepresentation;

namespace ChessLib.Validators.MoveValidation
{

    interface IMoveRule
    {
        MoveExceptionType? Validate(in IBoard boardInfo, in ulong[][] postMoveBoard, in MoveExt move);
    }
}
