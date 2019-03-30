using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace ChessLib.Data
{
    /// <summary>
    /// Holds 64 move/attack boards. Usually used to represent a piece's possible attacks or moves from each square.
    /// </summary>
    public class Board : IEnumerable<ulong>
    {
        public ulong[] MoveBoard;

        public Board() { MoveBoard = new ulong[64]; }

        public Board(ulong[] initValues) : this()
        {
            Debug.Assert(initValues.Length == 64);
            Array.Copy(initValues, MoveBoard, 64);
        }

        public ulong this[int k]
        {
            get => MoveBoard[k];
            set => MoveBoard[k] = value;
        }

        public ulong this[int rank, int file]
        {
            get => MoveBoard[rank * 8 + file];
            set => MoveBoard[rank * 8 + file] = value;
        }

        public IEnumerator GetEnumerator()
        {
            return MoveBoard.GetEnumerator();
        }

        IEnumerator<ulong> IEnumerable<ulong>.GetEnumerator()
        {
            return ((IEnumerable<ulong>)MoveBoard).GetEnumerator();
        }
    }
}