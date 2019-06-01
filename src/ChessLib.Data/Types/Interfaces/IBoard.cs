using ChessLib.Types.Enums;
using System;

namespace ChessLib.Types.Interfaces
{
    public interface IBoard : ICloneable
    {
        Color ActivePlayer { get; set; }
        CastlingAvailability CastlingAvailability { get; set; }
        ushort? EnPassantSquare { get; set; }
        uint FullmoveCounter { get; set; }
        uint? HalfmoveClock { get; set; }
        ulong[][] GetPiecePlacement();
        bool Chess960 { get; }
        string InitialFEN { get; }
    }
}