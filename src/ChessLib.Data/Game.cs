using ChessLib.Data.Types.Interfaces;

namespace ChessLib.Data
{
    public class Game<TMove> where TMove : IMove
    {
        public Tags TagSection;
        public MoveTree<TMove> MoveSection;

        public Game()
        {
            TagSection = new Tags();
            MoveSection = new MoveTree<TMove>(null);
        }

        public Game(string fen) : this()
        {
            TagSection.SetFen(fen);
        }
    }
}
