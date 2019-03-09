using MagicBitboard.Enums;

namespace MagicBitboard
{
    public class QueenMovesInitializer : MoveInitializer
    {
        public QueenMovesInitializer() : base(SlidingPieceDirectionContants.QueenDirections)
        {
        }
    }
}
