using MagicBitboard.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MagicBitboard.SlidingPieces
{
    public class MovePatternStorage : IEnumerable<ulong>
    {
        const ushort maxArraySize = 64;
        public readonly ulong[] AttackPatterns = new ulong[maxArraySize];
        public readonly BlockerAndMoveBoards[][] OccupancyAndMoveBoards = new BlockerAndMoveBoards[64][];
        public readonly ulong[] MagicKey = new ulong[64];
        public readonly ushort[] BitCounts = new ushort[64];
        public ulong[][] AttackArray = new ulong[64][];
        public Hashtable[] AttackHashes = new Hashtable[64];
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
                AttackHashes[index] = new Hashtable();
                var attackBoard = moves[index];
                var setBitCount = (ushort)12;
                AttackPatterns[index] = moves[index];
                var occupancyPermutations = MoveInitializer.GetAllPermutations(attackBoard);
                var permutations = mi.GetPermutationsForMask(attackBoard, occupancyPermutations, index).ToArray();
                OccupancyAndMoveBoards[index] = permutations;
                BitCounts[index] = setBitCount;
                MagicKey[index] = mi.GenerateKey(OccupancyAndMoveBoards[index], setBitCount, out ulong[] attackArray);
                AttackArray[index] = attackArray;
                permutations.ToList().ForEach(x => AttackHashes[index].Add(x.Occupancy, x.MoveBoard));
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
}
