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

        public override ulong GenerateKey(BlockerAndMoveBoards[] blockerAndMoveBoards, int maskLength)
        {
            var maxMoves = 1 << 12;
            var attacks = new ulong[maxMoves];

            var key = (ulong)0;
            var fail = true;
            var dtStart = DateTime.Now;
            while (fail)
            {
                key = GetRandomKey();
                fail = false;

                Array.Clear(attacks, 0, maxMoves);

                foreach (var pattern in blockerAndMoveBoards)
                {
                    var hash = (pattern.BlockerBoard * key) >> (64 - maskLength);
                    if (attacks[hash] != 0 && attacks[hash] != pattern.MoveBoard)
                    {
                        fail = true;
                        break;
                    }

                    attacks[hash] = pattern.MoveBoard;
                }
            }
            var totalMs = DateTime.Now.Subtract(dtStart).TotalMilliseconds;
            Debug.WriteLine($"Finished with key generation in {(DateTime.Now - dtStart).TotalMilliseconds}ms.");
            var nonZero = attacks.Count(x => x != 0);
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
