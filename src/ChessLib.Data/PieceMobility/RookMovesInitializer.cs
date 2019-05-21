using ChessLib.Types.Enums;

namespace ChessLib.Data.PieceMobility
{
    public class RookMovesInitializer : MoveInitializer
    {
        public RookMovesInitializer() : base(SlidingPieceDirectionConstants.RookDirections) { }
    }
}
