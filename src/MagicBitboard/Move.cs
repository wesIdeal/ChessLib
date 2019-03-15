using MagicBitboard.Enums;
using MagicBitboard.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Text;


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

        public bool Equals(MoveExt other) => Move == other.Move;
        public bool Equals(ushort other) => Move == other;

        public override string ToString()
        {
            if (MoveType == MoveType.Promotion)
            {
                return $"{DestinationIndex.IndexToSquareDisplay()}={PieceHelper.GetCharFromPromotionPiece(PromotionPiece)}";
            }
            return "";
        }
    }




    public static class MoveHelpers
    {
        private static string[] castling = new[] { "O-O", "O-O-O" };
        public static MoveExt GenerateMove(ushort fromIdx, ushort toIdx, MoveType moveType = MoveType.Normal, PromotionPiece promotionPiece = 0)
        {
            var mt = (ushort)moveType << 14;
            var pp = (ushort)promotionPiece << 12;
            var origin = fromIdx << 6;
            var dest = toIdx << 0;
            return new MoveExt((ushort)(mt | pp | origin | dest));
        }

        public static MoveExt GenerateMoveFromText(string moveText, Color moveColor)
        {
            moveText = moveText.TrimEnd('#', '?', '!', '+').Trim();
            MoveType moveType = MoveType.Normal;
            MoveExt rvMove = new MoveExt(0);
            if (castling.Contains(moveText.ToLowerInvariant()))
            {
                moveType = MoveType.Castle;
            }
            else if (moveText.Contains("="))
            {
                var promotionPieces = moveText.Split('=');
                ushort dest = (ushort)BoardHelpers.SquareTextToIndex(promotionPieces[0]).Value;
                var promotionPiece = PieceHelper.GetPromotionPieceFromChar(promotionPieces[1][0]);
                var source = (ushort)(moveColor == Color.White ? dest - 8 : dest + 8);
                rvMove = GenerateMove(source, dest, MoveType.Promotion, promotionPiece);

            }

            return rvMove;
        }

        public static ushort DestinationIndex(this ushort move) => (ushort)(move & 63);
        public static ushort SourceIndex(this ushort move) => (ushort)((move >> 6) & 63);

        public static ulong DestinationValue(this ushort move) => 1ul << (move & 63);
        public static ulong SourceValue(this ushort move) => 1ul << ((move >> 6) & 63);

        public static PromotionPiece GetPiecePromoted(this ushort move) => (PromotionPiece)((move >> 12) & 3);
        public static MoveType GetMoveType(this ushort move) => (MoveType)((move >> 14) & 3);

    }
}
