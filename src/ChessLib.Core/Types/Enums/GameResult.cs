using System.ComponentModel;

namespace ChessLib.Core.Types.Enums
{
    public enum GameResult
    {
        [Description("*")] None,
        [Description("1-0")] WhiteWins,
        [Description("0-1")] BlackWins,
        [Description("1/2-1/2")] Draw
    }
}