using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types.Interfaces;
using System;

namespace ChessLib.Data
{
    public class MoveMadeEventArgs : EventArgs
    {
        public MoveMadeEventArgs(MoveExt moveMade, IBoard currentBoard, MoveExt[] nextMoves, ushort[] squaresUpdated)
        {
            NextMoves = nextMoves;
            SquaresUpdated = squaresUpdated;
        }

        public MoveExt[] NextMoves { get; private set; }
        public ushort[] SquaresUpdated { get; private set; }
    }

}
