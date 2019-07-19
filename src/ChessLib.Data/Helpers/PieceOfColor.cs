using ChessLib.Data.Types.Enums;

namespace ChessLib.Data.Helpers
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
    }
}