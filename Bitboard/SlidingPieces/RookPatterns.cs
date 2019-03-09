namespace MagicBitboard.SlidingPieces
{
    public class RookPatterns : MovePatternStorage
    {
        Bitboard bb = new Bitboard();
        public RookPatterns()
        {
            Initialize(bb.RookAttackMask, new RookMovesInitializer());
        }
    }
}
