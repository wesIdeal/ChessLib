using ChessLib.Data.Exceptions;
using ChessLib.Types.Enums;
using System;
using System.Collections.Generic;

namespace ChessLib.Data.MoveRepresentation
{

    /// <summary>
    /// Holds intermediate move inforation, while transforming from text to a binary move
    /// </summary>
    public class MoveDetail : IEquatable<MoveDetail>
    {
        public MoveDetail()
        { }
        /// <summary>
        /// Constructs a new move detail object from rank and file.
        /// </summary>
        /// <param name="sourceRank"></param>
        /// <param name="sourceFile"></param>
        /// <param name="destRank"></param>
        /// <param name="destFile"></param>
        /// <param name="piece"></param>
        /// <param name="color"></param>
        /// <param name="moveText">Used only for exception messages.</param>
        /// <param name="isCapture"></param>
        /// <param name="moveType"></param>
        /// <param name="promotionPiece"></param>
        public MoveDetail(ushort? sourceRank, ushort? sourceFile, ushort? destRank, ushort? destFile, Piece piece, Color color, string moveText = "", bool isCapture = false, MoveType moveType = MoveType.Normal, PromotionPiece? promotionPiece = null)
        {
            SourceFile = sourceFile;
            SourceRank = sourceRank;
            DestinationFile = destFile;
            DestinationRank = destRank;
            Piece = piece;
            PromotionPiece = promotionPiece;
            MoveType = moveType;
            IsCapture = isCapture;
            Color = color;
            MoveText = moveText;
        }

        /// <summary>
        /// Constructs a new move detail object from indexes.
        /// </summary>
        /// <param name="sourceIndex"></param>
        /// <param name="destinationIndex"></param>
        /// <param name="piece"></param>
        /// <param name="color"></param>
        /// <param name="moveText">Used only for exception messages</param>
        /// <param name="isCapture"></param>
        /// <param name="moveType"></param>
        /// <param name="promotionPiece"></param>
        public MoveDetail(ushort? sourceIndex, ushort? destinationIndex, Piece piece, Color color, string moveText = "", bool isCapture = false, MoveType moveType = MoveType.Normal, PromotionPiece? promotionPiece = null)
        {
            SourceIndex = sourceIndex;
            DestinationIndex = destinationIndex;

            Piece = piece;
            PromotionPiece = promotionPiece;
            MoveType = moveType;
            IsCapture = isCapture;
            Color = color;
            MoveText = moveText;
        }

        public ushort? SourceFile { get; set; }
        public ushort? SourceRank { get; set; }
        public ushort? SourceIndex
        {
            get => SourceFile.HasValue && SourceRank.HasValue ? (ushort)(SourceFile + (SourceRank * 8)) : (ushort?)null;
            set
            {
                if (value == null)
                {
                    SourceRank = SourceFile = null;
                    return;
                }

                ValidateIndexes(value.Value, out ushort r, out ushort f);
                SourceRank = r;
                SourceFile = f;
            }

        }

        public ushort? DestinationFile { get; set; }
        public ushort? DestinationRank { get; set; }
        public ushort? DestinationIndex
        {
            get => DestinationFile.HasValue && DestinationRank.HasValue ? (ushort)(DestinationFile + (DestinationRank * 8)) : (ushort?)null;
            set
            {
                if (value == null)
                {
                    DestinationRank = DestinationFile = null;
                    return;
                }
                ValidateIndexes(value.Value, out ushort r, out ushort f);
                DestinationRank = r;
                DestinationFile = f;
            }
        }

        public Piece Piece { get; set; }
        public PromotionPiece? PromotionPiece { get; set; }
        public MoveType MoveType { get; set; }
        public bool IsCapture { get; set; }
        public Color Color { get; set; }
        public string MoveText { get; set; }

        private static void ValidateIndexes(ushort ids, out ushort r, out ushort f)
        {
            r = (ushort)(ids / 8);
            f = (ushort)(ids % 8);
            if (ids > 63)
            {
                throw new MoveException("Board Indexes must be between 0 and 63.");
            }
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
                  DestinationFile == other.DestinationFile &&
                  DestinationRank == other.DestinationRank &&
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
            hashCode = hashCode * -1521134295 + EqualityComparer<ushort?>.Default.GetHashCode(DestinationFile);
            hashCode = hashCode * -1521134295 + EqualityComparer<ushort?>.Default.GetHashCode(DestinationRank);
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
