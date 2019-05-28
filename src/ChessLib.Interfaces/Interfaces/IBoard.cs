using ChessLib.Types.Enums;
using System;

namespace ChessLib.Types.Interfaces
{
    public interface IBoard : ICloneable
    {
        Color ActivePlayer { get; set; }
        CastlingAvailability CastlingAvailability { get; set; }
        ulong TotalOccupancy { get; }
        ushort? EnPassantSquare { get; set; }
        uint FullmoveCounter { get; set; }
        uint? HalfmoveClock { get; set; }
        IPiecePlacement PiecePlacement { get; set; }
        bool Chess960 { get; }
        string InitialFEN { get; }
    }
}