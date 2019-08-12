using ChessLib.Data.Boards;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types.Interfaces;
using System;
using System.Diagnostics;

namespace ChessLib.Data
{
    public class Game<TMove> : MoveTraversalService
        where TMove : MoveExt, IEquatable<TMove>
    {
        public Tags TagSection;
        public MoveTree<MoveStorage> MoveSection => base.MoveTree;

        public Game()
        {
            TagSection = new Tags();
            TagSection.FENChanged += OnFenChanged;
            TagSection.SetFen(FENHelpers.FENInitial);
        }

        private void OnFenChanged(object sender, string fen)
        {
            this.InitialFEN = fen;

        }

        public Game(string fen)
        {
            TagSection = new Tags();
            TagSection.FENChanged += OnFenChanged;
            TagSection.SetFen(fen);
        }

        public new MoveNode<MoveStorage> ExitVariation()
        {
            var pgnFormatter = new PGNFormatter<TMove>(new PGNFormatterOptions() { IndentVariations = true, NewlineEachMove = true });
            Debug.WriteLine(pgnFormatter.BuildPGN(this));
            return base.ExitVariation();
        }

        protected IMoveTraversalService MoveSvc { get; private set; }




    }

}
