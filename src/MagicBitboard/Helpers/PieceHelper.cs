using System;
using System.Collections.Generic;
using System.Text;

namespace MagicBitboard.Helpers
{
    public static class PieceHelper
    {
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
    }
}
