using ChessLib.Data.Types;

namespace ChessLib.Data.MoveInitializers
{
    public class QueenMovesInitializer : MoveInitializer
    {
        public QueenMovesInitializer() : base(SlidingPieceDirectionContants.QueenDirections)
        {
        }
    }
}
