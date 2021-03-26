using System;
using ChessLib.Core.MagicBitboard.Bitwise;
using File = System.IO.File;

namespace ChessLib.Core.MagicBitboard.MovingPieces
{
    internal class Bishop : SlidingPiece
    {
        public Bishop()
        {
            base.Initialize();
        }

        protected override Func<ulong, ulong>[] MoveShifts => new Func<ulong, ulong>[]
        {
            MovingPieceService.ShiftNE,
            MovingPieceService.ShiftNW,
            MovingPieceService.ShiftSE,
            MovingPieceService.ShiftSW
        };
        protected override Func<ulong, ulong>[] AttackShifts=> new Func<ulong, ulong>[]
        {
            MovingPieceService.ShiftNE,
            MovingPieceService.ShiftNW,
            MovingPieceService.ShiftSE,
            MovingPieceService.ShiftSW
        };


    }
}