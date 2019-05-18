﻿using ChessLib.Data;
using ChessLib.Data.PieceMobility;
using ChessLib.Data.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ChessLib.Data
{
    public class MovePatternStorage : IEnumerable<ulong>
    {
        const ushort MaxArraySize = 64;
        public readonly ulong[] AttackPatterns = new ulong[MaxArraySize];
        public readonly BlockerAndMoveBoards[][] OccupancyAndMoveBoards = new BlockerAndMoveBoards[64][];
        public readonly ulong[] MagicKey = new ulong[64];
        public ulong[][] AttackArray = new ulong[64][];

        public void Initialize(Board board, MoveInitializer moveInitializer)
        {
            InitializeFromOneDimensionalArray(board.MoveBoard, moveInitializer);
        }
        public void Initialize(ulong[,] moves, MoveInitializer moveInitializer)
        {
            if (moves.Length > MaxArraySize) throw new ArgumentException($"Cannot hold more than {MaxArraySize} elements in Move Storage array.");
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
        private const int ArraySize = 12;
        public void InitializeFromOneDimensionalArray(ulong[] moves, MoveInitializer mi)
        {
            if (moves.Length > MaxArraySize) throw new ArgumentException($"Cannot hold more than {MaxArraySize} elements in Move Storage array.");
            for (int index = 0; index < 64; index++)
            {
                var attackBoard = moves[index];
                var setBitCount = ArraySize;//= attackBoard.CountSetBits();
                AttackPatterns[index] = moves[index];
                var occupancyPermutations = MoveInitializer.GetAllPermutations(attackBoard);
                var permutations = mi.GetAllPermutationsForAttackMask(index, attackBoard, occupancyPermutations).ToArray();
                OccupancyAndMoveBoards[index] = permutations;
                MagicKey[index] = mi.GenerateMagicKey(OccupancyAndMoveBoards[index], setBitCount, out ulong[] attackArray);
                AttackArray[index] = attackArray;
            }
        }

        public ulong this[Rank rank, File file]
        {
            get
            {
                var r = (int)rank;
                var f = (int)file;
                return AttackPatterns[(r * 8) + f];
            }
        }

        public ulong this[int index] => AttackPatterns[index];

        public IEnumerator<ulong> GetEnumerator() => AttackPatterns.ToList().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public ulong GetLegalMoves(uint positionIndex, ulong occupancyBoard)
        {
            var relevantOccupancy = occupancyBoard & AttackPatterns[positionIndex];
            var magicMoveIndex = (relevantOccupancy * MagicKey[positionIndex]) >> (64 - ArraySize);
            var board = AttackArray[positionIndex][magicMoveIndex];
            return board;
        }
    }
}