using ChessLib.Data.Helpers;
using ChessLib.Data.MoveInitializers;

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
