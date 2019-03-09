namespace MagicBitboard.SlidingPieces
{
    public class QueenPatterns : MovePatternStorage
    {
        Bitboard bb = new Bitboard();
        public QueenPatterns()
        {
            Initialize(bb.QueenAttackMask, new RookMovesInitializer());
        }
    }
}
