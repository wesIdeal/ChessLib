using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;

namespace ChessLib.Core.Validation.Validators.BoardValidation
{
    public interface IBoardRule
    {
        BoardExceptionType Validate(in Board boardInfo);
    }
}