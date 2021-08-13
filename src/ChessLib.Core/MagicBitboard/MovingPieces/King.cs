using System;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Types.Enums;

namespace ChessLib.Core.MagicBitboard.MovingPieces
{
    internal class King : MovingPiece
    {
        private readonly Func<ulong, ulong>[] _kingDirectionMethods =
        {
            MovingPieceService.ShiftN,
            MovingPieceService.ShiftS,
            MovingPieceService.ShiftW,
            MovingPieceService.ShiftE,
            MovingPieceService.ShiftNE,
            MovingPieceService.ShiftNW,
            MovingPieceService.ShiftSW,
            MovingPieceService.ShiftSE
        };

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
            var squareValue = MovingPieceService.GetBoardValueOfIndex(square);
            ulong squares = 0;
            foreach (var shift in _kingDirectionMethods)
            {
                squares |= shift(squareValue);
            }

            return squares;
        }

        private bool IsKingSquare(ushort squareIndex, Color color)
        {
            switch (color)
            {
                case Color.White:
                    return squareIndex == BoardConstants.WhiteKingSquare;
                case Color.Black:
                    return squareIndex == BoardConstants.BlackKingSquare;
                default:
                    throw new ArgumentOutOfRangeException(nameof(color), color, null);
            }
        }
    }
}