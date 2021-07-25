using System;
using ChessLib.Core.Types.Enums;

namespace ChessLib.Core.Types.Interfaces
{
    public interface IBoard : ICloneable
    {
        ulong[][] Occupancy { get; }

        ulong[][] CloneOccupancy();
    }
}