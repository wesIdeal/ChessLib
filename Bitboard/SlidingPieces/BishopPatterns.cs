using MagicBitboard.Helpers;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MagicBitboard.SlidingPieces
{
    public class BishopPatterns : MovePatternStorage
    {
        public BishopPatterns()
        {
            Initialize(PieceAttackPatternHelper.BishopAttackMask, new BishopMovesInitializer());
        }
    }
}
