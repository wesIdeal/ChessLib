using ChessLib.Types.Interfaces;
using System;

namespace ChessLib.Data.MoveRepresentation
{
    public interface IMoveText : IEquatable<IMoveText>, IMove
    {
        string SAN { get; set; }
        string Comment { get; set; }
        string NAG { get; set; }
    }
}