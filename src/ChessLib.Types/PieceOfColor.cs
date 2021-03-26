using ChessLib.Core.Helpers;
using ChessLib.Core.Types.Enums;

namespace ChessLib.Core
{
    public struct PieceOfColor
    {
        public Color Color { get; set; }
        public Piece Piece { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is PieceOfColor))
            {
                return false;
            }

            var actual = (PieceOfColor)obj;
            return Color == actual.Color &&
                   Piece == actual.Piece;
        }

        public override int GetHashCode()
        {
            return Color.ToInt() ^ Piece.ToInt();
        }

        public override string ToString()
        {
            return $"{Color} {Piece}";
        }

        public static bool operator ==(PieceOfColor left, PieceOfColor right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PieceOfColor left, PieceOfColor right)
        {
            return !(left == right);
        }

        public static ulong GetPieceHashValue(Color c, Piece p)
        {
            var nPiece = (int)p * 2;
            if (c == Color.White)
            {
                nPiece += 1;
            }

            return (ulong)nPiece;
        }

        public ulong GetPieceHashValue()
        {
            return GetPieceHashValue(this.Color, this.Piece);
        }
    }
}