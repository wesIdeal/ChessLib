using System;
using System.Collections.Generic;
using System.Text;
using ChessLib.Data.Helpers;
using ChessLib.Types;
using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;

namespace ChessLib.Data.Boards
{
    public class PiecePlacement
    {
        protected ulong[][] _pieces = new ulong[2][];
        public ulong BlackOccupancy => ColorOccuancy(Color.Black);
        public ulong WhiteOccupancy => ColorOccuancy(Color.Black);

        public ulong PieceOfColorOccupancy(PieceOfColor poc) => PieceOfColorOccupancy(poc.Color, poc.Piece);

        public ulong PieceOfColorOccupancy(Color color, Piece piece) => _pieces.Occupancy(color, piece);

        public ulong[][] GetBoardPostMove(in IMoveExt move) => BoardHelpers.GetBoardPostMove(_pieces, move);

        public void ApplyMove(in IMoveExt move) => _pieces = GetBoardPostMove(move);

        private ulong ColorOccuancy(Color c) => _pieces.Occupancy(c);

        public ulong BlackPiecesOfType(Piece p) =>
             _pieces[(int)Color.Black][(int)p];
        public ulong WhitePiecesOfType(Piece p) =>
            _pieces[(int)Color.White][(int)p];

    }
}
