using ChessLib.Data.MoveRepresentation;
using ChessLib.Types;
using ChessLib.Types.Exceptions;
using ChessLib.Types.Interfaces;

namespace ChessLib.Data.Validators.MoveValidation
{

    public interface IMoveRule
    {
        MoveError Validate(in IBoard boardInfo, in ulong[][] postMoveBoard, in IMoveExt move);
    }
}
