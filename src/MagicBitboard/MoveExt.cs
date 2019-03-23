using MagicBitboard.Enums;
using MagicBitboard.Helpers;
using System.Collections.Generic;

namespace MagicBitboard
{
    /// <summary>
    /// This is for move storage, long term.
    /// </summary>
    /// <remarks>
    /// Structure:
    /// bits 0-5: DestinationIndex (0-63)
    /// bits 6-11: OriginIndex (0-63)
    /// bits 12-14: Promotion Piece Type (Knight, Bishop, Rook, Queen)
    /// bits 14-16: MoveType
    /// </remarks>
    public class MoveExt : System.IEquatable<MoveExt>
    {
        public readonly ushort Move;

        public MoveExt(ushort move) { Move = move; }
        public ushort DestinationIndex => MoveHelpers.DestinationIndex(Move);
        public ushort SourceIndex => MoveHelpers.SourceIndex(Move);
        public ulong DestinationValue => MoveHelpers.DestinationValue(Move);
        public ulong SourceValue => MoveHelpers.SourceValue(Move);
        public MoveType MoveType => MoveHelpers.GetMoveType(Move);
        public PromotionPiece PromotionPiece => MoveHelpers.GetPiecePromoted(Move);
        public List<MoveExt> Variations;

        public bool Equals(MoveExt other) => Move == other.Move;
        public bool Equals(ushort other) => Move == other;

        public override string ToString()
        {
            if (MoveType == MoveType.Promotion)
            {
                return $"{DestinationIndex.IndexToSquareDisplay()}={PieceHelpers.GetCharFromPromotionPiece(PromotionPiece)}";
            }
            return "";
        }
    }
}
