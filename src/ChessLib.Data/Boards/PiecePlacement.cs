using ChessLib.Data.Helpers;
using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;

namespace ChessLib.Data.Boards
{
    public class PiecePlacement : IPiecePlacement
    {
        private ulong[][] _pieces = new ulong[2][];

        public PiecePlacement(ulong[][] pieces)
        {
            _pieces = pieces;
        }

        public ulong this[Color c, Piece p] => _pieces[(int)c][(int)p];

        public ulong BlackOccupancy => ColorOccupancy(Color.Black);
        public ulong WhiteOccupancy => ColorOccupancy(Color.Black);
        public ulong ColorOccupancy(Color c) => _pieces.Occupancy(c);

        public ulong PieceOfColorOccupancy(PieceOfColor poc) => PieceOfColorOccupancy(poc.Color, poc.Piece);

        public ulong PieceOfColorOccupancy(Color color, Piece piece) => _pieces[(int)color][(int)piece];

        public ulong[][] GetBoardPostMove(in IMoveExt move) => BoardHelpers.GetBoardPostMove(_pieces, move);

        public void ApplyMove(in IMoveExt move) => _pieces = GetBoardPostMove(move);

        public ulong BlackPiecesOfType(Piece p) =>
            PieceOfColorOccupancy(Color.Black, p);

        public ulong WhitePiecesOfType(Piece p) =>
            PieceOfColorOccupancy(Color.White, p);

        public ulong[][] GetPiecePlacementArray()
        {
            return _pieces;
        }
    }
}
