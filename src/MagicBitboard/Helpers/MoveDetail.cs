using MagicBitboard.Enums;
using System;
using System.Collections.Generic;

namespace MagicBitboard.Helpers
{
    public static partial class MoveHelpers
    {
        public class MoveDetail : IEquatable<MoveDetail>
        {
            public MoveDetail()
            {
            }

            public MoveDetail(ushort? sourceRank, ushort? sourceFile, ushort? destRank, ushort? destFile, Piece piece, Color color, string moveText, bool isCapture = false, MoveType moveType = MoveType.Normal, PromotionPiece? promotionPiece = null)
            {
                SourceFile = sourceFile;
                SourceRank = sourceRank;
                DestFile = destFile;
                DestRank = destRank;
                Piece = piece;
                PromotionPiece = promotionPiece;
                MoveType = moveType;
                IsCapture = isCapture;
                Color = color;
                MoveText = moveText;
            }

            public ushort? SourceFile { get; set; }
            public ushort? SourceRank { get; set; }
            public ushort? DestFile { get; set; }
            public ushort? DestRank { get; set; }
            public Piece Piece { get; set; }
            public PromotionPiece? PromotionPiece { get; set; }
            public MoveType MoveType { get; set; }
            public bool IsCapture { get; set; }
            public Color Color { get; set; }
            public string MoveText { get; set; }
            public ushort? DestinationIndex
            {
                get => DestFile.HasValue && DestRank.HasValue ? (ushort)(DestFile + (DestRank * 8)) : (ushort?)null;
            }
            public ushort? SourceIndex
            {
                get => SourceFile.HasValue && SourceRank.HasValue ? (ushort)(SourceFile + (SourceRank * 8)) : (ushort?)null;
            }
            public override bool Equals(object obj)
            {
                var detail = obj as MoveDetail;
                return this.Equals(detail);
            }

            public bool Equals(MoveDetail other)
            {
                return other != null &&
                      SourceFile == other.SourceFile &&
                      SourceRank == other.SourceRank &&
                      DestFile == other.DestFile &&
                      DestRank == other.DestRank &&
                      Piece == other.Piece &&
                      IsCapture == other.IsCapture &&
                      MoveType == other.MoveType &&
                      PromotionPiece == other.PromotionPiece &&
                      Color == other.Color &&
                      MoveText == other.MoveText;
            }

            public override int GetHashCode()
            {
                var hashCode = -1046514577;
                hashCode = hashCode * -1521134295 + EqualityComparer<ushort?>.Default.GetHashCode(SourceFile);
                hashCode = hashCode * -1521134295 + EqualityComparer<ushort?>.Default.GetHashCode(SourceRank);
                hashCode = hashCode * -1521134295 + EqualityComparer<ushort?>.Default.GetHashCode(DestFile);
                hashCode = hashCode * -1521134295 + EqualityComparer<ushort?>.Default.GetHashCode(DestRank);
                hashCode = hashCode * -1521134295 + Piece.GetHashCode();
                hashCode = hashCode * -1521134295 + EqualityComparer<PromotionPiece?>.Default.GetHashCode(PromotionPiece);
                hashCode = hashCode * -1521134295 + MoveType.GetHashCode();
                hashCode = hashCode * -1521134295 + IsCapture.GetHashCode();
                hashCode = hashCode * -1521134295 + Color.GetHashCode();
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MoveText);
                return hashCode;
            }
        }
    }
}
