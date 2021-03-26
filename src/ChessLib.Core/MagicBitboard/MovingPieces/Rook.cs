using System;
using ChessLib.Core.MagicBitboard.Bitwise;

namespace ChessLib.Core.MagicBitboard.MovingPieces
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