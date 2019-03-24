using ChessLib.Data.Helpers;
using ChessLib.Data.Types;
using System.Collections.Generic;

namespace ChessLib.Data.MoveRepresentation
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
        public ushort Move { get => _move; private set { _move = value; } }
        public ushort _move;
        public MoveExt(ushort move) { Move = move; }
        public ushort DestinationIndex => MoveHelpers.DestinationIndex(Move);
        public ushort SourceIndex => MoveHelpers.SourceIndex(Move);
        public ulong DestinationValue => MoveHelpers.DestinationValue(Move);
        public ulong SourceValue => MoveHelpers.SourceValue(Move);
        public MoveType MoveType
        {
            get { return MoveHelpers.GetMoveType(Move); }
            set
            {
                ushort mt = (ushort)((ushort)value << 14);
                Move &= 0x1fff;
                Move |= mt;
            }
        }
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
