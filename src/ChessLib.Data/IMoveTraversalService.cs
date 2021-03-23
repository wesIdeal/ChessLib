using ChessLib.Data.MoveRepresentation;
using System;

namespace ChessLib.Data
{
    public enum MoveApplicationStrategy
    {
        ContinueMainLine,
        Variation,
        NewMainLine,
        ReplaceMove
    }

}
