using System;
using ChessLib.Core.MagicBitboard.Bitwise;

namespace ChessLib.Core.MagicBitboard.MovingPieces
{
    internal class Bishop : SlidingPiece
    {
        protected override Func<ulong, ulong>[] MoveShifts => new Func<ulong, ulong>[]
        {
            MovingPieceService.ShiftNE,
            MovingPieceService.ShiftNW,
            MovingPieceService.ShiftSE,
            MovingPieceService.ShiftSW
        };

        protected override Func<ulong, ulong>[] AttackShifts => new Func<ulong, ulong>[]
        {
            MovingPieceService.ShiftNE,
            MovingPieceService.ShiftNW,
            MovingPieceService.ShiftSE,
            MovingPieceService.ShiftSW
        };

        public Bishop()
        {
            base.Initialize();
        }
    }
}