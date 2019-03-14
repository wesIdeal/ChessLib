using MagicBitboard.Enums;
using MagicBitboard.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace MagicBitboard
{
    using System.Runtime.Serialization;
    using Move = UInt16;
    public enum MoveType
    {
        Normal = 0, Promotion = 1, EnPassent = 2, Castle = 3
    }
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


    public static class MoveHelpers
    {
        private static string[] castling = new[] { "O-O", "O-O-O" };
        public static Move GenerateMove(ushort fromIdx, ushort toIdx, MoveType moveType = MoveType.Normal, PromotionPiece promotionPiece = 0)
        {
            var mt = (ushort)moveType << 14;
            var pp = (ushort)promotionPiece << 12;
            var origin = fromIdx << 6;
            var dest = toIdx << 0;
            return (Move)(mt | pp | origin | dest);
        }



        public static Move GenerateMoveFromText(string moveText, Color moveColor)
        {
            moveText = moveText.TrimEnd('#', '?', '!', '+').Trim();
            MoveType moveType = MoveType.Normal;
            if (castling.Contains(moveText.ToLowerInvariant()))
            {
                moveType = MoveType.Castle;
            }
            else if (moveText.Contains("="))
            {
                var promotionPieces = moveText.Split('=');
                ushort dest = (ushort)BoardHelpers.SquareTextToIndex(promotionPieces[0]).Value;
                var promotionPiece = GetPromotionPieceFromChar(promotionPieces[1][0]);
                var source = (ushort)(moveColor == Color.White ? dest - 8 : dest + 8);
                var proposedMove = GenerateMove(source, dest, MoveType.Promotion, promotionPiece);
                return proposedMove;
            }

            return 0;
        }
        
        public static PromotionPiece GetPromotionPieceFromChar(char p)
        {
            switch (char.ToUpper(p))
            {
                case 'B': return PromotionPiece.Bishop;
                case 'N': return PromotionPiece.Knight;
                case 'Q': return PromotionPiece.Queen;
                case 'R': return PromotionPiece.Rook;
                default: throw new Exception("Char / Piece not found in switch cases.");
            }
        }

        public static ushort Destination(this Move move)
        {
            return (ushort)(move & 63);
        }
        public static ushort Source(this Move move)
        {
            return (ushort)((move >> 6) & 63);
        }
        public static PromotionPiece GetPiecePromoted(this Move move)
        {
            return (PromotionPiece)((move >> 12) & 3);
        }
        public static MoveType GetMoveType(this Move move)
        {
            return (MoveType)((move >> 14) & 3);
        }
        public static char GetCharFromPromotionPiece(PromotionPiece p)
        {
            switch (p)
            {
                case PromotionPiece.Bishop: return 'B';
                case PromotionPiece.Knight: return 'N';
                case PromotionPiece.Queen: return 'Q';
                case PromotionPiece.Rook: return 'R';
                default: throw new Exception("Promotion Piece not found in switch cases.");
            }
        }
    }

    [Serializable]
    public class MoveException : Exception
    {
        public MoveException()
        {
        }

        public MoveException(string message) : base(message)
        {
        }

        public MoveException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MoveException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
