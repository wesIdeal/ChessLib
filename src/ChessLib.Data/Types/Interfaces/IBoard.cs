using System;
using ChessLib.Data.Types.Enums;

namespace ChessLib.Data.Types.Interfaces
{
    public interface IBoard : ICloneable
    {
        GameState GameState { get; }
        Color ActivePlayer { get; }
        CastlingAvailability CastlingAvailability { get; }
        ushort? EnPassantSquare { get; }
        ushort FullMoveCounter { get; }
        ushort HalfMoveClock { get; }
        ulong[][] Occupancy { get; }
    }
}