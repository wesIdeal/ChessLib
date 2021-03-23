using System;
using System.ComponentModel;

namespace ChessLib.Types.Enums
{
    [Flags]
    public enum CastlingAvailability
    {
        [Description("-")]
        NoCastlingAvailable = 0,
        [Description("K")]
        WhiteKingside = 1,
        [Description("Q")]
        WhiteQueenside = 2,
        [Description("k")]
        BlackKingside = 4,
        [Description("q")]
        BlackQueenside = 8
    }
}