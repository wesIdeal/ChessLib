using System;
using System.Diagnostics;

namespace ChessLib.Data
{
    /// <summary>
    /// Holds 64 move/attack boards. Usually used to represent a piece's possible attacks or moves from each square.
    /// </summary>
    internal class MoveBoard
    {
        public ulong[] Board;

        public MoveBoard() { Board = new ulong[64]; }

        public MoveBoard(ulong[] initValues) : this()
        {
            Debug.Assert(initValues.Length == 64);
            Array.Copy(initValues, Board, 64);
        }

        public ulong this[int k]
        {
            get => Board[k];
            set => Board[k] = value;
        }

        public ulong this[int rank, int file]
        {
            get => Board[rank * 8 + file];
            set => Board[rank * 8 + file] = value;
        }


    }
}