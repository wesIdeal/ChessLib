using ChessLib.Types.Enums;
using System;

namespace ChessLib.Types.Interfaces
{
    public interface IMove
    {
    }

    public interface IMoveStorage : IEquatable<IMoveStorage>, IMove
    {
        Color ColorMoving { get; }
        ushort Move { get; }
        Piece PieceMoving { get; }
    }
}