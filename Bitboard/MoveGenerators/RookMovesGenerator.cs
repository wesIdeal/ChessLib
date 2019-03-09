using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MagicBitboard
{
    public class RookMovesGenerator : MoveInitializer
    {
        private Random _random = new Random();

        public ulong NextRandom()
        {
            var leftPart = (ulong)_random.Next() << 32;
            var rightPart = (ulong)_random.Next();

            return leftPart | rightPart;
        }

        private ulong GetRandomKey()
        {
            return NextRandom() & NextRandom() & NextRandom();
        }

        public override ulong CalculateMovesFromPosition(int positionIndex, ulong occupancyBoard)
        {
            var rv = (ulong)0;
            var startingValue = (ulong)1 << positionIndex;
            //N
            var positionalValue = startingValue;
            while ((positionalValue = positionalValue.ShiftN()) != 0)
            {

                rv |= positionalValue;
                if ((occupancyBoard & positionalValue) == positionalValue) break;
            }
            //E
            positionalValue = startingValue;

            while ((positionalValue = positionalValue.ShiftE()) != 0)
            {
                rv |= positionalValue;
                if ((occupancyBoard & positionalValue) == positionalValue) break;
            }
            //S
            positionalValue = startingValue;
            while ((positionalValue = positionalValue.ShiftS()) != 0)
            {
                rv |= positionalValue;
                if ((occupancyBoard & positionalValue) == positionalValue) break;
            }
            //W
            positionalValue = startingValue;
            while ((positionalValue = positionalValue.ShiftW()) != 0)
            {
                rv |= positionalValue;
                if ((occupancyBoard & positionalValue) == positionalValue) break;
            }
            return rv;
        }

        public override ulong GenerateKey(BlockerAndMoveBoards[] blockerAndMoveBoards, int maskLength, out ulong[] attackArray)
        {
            var maxMoves = 1 << maskLength;
            attackArray = new ulong[maxMoves];

            var key = (ulong)0;
            var fail = true;
            var dtStart = DateTime.Now;
            var count = 1;
            while (fail)
            {
                key = GetRandomKey();
                fail = false;

                Array.Clear(attackArray, 0, maxMoves);

                foreach (var pattern in blockerAndMoveBoards)
                {
                    var hash = (pattern.Occupancy * key) >> (64 - maskLength);
                    if (attackArray[hash] != 0 && attackArray[hash] != pattern.MoveBoard)
                    {
                        fail = true;
                        count++;
                        break;
                    }

                    attackArray[hash] = pattern.MoveBoard;
                }
            }
            var totalMs = DateTime.Now.Subtract(dtStart).TotalMilliseconds;
            Debug.WriteLine($"Finished with key generation in {(DateTime.Now - dtStart).TotalMilliseconds}ms, searched {count} times.");
            var nonZero = attackArray.Count(x => x != 0);
            return key;
        }

        public override IEnumerable<BlockerAndMoveBoards> GetPermutationsForMask(ulong attackMask, IEnumerable<ulong> occupancyBoard, int pieceLocationIndex)
        {
            var boardCombos = new List<BlockerAndMoveBoards>();
            var count = 0;
            var dtStart = DateTime.Now;
            var totalBoards = occupancyBoard.Count();
            foreach (var board in occupancyBoard)
            {
                //Debug.Write(string.Format("\r{0,4} | {1,-4}", count++, totalBoards));
                boardCombos.Add(new BlockerAndMoveBoards(board, CalculateMovesFromPosition(pieceLocationIndex, board)));
            }
            Debug.WriteLine($"Finished with index {pieceLocationIndex} in {(DateTime.Now - dtStart).TotalMilliseconds}ms.");
            return boardCombos;
        }
    }
}
