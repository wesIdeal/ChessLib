using ChessLib.MagicBitboard.Bitwise;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChessLib.MagicBitboard.Storage
{
    public class MoveObstructionBoard
    {
        public ulong Occupancy { get; }
        public ulong MoveBoard { get; }

        
        public MoveObstructionBoard(ulong blockerBoard, ulong moveBoard)
        {
            Occupancy = blockerBoard;
            MoveBoard = moveBoard;
        }
       
    }
}
