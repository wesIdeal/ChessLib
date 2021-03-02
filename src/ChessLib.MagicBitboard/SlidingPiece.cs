using ChessLib.Data.Types.Enums;
using ChessLib.MagicBitboard.Bitwise;
using ChessLib.MagicBitboard.Storage;
using System;
using System.Diagnostics;
using System.Linq;

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

        public override ulong GetPseudoLegalMoves(ushort square, Color playerColor, ulong occupancy)
        {
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
                var blockerPermutations = GetBlockerBoardsFromMoves(attackMask);
                rv[squareIndex] = blockerPermutations.Select(blockerBoard =>
                    new MoveObstructionBoard(blockerBoard, GetMoves(squareIndex, blockerBoard))).ToArray();
            }

            return rv;
        }

        /// <summary>
        ///     Gets the permutations of blockers for a given move moveMask.
        /// </summary>
        /// <param name="moveMask">The relevant move moveMask</param>
        /// <returns>All relevant occupancy boards for the given moveMask</returns>
        public ulong[] GetBlockerBoardsFromMoves(in ulong moveMask)
        {
            var setBitIndices = MovingPieceService.GetSetBits(moveMask);
            return MovingPieceService.GetAllPermutationsOfSetBits(setBitIndices,0,0).Distinct().ToArray();
        }
    }

    public enum PieceDirection
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