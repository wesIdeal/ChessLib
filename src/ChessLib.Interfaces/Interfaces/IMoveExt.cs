using ChessLib.Types.Enums;

namespace ChessLib.Types.Interfaces
{
    public interface IMoveExt
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