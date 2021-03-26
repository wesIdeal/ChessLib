using System;

namespace ChessLib.Core.Types.Enums
{
    [Flags]
    public enum MoveDirection
    {
        N = 1,
        NE = 2,
        E = 4,
        SE = 8,
        S = 16,
        SW = 32,
        W = 64,
        NW = 128
    }
}