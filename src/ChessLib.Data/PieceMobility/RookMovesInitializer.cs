using ChessLib.Data.Types;

namespace ChessLib.Data.PieceMobility
{
    public class RookMovesInitializer : MoveInitializer
    {
        public RookMovesInitializer() : base(SlidingPieceDirectionContants.RookDirections) { }
    }
}
