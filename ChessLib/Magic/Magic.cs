using System.Collections.Generic;
using System.Text;

namespace ChessLib.Magic
{
    public struct BlockerAndMoveBoards
    {
        public BlockerAndMoveBoards(ulong blockBoard, ulong moveBoard) : this()
        {
            BlockerBoard = blockBoard;
            MoveBoard = moveBoard;
        }

        public ulong BlockerBoard { get; set; }
        public ulong MoveBoard { get; set; }
    }
    class Magic
    {
        //public List<BlockerAndMoveBoards> GetRookBlockerBoardsAndMoveBoards(ulong rookAttackPatternAtPosition, int rookPositionIndex)
        //{
        //    var relevantOccupancyMasks = GenerateSlidingPieceOccupancyBoards();
        //    var rookOccupancyMasks = relevantOccupancyMasks[0];
        //    var bishopOccupancyMasks = relevantOccupancyMasks[1];
        //}

        public ulong[][] GenerateSlidingPieceOccupancyBoards()
        {
            int i;
            int bitRef;
            ulong mask;
            var occupancyMask = new ulong[2][];
            var occupancyMaskRook = occupancyMask[0];
            var occupancyMaskBishop = occupancyMask[1];
            for (bitRef = 0; bitRef <= 63; bitRef++)
            {
                mask = 0;
                for (i = bitRef + 8; i <= 55; i += 8) mask |= (((ulong)1) << i);
                for (i = bitRef - 8; i >= 8; i -= 8) mask |= (((ulong)1) << i);
                for (i = bitRef + 1; i % 8 != 7 && i % 8 != 0; i++) mask |= (((ulong)1) << i);
                for (i = bitRef - 1; i % 8 != 7 && i % 8 != 0; i--) mask |= (((ulong)1) << i);
                occupancyMaskRook[bitRef] = mask;



                mask = 0;
                for (i = bitRef + 9; i % 8 != 7 && i % 8 != 0 && i <= 55; i += 9) mask |= (((ulong)1) << i);
                for (i = bitRef - 9; i % 8 != 7 && i % 8 != 0 && i >= 8; i -= 9) mask |= (((ulong)1) << i);
                for (i = bitRef + 7; i % 8 != 7 && i % 8 != 0 && i <= 55; i += 7) mask |= (((ulong)1) << i);
                for (i = bitRef - 7; i % 8 != 7 && i % 8 != 0 && i >= 8; i -= 7) mask |= (((ulong)1) << i);
                occupancyMaskBishop[bitRef] = mask;
            }
            return occupancyMask;
        }

        public List<ulong> GetBlockerBoardsForPosition(ulong position)
        {
            var rv = new List<ulong>();
            var indexes = position.GetBitIndexesForPosition();
            var permCount = 1 << indexes.Count;

            for (var i = 0; i < permCount; i++)
            {
                rv.Add(i.GeneratePermutation(indexes));
            }

            return rv;
        }

        
    }
}
