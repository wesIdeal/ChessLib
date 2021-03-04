#region

using ChessLib.Data.Types.Enums;
using ChessLib.MagicBitboard.Bitwise;
using ChessLib.MagicBitboard.Storage;
using System;
using System.Diagnostics;
using System.Linq;

#endregion

namespace ChessLib.MagicBitboard.MovingPieces
{
    internal abstract class SlidingPiece : MovingPiece
    {
        public MoveObstructionBoard[][] MoveObstructionBoards = new MoveObstructionBoard[64][];
        public Storage.MagicBitboard[] MagicBitboard { get; private set; }
        protected abstract Func<ulong, ulong>[] MoveShifts { get; }
        protected abstract Func<ulong, ulong>[] AttackShifts { get; }

        public override void Initialize()
        {
            SetAttacks();
            MoveObstructionBoards = GetObstructionsFromMoveMasks();
            MagicBitboard = GetMagicBitboards();
        }

        public override ulong GetPseudoLegalMoves(ushort square, Color playerColor, ulong occupancy)
        {
            return MagicBitboard[square].GetAttacks(occupancy);
        }

        private Storage.MagicBitboard[] GetMagicBitboards()
        {
            var generator = new MagicGenerator();
            var rv = new Storage.MagicBitboard[64];
            for (var index = 0; index < 64; index++)
            {
                var obstructionBoard = MoveObstructionBoards[index];
                var message =
                    $"Square {index}, {obstructionBoard.Length} boards. Move board is {obstructionBoard.First().MoveBoard}, occupancy board is {obstructionBoard.First().Occupancy}";
                Console.WriteLine(message);
                Debug.WriteLine(message);
                rv[index] = generator.GenerateMagicKey(obstructionBoard);
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
            foreach (var shiftDirection in AttackShifts)
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
            foreach (var shiftDirection in MoveShifts)
            {
                if (shiftDirection == null) continue;
                result |= Traverse(shiftDirection, squareValue, occupancy);
            }

            return result;
        }

        private ulong Traverse(Func<ulong, ulong> traversalFunc, in ulong value, in ulong occupancy)
        {
            var result = (ulong) 0;
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


        private MoveObstructionBoard[][] GetObstructionsFromMoveMasks()
        {
            var rv = new MoveObstructionBoard[64][];
            foreach (var squareIndex in AllSquares)
            {
                var attackMask = AttackMask[squareIndex];
                var potentialOccupancyPermutations = GetOccupancyBoardsFromMoveSet(attackMask);
                rv[squareIndex] = potentialOccupancyPermutations.Select(occupancyBoard =>
                    new MoveObstructionBoard(occupancyBoard, GetMoves(squareIndex, occupancyBoard))).ToArray();
            }

            return rv;
        }

        /// <summary>
        ///     Gets the permutations of obstructions for a given move moveMask.
        /// </summary>
        /// <param name="moveMask">The relevant move moveMask</param>
        /// <returns>All relevant occupancy boards for the given moveMask</returns>
        public ulong[] GetOccupancyBoardsFromMoveSet(in ulong moveMask)
        {
            var setBitIndices = MovingPieceService.GetSetBits(moveMask);
            return MovingPieceService.GetAllPermutationsOfSetBits(setBitIndices, 0, 0).Distinct().ToArray();
        }
    }
}