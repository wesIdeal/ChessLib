using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;

namespace ChessLib.Data.Validators.BoardValidation
{
    public interface IBoardRule
    {
        BoardExceptionType Validate(in IBoard boardInfo);
    }
}

