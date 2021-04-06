using System;
using ChessLib.Core.Types.Enums;

namespace ChessLib.Core.Types.Interfaces
{
    public interface IBoard : IBoardState, ICloneable
    {
        ulong[][] Occupancy { get; }

        ulong[][] CloneOccupancy();
    }
}