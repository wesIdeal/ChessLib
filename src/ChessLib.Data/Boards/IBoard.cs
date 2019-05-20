using System;
using ChessLib.Data.Types;

namespace ChessLib.Data
{
    public interface IBoard : ICloneable
    {
        Color ActivePlayer { get; set; }
        CastlingAvailability CastlingAvailability { get; set; }
        ushort? EnPassantSquare { get; set; }
        uint FullmoveCounter { get; set; }
        uint? HalfmoveClock { get; set; }
        ulong[][] PiecePlacement { get; set; }

        string ToFEN();
    }
}