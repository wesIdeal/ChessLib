using ChessLib.Data.PieceMobility;

namespace ChessLib.Data.Magic.Init
{
    internal class RookPatterns : MovePatternStorage
    {
        public RookPatterns()
        {
            Initialize(PieceAttackPatterns.Instance.RookAttackMask, new RookMovesInitializer());
        }
    }
}
