using ChessLib.Data.Types;

namespace ChessLib.Data.MoveInitializers
{
    public class RookMovesInitializer : MoveInitializer
    {
        public RookMovesInitializer() : base(SlidingPieceDirectionContants.RookDirections) { }
    }
}
