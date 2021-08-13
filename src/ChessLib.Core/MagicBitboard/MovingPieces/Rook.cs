using System;
using System.Runtime.CompilerServices;
using ChessLib.Core.MagicBitboard.Bitwise;

[assembly: InternalsVisibleTo("ChessLib.Core.Tests.MagicBitboard.MovingPieces")]

namespace ChessLib.Core.MagicBitboard.MovingPieces
{
    internal class Rook : SlidingPiece
    {
        protected override Func<ulong, ulong>[] MoveShifts => new Func<ulong, ulong>[]
        {
            MovingPieceService.ShiftN,
            MovingPieceService.ShiftS,
            MovingPieceService.ShiftW,
            MovingPieceService.ShiftE
        };

        protected override Func<ulong, ulong>[] AttackShifts => new Func<ulong, ulong>[]
        {
            MovingPieceService.AttackN,
            MovingPieceService.AttackS,
            MovingPieceService.AttackW,
            MovingPieceService.AttackE
        };

        public Rook()
        {
            base.Initialize();
        }
    }
}