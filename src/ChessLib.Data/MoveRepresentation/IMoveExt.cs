using System;
using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;

namespace ChessLib.Data.MoveRepresentation
{
    public interface IMoveExt : IEquatable<IMoveExt>, IMove
    {
        ushort DestinationIndex { get; }
        ulong DestinationValue { get; }
        ushort Move { get; }
        MoveType MoveType { get; set; }
        PromotionPiece PromotionPiece { get; }
        ushort SourceIndex { get; }
        ulong SourceValue { get; }
        bool Equals(ushort other);
        string ToString();
    }
}