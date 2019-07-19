using System;
using ChessLib.Data.Types.Enums;

namespace ChessLib.Data.Types.Interfaces
{
    public interface IBoard : ICloneable
    {
        Color ActivePlayer { get; set; }
        CastlingAvailability CastlingAvailability { get; set; }
        ushort? EnPassantSquare { get; set; }
        uint FullmoveCounter { get; set; }
        ushort HalfmoveClock { get; set; }
        ulong[][] GetPiecePlacement();
        bool Chess960 { get; }
        string InitialFEN { get; }
    }
}