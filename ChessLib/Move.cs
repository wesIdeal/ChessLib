using System.Collections.Generic;
using System.Text;

namespace ChessLib
{
    public struct PieceOfColor
    {
        public Piece Piece { get; set; }
        public Color Color { get; set; }
    }
    public enum Piece { Pawn, Knight, Bishop, Rook, Queen, King }
    public enum Color { White, Black }
    public enum File { a, b, c, d, e, f, g, h }
    public class BoardProperties
    {
        public bool CanWhiteCastleQueenSide { get; set; }
        public bool CanBlackCastleQueenSide { get; set; }
        public bool CanWhiteCastleKingSide { get; set; }
        public bool CanBlackCastleKingSide { get; set; }
        public Square EnPassentSquare { get; set; }
        public int HalfmoveClock { get; set; }
        public int FullMoveNumber { get; set; }
        public Color ActiveColor { get; set; }
    }

    public class Move
    {
        public Move()
        {
        }

        public Move(Square origin, Square destination, bool isCapture, Piece piece)
        {
            Origin = origin;
            Destination = destination;
            IsCapture = isCapture;
            Piece = piece;
        }

        public Square Origin { get; set; }
        public Square Destination { get; set; }
        public bool IsCapture { get; set; }
        public Piece Piece { get; set; }

        public override bool Equals(object obj)
        {
            var move = obj as Move;
            return move != null &&
                   Origin.Equals(move.Origin) &&
                   Destination.Equals(move.Destination) &&
                   IsCapture == move.IsCapture &&
                   Piece == move.Piece;
        }

        public override int GetHashCode()
        {
            var hashCode = -823416892;
            hashCode = hashCode * -1521134295 + EqualityComparer<Square>.Default.GetHashCode(Origin);
            hashCode = hashCode * -1521134295 + EqualityComparer<Square>.Default.GetHashCode(Destination);
            hashCode = hashCode * -1521134295 + IsCapture.GetHashCode();
            hashCode = hashCode * -1521134295 + Piece.GetHashCode();
            return hashCode;
        }
    }
}
