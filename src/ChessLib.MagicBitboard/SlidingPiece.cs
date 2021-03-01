using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ChessLib.Data.Types.Enums;
using ChessLib.MagicBitboard.Bitwise;
using ChessLib.MagicBitboard.Storage;

namespace ChessLib.MagicBitboard
{
    internal abstract class SlidingPiece : MovingPiece
    {
        public MoveObstructionBoard[][] BlockerBoards = new MoveObstructionBoard[64][];
        public MagicBitboard[] MagicBitboard { get; private set; }
        protected abstract Func<ulong, ulong>[] DirectionalMethods { get; }
        protected abstract Func<ulong, ulong>[] AttackDirections { get; }

        public override void Initialize()
        {
            SetAttacks();
            BlockerBoards = GetBlockersFromMoveMasks();
           MagicBitboard = GetMagicBitboards();
        }

        public override ulong GetPsuedoLegalMoves(ushort square, Color playerColor, ulong playerOccupancy, ulong opponentOccupancy)
        {
            ulong occupancy = playerOccupancy | opponentOccupancy;
            return MagicBitboard[square].GetAttacks(occupancy);
        }

        private MagicBitboard[] GetMagicBitboards()
        {
            var generator = new MagicGenerator();
            var rv = new MagicBitboard[64];
            for (var index = 0; index < 64; index++)
            {
                var blockerBoard = BlockerBoards[index];
                var message = $"Square {index}, {blockerBoard.Length} boards. Move board is {blockerBoard.First().MoveBoard}, blocker board is {blockerBoard.First().Occupancy}";
                Console.WriteLine(message);
                Debug.WriteLine(message);
                rv[index] = generator.GenerateMagicKey(blockerBoard);
            }

            return rv;
        }

        protected void SetAttacks()
        {
            MoveMask = new ulong[64];
            AttackMask = new ulong[64];
            foreach (var squareIndex in AllSquares)
            {
                MoveMask[squareIndex] = GetMoves(squareIndex, 0);
                AttackMask[squareIndex] = GetAttacks(squareIndex, 0);
            }
        }

        private ulong GetAttacks(ushort square, ulong occupancy)
        {
            ulong result = 0;
            var squareValue = MovingPieceService.GetBoardValueOfIndex(square);
            foreach (var shiftDirection in AttackDirections)
            {
                if (shiftDirection == null) continue;
                result |= Traverse(shiftDirection, squareValue, occupancy);
            }

            return result;
        }
        private ulong GetMoves(ushort square, ulong occupancy)
        {
            ulong result = 0;
            var squareValue = MovingPieceService.GetBoardValueOfIndex(square);
            foreach (var shiftDirection in DirectionalMethods)
            {
                if (shiftDirection == null) continue;
                result |= Traverse(shiftDirection, squareValue, occupancy);
            }

            return result;
        }

        private ulong Traverse(Func<ulong, ulong> traversalFunc, in ulong value, in ulong occupancy)
        {
            var result = (ulong)0;
            var currentValue = value;
            while ((currentValue = traversalFunc(currentValue)) != 0)
            {
                result |= currentValue;
                if ((currentValue & occupancy) == currentValue)
                {
                    break;
                }
            }

            return result;
        }


        private MoveObstructionBoard[][] GetBlockersFromMoveMasks()
        {
            var rv = new MoveObstructionBoard[64][];
            foreach (var squareIndex in AllSquares)
            {
                var attackMask = AttackMask[squareIndex];
                var blockerPermutations = GetAllBlockerPermutationsFromMoveMask(attackMask).OrderBy(x => x).Distinct().ToArray();
               
                rv[squareIndex] = blockerPermutations.Select(blockerBoard =>
                    new MoveObstructionBoard(blockerBoard, GetMoves(squareIndex, blockerBoard))).ToArray();
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
                using (IEnumerator<ulong> occupancyPermutations =
                    GetAllBlockerPermutationsFromMoveMask(setBits, index, value).GetEnumerator())
                {
                    while (occupancyPermutations.MoveNext())
                    {
                        yield return occupancyPermutations.Current;
                    }
                }
            }

            value = MovingPieceSvc.ClearBit(value, setBits[idx]);
            yield return value;
            if (index < setBits.Length)
            {
                using (IEnumerator<ulong> occupancyPermutations =
                    GetAllBlockerPermutationsFromMoveMask(setBits, index, value).GetEnumerator())
                {
                    while (occupancyPermutations.MoveNext())
                    {
                        yield return occupancyPermutations.Current;
                    }
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