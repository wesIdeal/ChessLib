using ChessLib.Data.Types.Enums;
using ChessLib.Data.Types.Exceptions;
using System;

namespace ChessLib.Data.MoveRepresentation
{

    /// <summary>
    /// Holds intermediate move inforation (in-between text SAN and ushort), while transforming from text to a binary move
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
            return Equals(detail);
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

    }
}
