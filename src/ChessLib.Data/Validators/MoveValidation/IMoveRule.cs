using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types.Exceptions;
using ChessLib.Data.Types.Interfaces;

namespace ChessLib.Data.Validators.MoveValidation
{

    public interface IMoveRule
    {
        MoveError Validate(in IBoard boardInfo, in ulong[][] postMoveBoard, in IMoveExt move);
    }
}
