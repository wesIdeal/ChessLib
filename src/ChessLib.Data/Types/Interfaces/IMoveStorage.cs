using ChessLib.Data.MoveRepresentation;
using ChessLib.Types.Enums;
using System;

namespace ChessLib.Types.Interfaces
{
    public interface IMove
    {
    }

    public interface IMoveStorage : IMoveNode<IMoveExt>, IEquatable<IMoveStorage>, IMove
    {
        Color ColorMoving { get; }
        Piece PieceMoving { get; }
        string SAN { get; }
    }
}