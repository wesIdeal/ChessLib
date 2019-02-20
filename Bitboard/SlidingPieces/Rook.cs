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
        public readonly ulong[] Patterns = new ulong[maxArraySize];
        public readonly BlockerAndMoveBoards[][] OccupancyBoardSet = new BlockerAndMoveBoards[64][];
        public readonly ulong[] MagicKey = new ulong[64];

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
                var attack = moves[index];
                Patterns[index] = moves[index];
                var occupancyPermutations = MoveInitializer.GetAllPermutations(attack);
                OccupancyBoardSet[index] = mi.GetPermutationsForMask(attack, occupancyPermutations, index).ToArray();
                MagicKey[index] = mi.GenerateKey(OccupancyBoardSet[index], attack.CountSetBits());
            }
        }

        public ulong this[Rank rank, File file]
        {
            get
            {
                var r = rank.ToInt();
                var f = file.ToInt();
                return Patterns[(r * 8) + f];
            }
        }

        public ulong this[int index]
        {
            get
            {
                return Patterns[index];
            }
        }

        public IEnumerator<ulong> GetEnumerator() => Patterns.ToList().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }

    public class RookPatterns : MovePatternStorage
    {
        public RookPatterns()
        {
            var bb = new Bitboard();
            Initialize(bb.RookAttackMask, new RookMovesGenerator());
        }

    }
}
