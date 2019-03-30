using ChessLib.Data.Helpers;
using ChessLib.Data.PieceMobility;

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
