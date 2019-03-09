using MagicBitboard.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicBitboard.SlidingPieces
{
    public class MovePatternStorage : IEnumerable<ulong>
    {
        const ushort maxArraySize = 64;
        public readonly ulong[] AttackPatterns = new ulong[maxArraySize];
        public readonly BlockerAndMoveBoards[][] OccupancyAndMoveBoards = new BlockerAndMoveBoards[64][];
        public readonly ulong[] MagicKey = new ulong[64];
        public readonly ushort[] BitCounts = new ushort[64];
        public MovePatternStorage() { }

        public MovePatternStorage(ulong[,] moves, MoveInitializer mi)
        {
            Initialize(moves, mi);
        }

        public MovePatternStorage(ulong[] moves, MoveInitializer mi)
        {

            InitializeFromOneDimensionalArray(moves, mi);
        }



        protected void Initialize(ulong[,] moves, MoveInitializer moveInitializer)
        {
            if (moves.Length > maxArraySize) throw new ArgumentException($"Cannot hold more than {maxArraySize} elements in Move Storage array.");
            var attacks = new ulong[64];
            for (int r = 0; r < 8; r++)
            {
                var rankOffset = r * 8;
                for (int f = 0; f < 8; f++)
                {
                    var index = rankOffset + f;
                    attacks[index] = moves[r, f];
                }
            }
            InitializeFromOneDimensionalArray(attacks, moveInitializer);
        }

        private void InitializeFromOneDimensionalArray(ulong[] moves, MoveInitializer mi)
        {
            if (moves.Length > maxArraySize) throw new ArgumentException($"Cannot hold more than {maxArraySize} elements in Move Storage array.");
            for (int index = 0; index < 64; index++)
            {
                var attackBoard = moves[index];
                var setBitCount =(ushort) 12;
                AttackPatterns[index] = moves[index];
                var occupancyPermutations = MoveInitializer.GetAllPermutations(attackBoard);
                var permutations = mi.GetPermutationsForMask(attackBoard, occupancyPermutations, index).ToArray();
                OccupancyAndMoveBoards[index] = permutations;
                BitCounts[index] = setBitCount;
                MagicKey[index] = mi.GenerateKey(OccupancyAndMoveBoards[index], setBitCount);

            }
        }

        public ulong this[Rank rank, File file]
        {
            get
            {
                var r = rank.ToInt();
                var f = file.ToInt();
                return AttackPatterns[(r * 8) + f];
            }
        }

        public ulong this[int index]
        {
            get
            {
                return AttackPatterns[index];
            }
        }

        public IEnumerator<ulong> GetEnumerator() => AttackPatterns.ToList().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }

    public class RookPatterns : MovePatternStorage
    {
        Bitboard bb = new Bitboard();
        public RookPatterns()
        {

            Initialize(bb.RookAttackMask, new RookMovesGenerator());
            var movesForRookOna1 = GetLegalMoves(0, 0x101000100000060);
        }

        public ulong GetLegalMoves(uint positionIndex, ulong occupancyBoard)
        {
            var relevantOccupancy = occupancyBoard & bb.RookAttackMask[(positionIndex / 8), (positionIndex % 8)];
            var magicMoveIndex = (relevantOccupancy * MagicKey[positionIndex]) >> (64 - BitCounts[positionIndex]);
            var tmp = OccupancyAndMoveBoards[positionIndex].Where(x => x.Occupancy == relevantOccupancy);
            var index = Array.FindIndex(OccupancyAndMoveBoards[positionIndex], x => x.Occupancy == relevantOccupancy);
            var board = this.OccupancyAndMoveBoards[positionIndex][magicMoveIndex];
            return board.MoveBoard;
        }
    }
}
