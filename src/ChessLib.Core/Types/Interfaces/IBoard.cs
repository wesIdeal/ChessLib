using System;

namespace ChessLib.Core.Types.Interfaces
{
    public interface IBoard : ICloneable
    {
        ulong[][] Occupancy { get; }
    }
}