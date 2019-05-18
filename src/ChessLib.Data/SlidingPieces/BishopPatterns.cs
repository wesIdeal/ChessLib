using ChessLib.Data.Helpers;
using ChessLib.Data.PieceMobility;

namespace ChessLib.Data
{
    public class BishopPatterns : MovePatternStorage
    {
        public BishopPatterns()
        {
            Initialize(PieceAttackPatternHelper.BishopAttackMask, new BishopMovesInitializer());
        }
    }
}
