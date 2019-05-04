using System;
using ChessLib.Data.Types;

namespace ChessLib.Data.MoveRepresentation
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