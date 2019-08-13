using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types.Exceptions;
using ChessLib.Data.Types.Interfaces;
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
