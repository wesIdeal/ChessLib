using System;
using ChessLib.MagicBitboard.Bitwise;

namespace ChessLib.MagicBitboard.MovingPieces
{
    internal class Rook : SlidingPiece
    {
        public Rook()
        {
            base.Initialize();
        }

        protected override Func<ulong, ulong>[] MoveShifts => new Func<ulong, ulong>[]
        {

            MovingPieceService.ShiftN,
            MovingPieceService.ShiftS,
            MovingPieceService.ShiftW,
            MovingPieceService.ShiftE,
        };

        protected override Func<ulong, ulong>[] AttackShifts => new Func<ulong, ulong>[]
        {
            MovingPieceService.AttackN,
            MovingPieceService.AttackS,
            MovingPieceService.AttackW,
            MovingPieceService.AttackE,
        };


    }
}