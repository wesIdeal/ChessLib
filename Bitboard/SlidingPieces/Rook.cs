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

            Initialize(bb.BishopAttackMask, new BishopMovesGenerator());

        }

    }
    public class RookPatterns : MovePatternStorage
    {
        Bitboard bb = new Bitboard();
        public RookPatterns()
        {

            Initialize(bb.RookAttackMask, new RookMovesGenerator());
            var occupancy = (ulong)0x101000100000060;
            var movesForRookOna1 = GetLegalMoves(0, occupancy);
            Debug.WriteLine("OCCUPANCY\r\n" + occupancy.GetDisplayBits());
            Debug.WriteLine("MOVES\r\n" + movesForRookOna1.GetDisplayBits());
        }
        
    }
}
