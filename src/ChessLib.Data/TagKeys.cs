using System;

namespace ChessLib.Data
{
    [Flags]
    public enum TagKeys
    {
        Event = 1 << 1,
        Site = 1 << 2,
        Date = 1 << 3,
        Round = 1 << 4,
        White = 1 << 5,
        Black = 1 << 6,
        Result = 1 << 7,
        Annotator = 1 << 8,
        TimeControl = 1 << 9,
        FEN = 1 << 10,
        All = Event | Site | Date | Round | White | Black | Result | Annotator | TimeControl | FEN
    }
}