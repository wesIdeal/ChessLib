using ChessLib.Data.Helpers;
using ChessLib.Data.PieceMobility;

namespace ChessLib.Data
{
    internal class BishopPatterns : MovePatternStorage
    {
        public BishopPatterns()
        {
            Initialize(PieceAttackPatternHelper.BishopAttackMask, new BishopMovesInitializer());
        }
    }
}
