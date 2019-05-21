using ChessLib.Types.Enums;

namespace ChessLib.Types.Interfaces
{
    public interface IBoardRule
    {
        BoardException Validate(in IBoard boardInfo);
    }
}

