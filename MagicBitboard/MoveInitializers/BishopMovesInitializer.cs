using MagicBitboard.Enums;

namespace MagicBitboard
{
    public class BishopMovesInitializer : MoveInitializer
    {
        public BishopMovesInitializer() : base(SlidingPieceDirectionContants.BishopDirections)
        {
        }
    }
}
