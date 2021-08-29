using ChessLib.Core.Types.Enums;

namespace ChessLib.Core.Types
{
    public readonly struct PieceOfColor
    {
        public Color Color { get;  }
        public Piece Piece { get;  }

        public PieceOfColor(Piece piece, Color color)
        {
            Piece = piece;
            Color = color;
        }
        public override bool Equals(object obj)
        {
            if (!(obj is PieceOfColor actual))
            {
                return false;
            }

            return Color == actual.Color &&
                   Piece == actual.Piece;
        }

        public override int GetHashCode()
        {
            return (int) Color ^ (int) Piece;
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
            var nPiece = (int) p * 2;
            if (c == Color.White)
            {
                nPiece += 1;
            }

            return (ulong) nPiece;
        }

        public ulong GetPieceHashValue()
        {
            return GetPieceHashValue(Color, Piece);
        }
    }
}