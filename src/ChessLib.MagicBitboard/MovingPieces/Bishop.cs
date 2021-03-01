using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using ChessLib.Data.Helpers;
using ChessLib.Data.Types.Enums;
using ChessLib.MagicBitboard.Bitwise;
using File = System.IO.File;

namespace ChessLib.MagicBitboard.MovingPieces
{
    internal class Bishop : SlidingPiece
    {
        public Bishop()
        {
            base.Initialize();
        }

        protected override Func<ulong, ulong>[] DirectionalMethods => new Func<ulong, ulong>[]
        {
            MovingPieceService.ShiftNE,
            MovingPieceService.ShiftNW,
            MovingPieceService.ShiftSE,
            MovingPieceService.ShiftSW
        };
        protected override Func<ulong, ulong>[] AttackDirections=> new Func<ulong, ulong>[]
        {
            MovingPieceService.ShiftNE,
            MovingPieceService.ShiftNW,
            MovingPieceService.ShiftSE,
            MovingPieceService.ShiftSW
        };


    }
}