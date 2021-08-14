using System;
using ChessLib.Core.Types.Enums;

namespace ChessLib.Core.Types.Interfaces
{
    public interface IMove : IEquatable<IMove>
    {
        ushort DestinationIndex { get; }
        ulong DestinationValue { get; }
        ushort MoveValue { get; }
        MoveType MoveType { get; }
        PromotionPiece PromotionPiece { get; }
        ushort SourceIndex { get; }
        ulong SourceValue { get; }
        
    }
}