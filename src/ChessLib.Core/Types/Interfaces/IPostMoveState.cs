using System;

namespace ChessLib.Core.Types.Interfaces
{
    public interface IPostMoveState : IEquatable<IPostMoveState>, ICloneable, IHasActiveColor, IHasColorMakingMove, IHasSan
    {
        uint BoardState { get; }
        ushort MoveValue { get; }
    }
}