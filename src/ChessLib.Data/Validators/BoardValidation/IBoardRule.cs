using ChessLib.Data.Types.Enums;
using ChessLib.Data.Types.Interfaces;

namespace ChessLib.Data.Validators.BoardValidation
{
    public interface IBoardRule
    {
        BoardExceptionType Validate(in IBoard boardInfo);
    }
}

