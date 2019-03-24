using MagicBitboard.Enums;
using MagicBitboard.Helpers;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MagicBitboard.Helpers
{
    public static partial class MoveHelpers
    {
        private const string rxPieces = "[NBRQK]";
        private const string rxFiles = "[a-h]";
        private const string rxRanks = "[1-8]";
        private static readonly string rxMoveDetails = $"((?<piece>{rxPieces})((?<sourceFile>{rxFiles})|(?<sourceRank>{rxRanks}))?|(?<pawnFile>{rxFiles}))(?<capture>x)?(?<destinationFile>{rxFiles})(?<destinationRank>{rxRanks})|((?<pawnFile>{rxFiles})(?<destinationRank>{rxRanks}))";
        private const string rxCastleLongGroup = "castleLong";
        private const string rxPromotion = "(?<sourceFile>[a-h])(((?<capture>x)(?<destinationFile>[a-h])(?<destinationRank>[1-8]))|(?<destinationRank>[1-8]))=(?<promotionPiece>[NBRQK])";
        private const string rxCastleShortGroup = "castleShort";
        private static readonly string rxCastle = $"(?<{rxCastleLongGroup}>O-O-O)|(?<{rxCastleShortGroup}>O-O)";

        public static ulong IndexToValue(this ushort idx) => (ulong)((ulong)0x01 << idx);

        public static MoveExt GenerateMove(ushort fromIdx, ushort toIdx, MoveType moveType = MoveType.Normal, PromotionPiece promotionPiece = 0)
        {
            var mt = (ushort)moveType << 14;
            var pp = (ushort)promotionPiece << 12;
            var origin = fromIdx << 6;
            var dest = toIdx << 0;
            return new MoveExt((ushort)(mt | pp | origin | dest));
        }

        public static ushort DestinationIndex(this ushort move) => (ushort)(move & 63);
        public static ushort SourceIndex(this ushort move) => (ushort)((move >> 6) & 63);

        public static ulong DestinationValue(this ushort move) => 1ul << (move & 63);
        public static ulong SourceValue(this ushort move) => 1ul << ((move >> 6) & 63);

        public static PromotionPiece GetPiecePromoted(this ushort move) => (PromotionPiece)((move >> 12) & 3);
        public static MoveType GetMoveType(this ushort move) => (MoveType)((move >> 14) & 3);

        public static MoveDetail GetAvailableMoveDetails(string move, Color color)
        {
            MoveDetail md = new MoveDetail() { MoveText = move, Color = color };
            if (move.Length < 2) throw new System.Exception("Invalid move.");
            Match match, promotionMatch, castleMatch;
            match = Regex.Match(move, rxMoveDetails);
            md.SourceFile = match.Groups["sourceFile"].Success ? (ushort)(match.Groups["sourceFile"].Value[0] - 'a') : (ushort?)null;
            md.SourceRank = match.Groups["sourceRank"].Success ? (ushort)(ushort.Parse(match.Groups["sourceRank"].Value) - 1) : (ushort?)null;
            md.DestFile = match.Groups["destinationFile"].Success ? (ushort)(match.Groups["destinationFile"].Value[0] - 'a') : (ushort?)null;
            md.DestRank = match.Groups["destinationRank"].Success ? (ushort)(ushort.Parse((match.Groups["destinationRank"].Value)) - 1) : (ushort?)null;
            md.IsCapture = match.Groups["capture"].Success;
            if ((promotionMatch = Regex.Match(move, rxPromotion)).Success)
            {
                md.MoveType = MoveType.Promotion;
                md.Piece = Piece.Pawn;
                md.PromotionPiece = PieceHelpers.GetPromotionPieceFromChar(promotionMatch.Groups["promotionPiece"].Value[0]);
                if (!md.IsCapture)
                {
                    md.DestFile = (ushort)(match.Groups["pawnFile"].Value[0] - 'a');
                }
                return md;
            }
            if ((castleMatch = Regex.Match(move, rxCastle)).Success)
            {
                md.Piece = Piece.King;
                md.MoveType = MoveType.Castle;
                md.SourceFile = 4;
                md.DestRank = md.SourceRank = (ushort)(color == Color.Black ? 7 : 0);
                if (castleMatch.Groups[rxCastleLongGroup].Success) { md.DestFile = 2; }
                else { md.DestFile = 6; }
                return md;
            }

            if (match.Groups["pawnFile"].Success)
            {
                md.Piece = Piece.Pawn;
                md.DestFile = match.Groups["pawnFile"].Success ? (ushort)(match.Groups["pawnFile"].Value[0] - 'a') : (ushort?)null;
                if (md.IsCapture)
                {
                    md.SourceFile = (ushort)(match.Groups["pawnFile"].Value[0] - 'a');
                    if (color == Color.Black)
                    {
                        md.SourceRank = (ushort)(md.DestRank.Value + 1);
                    }
                    else
                    {
                        md.SourceRank = (ushort)(md.DestRank.Value - 1);
                    }
                }
            }
            else
            {
                var pieceMatch = match.Groups["piece"];
                md.Piece = PieceHelpers.GetPiece(pieceMatch.Value[0]);
            }

            return md;
        }
    }
}
