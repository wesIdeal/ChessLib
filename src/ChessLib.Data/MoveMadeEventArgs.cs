using ChessLib.Data.MoveRepresentation;
using System;

namespace ChessLib.Data
{
    public class MoveMadeEventArgs : EventArgs
    {
        public MoveMadeEventArgs(MoveExt[] previousMoves, string fen)
        {
            CurrentFen = fen;
            PreviousMoves = previousMoves;
        }

        public string CurrentFen { get; }
        public MoveExt[] PreviousMoves { get; }
    }

}
