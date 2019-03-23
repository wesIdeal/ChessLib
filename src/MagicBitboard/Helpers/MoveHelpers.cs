using MagicBitboard.Enums;
using MagicBitboard.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MagicBitboard.Helpers
{
    public static class MoveHelpers
    {
        private const string rxPieces = "[NBRQK]";
        private const string rxFiles = "[a-h]";
        private const string rxRanks = "[1-8]";
        private static readonly string rxMoveDetails = $"((?<piece>{rxPieces})((?<sourceFile>{rxFiles})|(?<sourceRank>{rxRanks}))?|(?<pawnFile>{rxFiles}))(?<capture>x)?(?<destinationFile>{rxFiles})(?<destinationRank>{rxRanks})|((?<pawnFile>{rxFiles})(?<destinationRank>{rxRanks}))";
        private const string rxCastleLongGroup = "castleLong";
        private const string rxCastleShortGroup = "castleShort";
        private static readonly string rxCastle = $"(?<{rxCastleLongGroup}>O-O-O)|(?<{rxCastleShortGroup}>O-O)";

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
            //var pieceMoved = GetMoveDetails(moveText);
            if (Regex.IsMatch(moveText, rxCastle))
            {
                moveType = MoveType.Castle;
            }
            else if (moveText.Contains("="))
            {
                var promotionPieces = moveText.Split('=');
                ushort dest = (ushort)BoardHelpers.SquareTextToIndex(promotionPieces[0]).Value;
                var promotionPiece = PieceHelpers.GetPromotionPieceFromChar(promotionPieces[1][0]);
                var source = (ushort)(moveColor == Color.White ? dest - 8 : dest + 8);
                rvMove = GenerateMove(source, dest, MoveType.Promotion, promotionPiece);

            }

            return rvMove;
        }

        public static MoveDetail GetAvailableMoveDetails(string move, Color color)
        {
            MoveDetail md = new MoveDetail();
            if (move.Length < 2) throw new System.Exception("Invalid move.");
            Match match;
            if ((match = Regex.Match(move, rxCastle)).Success)
            {
                md.Piece = Piece.King;
                md.Castle = true;
                md.SourceFile = 4;
                md.DestRank = md.SourceRank = (ushort)(color == Color.Black ? 7 : 0);
                if (match.Groups[rxCastleLongGroup].Success) { md.DestFile = 2; }
                else { md.DestFile = 6; }
                return md;
            }
            match = Regex.Match(move, rxMoveDetails);
            md.Capture = match.Groups["capture"].Success;
            if (match.Groups["pawnFile"].Success)
                md.Piece = Piece.Pawn;
            else
            {
                var pieceMatch = match.Groups["piece"];
                md.Piece = PieceOfColor.GetPiece(pieceMatch.Value[0]);
            }

            md.SourceFile = match.Groups["sourceFile"].Success ? (ushort)(match.Groups["sourceFile"].Value[0] - 'a') : (ushort?)null;
            md.SourceRank = match.Groups["sourceRank"].Success ? (ushort)(ushort.Parse(match.Groups["sourceRank"].Value) - 1) : (ushort?)null;
            md.DestFile = match.Groups["destinationFile"].Success ? (ushort)(match.Groups["destinationFile"].Value[0] - 'a') : (ushort?)null;
            md.DestRank = match.Groups["destinationRank"].Success ? (ushort)(ushort.Parse((match.Groups["destinationRank"].Value)) - 1) : (ushort?)null;

            return md;
        }

        public static ushort DestinationIndex(this ushort move) => (ushort)(move & 63);
        public static ushort SourceIndex(this ushort move) => (ushort)((move >> 6) & 63);

        public static ulong DestinationValue(this ushort move) => 1ul << (move & 63);
        public static ulong SourceValue(this ushort move) => 1ul << ((move >> 6) & 63);

        public static PromotionPiece GetPiecePromoted(this ushort move) => (PromotionPiece)((move >> 12) & 3);
        public static MoveType GetMoveType(this ushort move) => (MoveType)((move >> 14) & 3);

        public class MoveDetail : IEquatable<MoveDetail>
        {
            public MoveDetail()
            {
            }

            public MoveDetail(ushort? sourceFile, ushort? sourceRank, ushort? destFile, ushort? destRank, Piece piece, bool capture, bool castle)
            {
                SourceFile = sourceFile;
                SourceRank = sourceRank;
                DestFile = destFile;
                DestRank = destRank;
                Piece = piece;
                Capture = capture;
                Castle = castle;
            }

            public ushort? SourceFile { get; set; }
            public ushort? SourceRank { get; set; }
            public ushort? DestFile { get; set; }
            public ushort? DestRank { get; set; }
            public Piece Piece { get; set; }
            public bool Capture { get; set; }
            public bool Castle { get; set; }

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
                      Capture == other.Capture &&
                      Castle == other.Castle;
            }

            public override int GetHashCode()
            {
                var hashCode = -1528112291;
                hashCode = hashCode * -1521134295 + EqualityComparer<ushort?>.Default.GetHashCode(SourceFile);
                hashCode = hashCode * -1521134295 + EqualityComparer<ushort?>.Default.GetHashCode(SourceRank);
                hashCode = hashCode * -1521134295 + EqualityComparer<ushort?>.Default.GetHashCode(DestFile);
                hashCode = hashCode * -1521134295 + EqualityComparer<ushort?>.Default.GetHashCode(DestRank);
                hashCode = hashCode * -1521134295 + Piece.GetHashCode();
                hashCode = hashCode * -1521134295 + Capture.GetHashCode();
                hashCode = hashCode * -1521134295 + Castle.GetHashCode();
                return hashCode;
            }
        }
    }
}
