﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicBitboard
{
    public class OccupancyBoards
    {
        public ulong AttackMask { get; private set; }
        public BlockerAndMoveBoards[] BlockerBoards { get; private set; }

        public static OccupancyBoards GetBishopBoards(ulong attackMask, IEnumerable<ulong> n, int piecePositionIndex)
        {
            var bishopMovesGenerator = new BishopMovesGenerator();
            var rv = new OccupancyBoards();
            rv.AttackMask = attackMask;
            var boardCombos = new List<BlockerAndMoveBoards>();
            var startTime = DateTime.Now;
            foreach (var board in n)
            {
                boardCombos.Add(new BlockerAndMoveBoards(board, bishopMovesGenerator.CalculateMovesFromPosition(piecePositionIndex, board)));
            }
            var totalMS = DateTime.Now.Subtract(startTime).TotalMilliseconds;
            rv.BlockerBoards = boardCombos.ToArray();
            return rv;
        }

        //public static OccupancyBoards GetRookBoards(ulong attackMask, IEnumerable<ulong> n, int piecePositionIndex)
        //{
        //    var rookMovesGenerator = new RookMovesGenerator();
        //    var rv = new OccupancyBoards();
        //    rv.AttackMask = attackMask;
        //    var boardCombos = new List<BlockerAndMoveBoards>();
        //    var startTime = DateTime.Now;
        //    foreach (var board in n)
        //    {
        //        boardCombos.Add(new BlockerAndMoveBoards(board, rookMovesGenerator.CalculateMovesFromPosition(piecePositionIndex, board)));
        //    }
        //    var totalMS = DateTime.Now.Subtract(startTime).TotalMilliseconds;
        //    rv.BlockerBoards = boardCombos.ToArray();
        //    return rv;
        //}
    }

    public class BlockerAndMoveBoards
    {
        public BlockerAndMoveBoards(ulong blockerBoard, ulong moveBoard)
        {
            Occupancy = blockerBoard;
            MoveBoard = moveBoard;  
        }

        public ulong Occupancy { get; private set; }
        public ulong MoveBoard { get; private set; }

        public override string ToString()
        {
            var bb = Convert.ToString((long)Occupancy, 2).PadLeft(64, '0').ToCharArray();
            var mb = Convert.ToString((long)MoveBoard, 2).PadLeft(64, '0').ToCharArray();
            var bbHeader = "Blocker";
            var mbHeader = "Move Board";
            StringBuilder sb = new StringBuilder();
            var headerFormat = "{0,-20}{1,-20}\r\n{2,-20}{3,-20}\r\n";
            var format = "{0,-20}{1,-20}\r\n";
            sb.AppendFormat(headerFormat, bbHeader, mbHeader, Occupancy.ToHexDisplay(), MoveBoard.ToHexDisplay());
            
            for (int i = 0; i < 8; i++)
            {
                var blockBoard = string.Join(" ", (bb.Skip(8 * i).Take(8).ToArray().Reverse()));
                var moveBoard = string.Join(" ", mb.Skip(8 * i).Take(8).ToArray().Reverse());
                sb.AppendFormat(format, blockBoard, moveBoard);
            }
            sb.AppendLine();
            return sb.ToString();
        }
    }
}
