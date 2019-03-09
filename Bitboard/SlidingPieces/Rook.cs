using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MagicBitboard.SlidingPieces
{

    public class RookPatterns : MovePatternStorage
    {
        Bitboard bb = new Bitboard();
        public RookPatterns()
        {

            Initialize(bb.RookAttackMask, new RookMovesGenerator());
            var occupancy = (ulong)0x101000100000060;
            var movesForRookOna1 = GetLegalMoves(0, occupancy);
            Debug.WriteLine("OCCUPANCY\r\n" + occupancy.GetDisplayBits());
            Debug.WriteLine("MOVES\r\n" + movesForRookOna1.GetDisplayBits());
        }

        public ulong GetLegalMoves(uint positionIndex, ulong occupancyBoard)
        {
            var dt = DateTime.Now;
            var relevantOccupancy = occupancyBoard & bb.RookAttackMask[(positionIndex / 8), (positionIndex % 8)];
            var magicMoveIndex = (relevantOccupancy * MagicKey[positionIndex]) >> (64 - BitCounts[positionIndex]);
            var tmp = OccupancyAndMoveBoards[positionIndex].Where(x => x.Occupancy == relevantOccupancy);
            var index = Array.FindIndex(OccupancyAndMoveBoards[positionIndex], x => x.Occupancy == relevantOccupancy);
            var board = this.AttackArray[positionIndex][magicMoveIndex];
           
            return board;
        }

        public ulong GetHashedLegalMoves(uint positionIndex, ulong occupancyBoard)
        {
            var dt = DateTime.Now;
            var relevantOccupancy = occupancyBoard & bb.RookAttackMask[(positionIndex / 8), (positionIndex % 8)];
            var magicMoveIndex = (relevantOccupancy * MagicKey[positionIndex]) >> (64 - BitCounts[positionIndex]);
            var tmp = OccupancyAndMoveBoards[positionIndex].Where(x => x.Occupancy == relevantOccupancy);
            var index = Array.FindIndex(OccupancyAndMoveBoards[positionIndex], x => x.Occupancy == relevantOccupancy);
            
            var hashedAttackBoard = (ulong)AttackHashes[positionIndex][relevantOccupancy];
       
            return hashedAttackBoard;
        }
    }
}
