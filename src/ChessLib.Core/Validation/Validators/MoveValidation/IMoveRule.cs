using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Validation.Validators.MoveValidation
{
    public interface IMoveRule
    {
        MoveError Validate(in Board boardInfo, in ulong[][] postMoveBoard, in IMove move);
    }
}