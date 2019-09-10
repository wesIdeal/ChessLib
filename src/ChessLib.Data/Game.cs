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

        public Game() : base(FENHelpers.FENInitial)
        {
            TagSection = new Tags(OnFenChanged);
            TagSection.SetFen(FENHelpers.FENInitial);
        }

        private void OnFenChanged(string fen)
        {
            InitialFen = fen;
        }

        public Game(string fen) : base(fen)
        {
            TagSection = new Tags(OnFenChanged);
            TagSection.SetFen(fen);
        }

        public void SetFEN(string fen)
        {
            OnFenChanged(fen);
        }

    }

}
