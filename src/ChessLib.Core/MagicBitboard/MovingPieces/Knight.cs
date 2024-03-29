﻿using System;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Types.Enums;

namespace ChessLib.Core.MagicBitboard.MovingPieces
{
    internal class Knight : MovingPiece
    {
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

        public override ulong GetPseudoLegalMoves(ushort square, Color playerColor, ulong occupancy)
        {
            return GetMoves(square);
        }

        internal ulong GetMoves(ushort square)
        {
            var squareValue = MovingPieceService.GetBoardValueOfIndex(square);
            ulong squares = 0;
            foreach (var shift in _knightDirectionMethods)
            {
                squares |= shift(squareValue);
            }

            return squares;
        }
    }
}