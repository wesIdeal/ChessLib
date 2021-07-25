using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Validation.Validators.BoardValidation
{
    public interface IBoardRule
    {
        BoardExceptionType Validate(in Board boardInfo);
    }
}