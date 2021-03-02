using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ChessLib.Data.Types.Enums;
using ChessLib.MagicBitboard.Bitwise;


namespace ChessLib.MagicBitboard.MovingPieces
{
   internal class King: MovingPiece
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
            Parallel.ForEach(_kingDirectionMethods, directionMethod =>
            {
                squares |= directionMethod(squareValue);
            });
            return squares;
        }

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
    }
}
