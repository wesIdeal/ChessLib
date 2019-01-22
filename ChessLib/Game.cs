using System;

namespace ChessLib
{
    public class Game : ChessBoard
    {
        public Game()
        {
        }

        public Game(string fen) : base(fen)
        {
        }
    }
}
