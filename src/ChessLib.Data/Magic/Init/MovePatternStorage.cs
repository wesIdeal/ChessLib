using ChessLib.Data.PieceMobility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Helpers;
using ChessLib.Data.Helpers;

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

        public void InitializePawns(MoveBoard board, PawnMovesInitializer initializer)
        {
            var moves = board.Board;
            if (moves.Length > MaxArraySize) throw new ArgumentException($"Cannot hold more than {MaxArraySize} elements in MoveValue Storage array.");
            for (int index = 8; index < 56; index++)
            {
                var attackBoard = moves[index];
                ulong[] attackArray = new ulong[0];
                if (attackBoard != 0)
                {
                    var setBitCount = ArraySize; //= attackBoard.CountSetBits();
                    AttackPatterns[index] = moves[index];
                    var occupancyPermutations = MoveInitializer.GetAllPermutations(attackBoard);
                    var permutations = initializer.GetAllPermutationsForAttackMask(index, attackBoard, occupancyPermutations)
                        .ToArray();
                    OccupancyAndMoveBoards[index] = permutations;
                    MagicKey[index] = initializer.GenerateMagicKey(OccupancyAndMoveBoards[index], setBitCount,
                        out attackArray);
                }
                AttackArray[index] = attackArray;
            }
            //);
        }

        private const int ArraySize = 12;
        public void InitializeFromOneDimensionalArray(ulong[] moves, MoveInitializer mi)
        {
            if (moves.Length > MaxArraySize) throw new ArgumentException($"Cannot hold more than {MaxArraySize} elements in MoveValue Storage array.");
            Debug.WriteLine($"{DateTime.Now.TimeOfDay}\tBeginning {mi.GetType()} initialization routing.");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            //Parallel.For(0, 64, (index) =>
            for (int index = 0; index < 64; index++)
            {
                var attackBoard = moves[index];
                ulong[] attackArray = new ulong[0];
                if (attackBoard != 0)
                {
                    var setBitCount = ArraySize; //= attackBoard.CountSetBits();
                    AttackPatterns[index] = moves[index];
                    var occupancyPermutations = MoveInitializer.GetAllPermutations(attackBoard);
                    var permutations = mi.GetAllPermutationsForAttackMask(index, attackBoard, occupancyPermutations)
                        .ToArray();
                    OccupancyAndMoveBoards[index] = permutations;
                    MagicKey[index] = mi.GenerateMagicKey(OccupancyAndMoveBoards[index], setBitCount,
                        out attackArray);
                }
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

        public ulong GetLegalMoves(uint positionIndex, ulong occupancyBoard, ushort? enPassantIdx = null)
        {
            var relevantOccupancy = occupancyBoard & AttackPatterns[positionIndex];
            var magicMoveIndex = relevantOccupancy * MagicKey[positionIndex] >> 64 - ArraySize;
            var board = AttackArray[positionIndex][magicMoveIndex];
            if (enPassantIdx.HasValue)
            {
                board |= enPassantIdx.Value.GetBoardValueOfIndex();
            }
            return board;
        }
    }
}
