using ChessLib.Data.Types;

namespace ChessLib.Data.PieceMobility
{
    public class BishopMovesInitializer : MoveInitializer
    {
        public BishopMovesInitializer() : base(SlidingPieceDirectionContants.BishopDirections)
        {
        }
    }
}
