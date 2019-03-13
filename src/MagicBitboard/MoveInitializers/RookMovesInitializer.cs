using MagicBitboard.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MagicBitboard
{
    public class RookMovesInitializer : MoveInitializer
    {
        public RookMovesInitializer() : base(SlidingPieceDirectionContants.RookDirections) { }
    }
}
