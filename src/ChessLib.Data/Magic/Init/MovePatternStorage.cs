using ChessLib.Data.PieceMobility;
using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLib.Data.Magic.Init
{
    internal class MovePatternStorage : IEnumerable<ulong>
    {
        const ushort MaxArraySize = 64;
        public readonly ulong[] AttackPatterns = new ulong[MaxArraySize];
        public readonly BlockerAndMoveBoards[][] OccupancyAndMoveBoards = new BlockerAndMoveBoards[64][];
        public readonly ulong[] MagicKey = new ulong[64];
        public ulong[][] AttackArray = new ulong[64][];

        public void Initialize(MoveBoard board, MoveInitializer moveInitializer)
        {
            InitializeFromOneDimensionalArray(board.Board, moveInitializer);
        }

        private const int ArraySize = 12;
        public void InitializeFromOneDimensionalArray(ulong[] moves, MoveInitializer mi)
        {
            if (moves.Length > MaxArraySize) throw new ArgumentException($"Cannot hold more than {MaxArraySize} elements in Move Storage array.");
            Debug.WriteLine($"{DateTime.Now.TimeOfDay}\tBeginning {mi.GetType()} initialization routing.");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            //Parallel.For(0, 64, (index) =>
            for (int index = 0; index < 64; index++)
            {
                Debug.WriteLine($"\t\t{mi.GetType().Name}: index {index}");
                var attackBoard = moves[index];
                var setBitCount = ArraySize;//= attackBoard.CountSetBits();
                AttackPatterns[index] = moves[index];
                var occupancyPermutations = MoveInitializer.GetAllPermutations(attackBoard);
                var permutations = mi.GetAllPermutationsForAttackMask(index, attackBoard, occupancyPermutations).ToArray();
                OccupancyAndMoveBoards[index] = permutations;
                MagicKey[index] = mi.GenerateMagicKey(OccupancyAndMoveBoards[index], setBitCount, out ulong[] attackArray);
                AttackArray[index] = attackArray;
            }
            //);
            sw.Stop();
            Debug.WriteLine($"{DateTime.Now.TimeOfDay}\tFinished {mi.GetType().Name} initialization in {sw.ElapsedMilliseconds} ms.");

        }

        public ulong this[Rank rank, File file]
        {
            get
            {
                var r = (int)rank;
                var f = (int)file;
                return AttackPatterns[r * 8 + f];
            }
        }

        public ulong this[int index] => AttackPatterns[index];

        public IEnumerator<ulong> GetEnumerator() => AttackPatterns.ToList().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public ulong GetLegalMoves(uint positionIndex, ulong occupancyBoard)
        {
            var relevantOccupancy = occupancyBoard & AttackPatterns[positionIndex];
            var magicMoveIndex = relevantOccupancy * MagicKey[positionIndex] >> 64 - ArraySize;
            var board = AttackArray[positionIndex][magicMoveIndex];
            return board;
        }
    }
}
