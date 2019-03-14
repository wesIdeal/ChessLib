using MagicBitboard.Enums;
using MagicBitboard.Helpers;
using System;

namespace MagicBitboard
{
    public class PieceOfColor : IEquatable<PieceOfColor>
    {
        public Color Color { get; set; }
        public Piece Piece { get; set; }

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

        public static PieceOfColor GetPieceOfColor(char p)
        {
            var poc = new PieceOfColor();
            if (Char.IsUpper(p)) poc.Color = Color.White;
            else poc.Color = Color.Black;
            poc.Piece = GetPiece(p);
            return poc;
        }

        public bool Equals(PieceOfColor other)
        {
            return this.Color == other.Color && this.Piece == other.Piece;
        }

        private delegate char ChangeCharCaseForColor(char c);

        public char GetCharRepresentation()
        {
            return GetCharRepresentation(this);
        }

        public static string GetHtmlRepresentation(char? fenChar)
        {
            if (!fenChar.HasValue) return "&nbsp;";
            var color = char.IsUpper(fenChar.Value) ? Color.White : Color.Black;
            Piece piece;
            switch (char.ToLower(fenChar.Value))
            {
                case 'p': piece = Piece.Pawn; break;
                case 'n': piece = Piece.Knight; break;
                case 'b': piece = Piece.Bishop; break;
                case 'r': piece = Piece.Rook; break;
                case 'q': piece = Piece.Queen; break;
                case 'k': piece = Piece.King; break;
                default: throw new Exception("Unexpected FEN char passed into method GetHtmlRepresentation()");
            }
            return BoardHelpers.HtmlPieceRepresentations[color][piece];
        }

        public static char GetCharRepresentation(PieceOfColor poc)
        {
            return GetCharRepresentation(poc.Piece, poc.Color);
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
    }
}
