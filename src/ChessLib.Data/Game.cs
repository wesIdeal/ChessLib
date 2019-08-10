using ChessLib.Data.Boards;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types.Interfaces;
using System;

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

    public class UIGame : Game<MoveStorage>

    {
        public UIGame(Game<MoveStorage> game)
           : base(game.TagSection.FENStart)
        {
            MoveSvc = new MoveTraversalService(game.TagSection.FENStart, ref MoveSection);
        }

        public string CurrentFEN => MoveSvc.CurrentFEN;

        public event EventHandler<MoveMadeEventArgs> MoveMade
        {
            add { MoveSvc.MoveMade += value; }
            remove { MoveSvc.MoveMade -= value; }
        }

        protected IMoveTraversalService MoveSvc { get; private set; }

        public MoveStorage[] GetNextMoves() => MoveSvc.GetNextMoves();

        public IBoard TraverseForward(MoveStorage move) => MoveSvc.TraverseForward(move);

        public IBoard TraverseForward() => MoveSvc.TraverseForward();

        public IBoard TraverseBackward() => MoveSvc.TraverseBackward();

    }

}
