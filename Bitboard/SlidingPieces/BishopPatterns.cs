using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MagicBitboard.SlidingPieces
{
    public class BishopPatterns : MovePatternStorage
    {
        Bitboard bb = new Bitboard();
        public BishopPatterns()
        {

            Initialize(bb.BishopAttackMask, new BishopMovesInitializer());

        }

    }
}
