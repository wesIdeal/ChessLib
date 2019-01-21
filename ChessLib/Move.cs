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
    }
}
