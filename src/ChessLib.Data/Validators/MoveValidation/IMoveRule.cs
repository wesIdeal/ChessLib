using ChessLib.Data;
using ChessLib.Data.Exceptions;
using ChessLib.Data.MoveRepresentation;

namespace ChessLib.Types.Interfaces
{

    public interface IMoveRule
    {
        MoveError? Validate(in IBoard boardInfo, in ulong[][] postMoveBoard, in IMoveExt move);
    }
}
