using MagicBitboard.Enums;
using System;

namespace MagicBitboard
{
    public class PieceOfColor : IEquatable<PieceOfColor>
    {
        public Color Color { get; set; }
        public Piece Piece { get; set; }
        public static PieceOfColor GetPieceOfColor(char p)
        {
            var poc = new PieceOfColor();
            if (Char.IsUpper(p)) poc.Color = Color.White;
            else poc.Color = Color.Black;

            switch (Char.ToLower(p))
            {
                case 'k':
                    poc.Piece = Piece.King;
                    break;
                case 'n':
                    poc.Piece = Piece.Knight;
                    break;
                case 'b':
                    poc.Piece = Piece.Bishop;
                    break;
                case 'p':
                    poc.Piece = Piece.Pawn;
                    break;
                case 'r':
                    poc.Piece = Piece.Rook;
                    break;
                case 'q':
                    poc.Piece = Piece.Queen;
                    break;
                default: throw new Exception("Cannot determine piece for " + p.ToString());
            }
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
            return MoveHelpers.HtmlPieceRepresentations[color][piece];
        }

        public static char GetCharRepresentation(PieceOfColor poc)
        {
            return GetCharRepresentation(poc.Piece, poc.Color);
        }
        public static char GetCharRepresentation(Piece p, Color c)
        {
            ChangeCharCaseForColor changeCase = char.ToUpper;
            if (c == Color.Black)
            {
                changeCase = char.ToLower;
            }
            switch (p)
            {
                case Piece.Bishop: return changeCase('b');
                case Piece.Knight: return changeCase('n');
                case Piece.Rook: return changeCase('r');
                case Piece.Queen: return changeCase('q');
                case Piece.King: return changeCase('k');
                case Piece.Pawn: return changeCase('p');
                default: throw new Exception("Piece's char representation not found.");
            }
        }
    }
}
