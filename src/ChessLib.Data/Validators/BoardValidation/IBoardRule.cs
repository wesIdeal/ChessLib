using ChessLib.Data.Types.Enums;
using ChessLib.Data.Types.Interfaces;

namespace ChessLib.Data.Validators.BoardValidation
{
    public interface IBoardRule
    {
        BoardException Validate(in IBoard boardInfo);
    }
}

