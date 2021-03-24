using System;
using ChessLib.Core;

namespace ChessLib.Data
{
    public class MoveMadeEventArgs : EventArgs
    {
        public MoveMadeEventArgs(Move[] previousMoves, string fen)
        {
            CurrentFen = fen;
            PreviousMoves = previousMoves;
        }

        public string CurrentFen { get; }
        public Move[] PreviousMoves { get; }
    }

}
