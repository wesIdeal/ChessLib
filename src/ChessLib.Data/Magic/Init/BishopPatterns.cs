using ChessLib.Data.Magic;
using ChessLib.Data.PieceMobility;

namespace ChessLib.Data.Magic.Init
{
    internal class BishopPatterns : MovePatternStorage
    {
        public BishopPatterns()
        {
            Initialize(PieceAttackPatterns.Instance.BishopAttackMask, new BishopMovesInitializer());
        }
    }
}
