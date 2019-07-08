using System;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types.Enums;

namespace ChessLib.Data.Types.Interfaces
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