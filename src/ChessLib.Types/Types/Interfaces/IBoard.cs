using System;
using ChessLib.Core.Types.Enums;

namespace ChessLib.Core.Types.Interfaces
{
    public interface IBoard : ICloneable
    {
        GameState GameState { get; }
        Color ActivePlayer { get; }
        CastlingAvailability CastlingAvailability { get; }
        ushort? EnPassantSquare { get; }
        uint FullMoveCounter { get; }
        ushort HalfMoveClock { get; }
        ulong[][] Occupancy { get; }
    }
}