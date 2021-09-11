using ChessLib.Core.Types;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Validation.MoveValidation
{
    public interface IMoveRule
    {
        MoveError Validate(in Board boardInfo,  in IMove move);
    }
}