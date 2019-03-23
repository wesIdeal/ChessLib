using MagicBitboard.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MagicBitboard.Helpers
{
    public static class PieceHelpers
    {
        public static Piece GetPiece(string p)
        {
            if (p.Length > 1) throw new Exception("Piece should be a single char.");
            return GetPiece(p[0]);
        }

        public static Piece GetPiece(char p)
        {
            switch (Char.ToLower(p))
            {
                case 'k':
                    return Piece.King;
                case 'n':
                    return Piece.Knight;
                case 'b':
                    return Piece.Bishop;
                case 'p':
                    return Piece.Pawn;
                case 'r':
                    return Piece.Rook;
                case 'q':
                    return Piece.Queen;
                default: throw new Exception("Cannot determine piece for " + p.ToString());
            }
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

        public static char GetCharRepresentation(Piece p, Color c)
        {
            var pChar = GetCharRepresentation(p);
            if (c == Color.Black)
            {
                return Char.ToLower(pChar);
            }
            return char.ToUpper(pChar);

        }

        public static char GetCharRepresentation(Piece p)
        {
            switch (p)
            {
                case Piece.Bishop: return 'b';
                case Piece.Knight: return 'n';
                case Piece.Rook: return 'r';
                case Piece.Queen: return 'q';
                case Piece.King: return 'k';
                case Piece.Pawn: return 'p';
                default: throw new Exception("Piece's char representation not found.");
            }
        }

        public static PieceOfColor GetPieceOfColor(char p)
        {
            var poc = new PieceOfColor();
            if (Char.IsUpper(p)) poc.Color = Color.White;
            else poc.Color = Color.Black;
            poc.Piece = GetPiece(p);
            return poc;
        }

        public struct PieceOfColor
        {
            public Color Color { get; set; }
            public Piece Piece { get; set; }
        }
    }
}
