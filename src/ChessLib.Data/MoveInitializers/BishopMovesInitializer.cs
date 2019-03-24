using ChessLib.Data.Types;

namespace ChessLib.Data.MoveInitializers
{
    public class BishopMovesInitializer : MoveInitializer
    {
        public BishopMovesInitializer() : base(SlidingPieceDirectionContants.BishopDirections)
        {
        }
    }
}
