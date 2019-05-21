using ChessLib.Types.Enums;

namespace ChessLib.Data.PieceMobility
{
    public class BishopMovesInitializer : MoveInitializer
    {
        public BishopMovesInitializer() : base(SlidingPieceDirectionConstants.BishopDirections)
        {
        }
    }
}
