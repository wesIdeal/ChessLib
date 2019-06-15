using ChessLib.Data.MoveRepresentation;
using ChessLib.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessLib.Data
{
    public class Game<TMove> where TMove : IMove
    {
        public Tags TagSection = new Tags();
        public MoveTree<TMove> MoveSection = new MoveTree<TMove>(null);
    }
}
