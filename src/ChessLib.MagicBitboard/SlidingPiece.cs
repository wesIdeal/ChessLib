using System;
using System.Collections.Generic;
using System.Linq;
using ChessLib.MagicBitboard.Bitwise;
using ChessLib.MagicBitboard.Storage;

namespace ChessLib.MagicBitboard
{
    internal abstract class SlidingPiece : MovingPiece
    {
        public MoveObstructionBoard[][] BlockerBoards = new MoveObstructionBoard[64][];
        public MagicBitboard[] MagicBitboard { get; private set; }
        protected abstract Func<ulong, ulong>[] DirectionalMethods { get; }

        public override void Initialize()
        {
            MoveMask = GetAttacks();
            BlockerBoards = GetBlockersFromMoveMasks();
            MagicBitboard = GetMagicBitboards();
        }

        private MagicBitboard[] GetMagicBitboards()
        {
            var generator = new MagicGenerator();
            var rv = new MagicBitboard[64];
            for (var index = 0; index < 64; index++)
            {
                var blockerBoard = BlockerBoards[index];
                rv[index] = generator.GenerateMagicKey(blockerBoard);
            }

            return rv;
        }

        protected ulong[] GetAttacks()
        {
            var moveArray = new ulong[64];
            foreach (var squareIndex in AllSquares)
            {
                moveArray[squareIndex] = GetAttacks(squareIndex, 0);
            }

            return moveArray;
        }

        private ulong GetAttacks(ushort square, ulong occupancy)
        {
            ulong result = 0;
            var squareValue = MovingPieceService.GetBoardValueOfIndex(square);
            foreach (var shiftDirection in DirectionalMethods)
            {
                if (shiftDirection != null)
                {
                    var shiftedValue = squareValue;

                    while ((shiftedValue = shiftDirection(shiftedValue)) != 0)
                    {
                        result |= shiftedValue;
                        if ((shiftedValue & occupancy) != 0)
                        {
                            break;
                        }
                    }
                }
            }

            return result;
        }


        private MoveObstructionBoard[][] GetBlockersFromMoveMasks()
        {
            var rv = new MoveObstructionBoard[64][];
            for (ushort squareIndex = 0; squareIndex < 64; squareIndex++)
            {
                var mask = MoveMask[squareIndex];
                var blockerPermutations = GetAllBlockerPermutationsFromMoveMask(mask);
                var index = squareIndex;
                rv[index] = blockerPermutations.Select(blockerBoard =>
                    new MoveObstructionBoard(blockerBoard, GetAttacks(index, blockerBoard))).ToArray();
            }

            return rv;
        }

        /// <summary>
        ///     Gets the permutations of blockers for a given attack mask.
        /// </summary>
        /// <param name="mask">The relevant attack mask</param>
        /// <returns>All relevant occupancy boards for the given mask</returns>
        public ulong[] GetAllBlockerPermutationsFromMoveMask(in ulong mask)
        {
            var setBitIndices = MovingPieceService.GetSetBits(mask);
            return GetAllBlockerPermutationsFromMoveMask(setBitIndices, 0, 0).Distinct().ToArray();
        }

        private IEnumerable<ulong> GetAllBlockerPermutationsFromMoveMask(ushort[] setBits, int idx, ulong value)
        {
            value = MovingPieceSvc.SetBit(value, setBits[idx]);
            yield return value;
            var index = idx + 1;
            if (index < setBits.Length)
            {
                using var occupancyPermutations =
                    GetAllBlockerPermutationsFromMoveMask(setBits, index, value).GetEnumerator();
                while (occupancyPermutations.MoveNext())
                {
                    yield return occupancyPermutations.Current;
                }
            }

            value = MovingPieceSvc.ClearBit(value, setBits[idx]);
            yield return value;
            if (index < setBits.Length)
            {
                using var occupancyPermutations =
                    GetAllBlockerPermutationsFromMoveMask(setBits, index, value).GetEnumerator();
                while (occupancyPermutations.MoveNext())
                {
                    yield return occupancyPermutations.Current;
                }
            }
        }

        protected enum PieceDirection
        {
            South = -8,
            West = -1,
            East = -West,
            North = -South,
            NorthEast = North + East,
            SouthEast = South + East,
            NorthWest = North + West,
            SouthWest = South + West
        }
    }
}