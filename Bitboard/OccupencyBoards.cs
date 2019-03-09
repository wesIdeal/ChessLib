using System;
using System.Collections.Generic;

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
}
