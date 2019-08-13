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

        public Game() :base(FENHelpers.FENInitial)
        {
            TagSection = new Tags();
            TagSection.FENChanged += OnFenChanged;
            TagSection.SetFen(FENHelpers.FENInitial);
        }

        private void OnFenChanged(object sender, string fen)
        {
            MainMoveTree = new MoveTree(null, fen);

        }

        public Game(string fen) : base(fen)
        {
            TagSection = new Tags();
            TagSection.FENChanged += OnFenChanged;
            TagSection.SetFen(fen);
        }

    }

}
