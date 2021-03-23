using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ChessLib.MagicBitboard.Bitwise;
using ChessLib.Types.Enums;

namespace ChessLib.MagicBitboard.MovingPieces
{
    internal class Knight : MovingPiece
    {
        public override ulong GetPseudoLegalMoves(ushort square, Color playerColor, ulong occupancy)
        {
            return GetMoves(square);
        }

        public override void Initialize()
        {
            throw new NotImplementedException();
        }

        internal ulong GetMoves(ushort square)
        {
            ulong squareValue = MovingPieceService.GetBoardValueOfIndex(square);
            ulong squares = 0;
            foreach (var shift in _knightDirectionMethods)
            {
                squares |= shift(squareValue);
            }
            return squares;
        }

        private readonly Func<ulong, ulong>[] _knightDirectionMethods =
        {
            MovingPieceService.ShiftNNW,
            MovingPieceService.ShiftNNE,
            MovingPieceService.ShiftENE,
            MovingPieceService.ShiftESE,
            MovingPieceService.ShiftSSE,
            MovingPieceService.ShiftSSW,
            MovingPieceService.ShiftWSW,
            MovingPieceService.ShiftWNW
        };
    }
}
