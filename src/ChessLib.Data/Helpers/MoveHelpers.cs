using System.Diagnostics;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types;
using System.Text.RegularExpressions;

namespace ChessLib.Data.Helpers
{
    public static class MoveHelpers
    {
        private const string RegExPieces = "[NBRQK]";
        private const string RegExFiles = "[a-h]";
        private const string RegExRanks = "[1-8]";
        private static readonly string RegExMoveDetails = $"((?<piece>{RegExPieces})((?<sourceFile>{RegExFiles})|(?<sourceRank>{RegExRanks}))?|(?<pawnFile>{RegExFiles}))(?<capture>x)?(?<destinationFile>{RegExFiles})(?<destinationRank>{RegExRanks})|((?<pawnFile>{RegExFiles})(?<destinationRank>{RegExRanks}))";
        private const string RegExCastleLongGroup = "castleLong";
        private const string RegExPromotion = "(?<sourceFile>[a-h])(((?<capture>x)(?<destinationFile>[a-h])(?<destinationRank>[1-8]))|(?<destinationRank>[1-8]))=(?<promotionPiece>[NBRQK])";
        private const string RegExCastleShortGroup = "castleShort";
        private static readonly string RegExCastle = $"(?<{RegExCastleLongGroup}>O-O-O)|(?<{RegExCastleShortGroup}>O-O)";

        public static ulong IndexToValue(this ushort idx) => (ulong)0x01 << idx;

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
            if (move.Length < 2) throw new System.Exception("Invalid move. Must have at least 2 characters.");
            Match promotionMatch, castleMatch;
            var match = Regex.Match(move, RegExMoveDetails);
            md.SourceFile = match.Groups["sourceFile"].Success ? (ushort)(match.Groups["sourceFile"].Value[0] - 'a') : (ushort?)null;
            md.SourceRank = match.Groups["sourceRank"].Success ? (ushort)(ushort.Parse(match.Groups["sourceRank"].Value) - 1) : (ushort?)null;
            md.DestinationFile = match.Groups["destinationFile"].Success ? (ushort)(match.Groups["destinationFile"].Value[0] - 'a') : (ushort?)null;
            md.DestinationRank = match.Groups["destinationRank"].Success ? (ushort)(ushort.Parse((match.Groups["destinationRank"].Value)) - 1) : (ushort?)null;
            md.IsCapture = match.Groups["capture"].Success;
            
            if ((promotionMatch = Regex.Match(move, RegExPromotion)).Success)
            {
                md.MoveType = MoveType.Promotion;
                md.Piece = Piece.Pawn;
                md.PromotionPiece = PieceHelpers.GetPromotionPieceFromChar(promotionMatch.Groups["promotionPiece"].Value[0]);
                if (!md.IsCapture)
                {
                    md.DestinationFile = (ushort)(match.Groups["pawnFile"].Value[0] - 'a');
                }
                return md;
            }
            if ((castleMatch = Regex.Match(move, RegExCastle)).Success)
            {
                md.Piece = Piece.King;
                md.MoveType = MoveType.Castle;
                md.SourceFile = 4;
                md.DestinationRank = md.SourceRank = (ushort)(color == Color.Black ? 7 : 0);
                md.DestinationFile = castleMatch.Groups[RegExCastleLongGroup].Success ? (ushort?) 2 : (ushort?) 6;
                return md;
            }

            if (match.Groups["pawnFile"].Success)
            {
                md.Piece = Piece.Pawn;
               
                if (md.IsCapture)
                {
                    md.SourceFile = (ushort)(match.Groups["pawnFile"].Value[0] - 'a');
                    if (color == Color.Black)
                    {
                        Debug.Assert(md.DestinationRank != null, "md.DestinationRank != null");
                        md.SourceRank = (ushort)(md.DestinationRank.Value + 1);
                    }
                    else
                    {
                        Debug.Assert(md.DestinationRank != null, "md.DestinationRank != null");
                        md.SourceRank = (ushort)(md.DestinationRank.Value - 1);
                    }
                }
                else
                {
                    md.DestinationFile = match.Groups["pawnFile"].Success ? (ushort)(match.Groups["pawnFile"].Value[0] - 'a') : (ushort?)null;
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
