using ChessLib.Data.Helpers;
using ChessLib.Data.PieceMobility;

namespace ChessLib.Data
{
    public class RookPatterns : MovePatternStorage
    {
       
        public RookPatterns()
        {
            Initialize(PieceAttackPatternHelper.RookAttackMask, new RookMovesInitializer());
        }
    }
}
