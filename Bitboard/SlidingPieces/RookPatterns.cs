using MagicBitboard.Helpers;

namespace MagicBitboard.SlidingPieces
{
    public class RookPatterns : MovePatternStorage
    {
       
        public RookPatterns()
        {
            Initialize(PieceAttackPatternHelper.RookAttackMask, new RookMovesInitializer());
        }
    }
}
