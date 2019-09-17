using System;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;

namespace ChessLib.Data
{
    public class Game<TMove> : MoveTraversalService
        where TMove : MoveExt, IEquatable<TMove>
    {
        public Game() : base(FENHelpers.FENInitial)
        {
            TagSection = new Tags(OnFenChanged);
            TagSection.SetFen(FENHelpers.FENInitial);
        }

        public Game(string fen) : base(fen)
        {
            TagSection = new Tags(OnFenChanged);
            TagSection.SetFen(fen);
        }

        public Tags TagSection { get; set; }

        private void OnFenChanged(string fen)
        {
            InitialFen = fen;
        }

        public void SetFEN(string fen)
        {
            OnFenChanged(fen);
        }
    }
}