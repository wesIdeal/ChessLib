using System;

namespace ChessLib.Core.Types.Interfaces
{
    public interface IPostMoveState : ICloneable, IHasSan
    {
        uint BoardState { get; }
        ushort MoveValue { get; }
    }
}