using ChessLib.Data.Types.Enums;

namespace ChessLib.Data.PieceMobility
{
    internal class BishopMovesInitializer : MoveInitializer
    {
        public BishopMovesInitializer() : base(SlidingPieceDirectionConstants.BishopDirections)
        {
        }
    }
}
