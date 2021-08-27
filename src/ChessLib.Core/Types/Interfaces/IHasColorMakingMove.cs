using ChessLib.Core.Types.Enums;

namespace ChessLib.Core.Types.Interfaces
{
    public interface IHasColorMakingMove
    {
        Color ColorMakingMove { get; }
    }

    public interface IHasActiveColor
    {
        Color ActiveColor { get; }
    }
}