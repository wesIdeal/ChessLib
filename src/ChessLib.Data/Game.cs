using ChessLib.Data.Types.Interfaces;

namespace ChessLib.Data
{
    public class Game<TMove> where TMove : IMove
    {
        public Tags TagSection = new Tags();
        public MoveTree<TMove> MoveSection = new MoveTree<TMove>(null);
    }
}
