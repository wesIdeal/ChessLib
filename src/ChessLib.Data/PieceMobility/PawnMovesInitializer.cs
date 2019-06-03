using ChessLib.Types.Enums;

namespace ChessLib.Data.PieceMobility
{
    internal class PawnMovesInitializer : MoveInitializer
    {
        public PawnMovesInitializer(Color pawnColor) : base(SlidingPieceDirectionConstants.WhitePawnDirections)
        {

        }
    }
}
