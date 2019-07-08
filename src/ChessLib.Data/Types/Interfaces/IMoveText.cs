using System;

namespace ChessLib.Data.Types.Interfaces
{
    public interface IMoveText : IEquatable<IMoveText>, IMove
    {
        string SAN { get; set; }
        string Comment { get; set; }
        string NAG { get; set; }
    }
}