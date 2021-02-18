using ChessLib.Data.Types.Enums;
using ChessLib.MagicBitboard.Storage;
using System;

namespace ChessLib.MagicBitboard
{
    internal interface IMovingPiece
    {
        ulong GetPsuedoLegalMoves(ushort square, Color playerColor, ulong playerOccupancy, ulong opponentOccupancy);
        void Initialize();
    }

    abstract class MovingPiece : IMovingPiece
    {
        /// <summary>
        /// Generates magic multiplier to retrieve moves for a given piece on a square
        /// </summary>
        /// <param name="blockerAndMoveBoards">The object containing occupancy and resultant moves</param>
        /// <param name="countOfSetBits">The number of set bits in the move mask. Used to determine the array size.</param>
        /// <param name="attackArray">The newly ordered array of attacks, based on the calculated magic number</param>
        /// <returns>The magic key which was found</returns>
        protected ulong GenerateMagicKey(MoveObstructionBoard[] blockerAndMoveBoards, int countOfSetBits, out ulong[] attackArray)
        {
            var maxMoves = 1 << countOfSetBits;
            ulong[] emptyArray = new ulong[maxMoves];
            attackArray = new ulong[maxMoves];
            Array.Copy(emptyArray, attackArray, maxMoves);

            var key = (ulong)0;
            var foundCollision = true;
            while (foundCollision)
            {
                key = GetRandomKey();
                foundCollision = false;
                attackArray = new ulong[maxMoves];
                foreach (var pattern in blockerAndMoveBoards)
                {
                    var hash = (pattern.Occupancy * key) >> (64 - countOfSetBits);
                    if (attackArray[hash] != 0 && attackArray[hash] != pattern.MoveBoard)
                    {
                        foundCollision = true;
                        break;
                    }

                    attackArray[hash] = pattern.MoveBoard;
                }
            }
            return key;
        }
        #region Random Number Helpers
        private readonly Random _random = new Random();

        public ulong NextRandom()
        {
            var leftPart = (ulong)_random.Next() << 32;
            var rightPart = (ulong)_random.Next();
            return leftPart | rightPart;
        }

        protected ulong GetRandomKey()
        {
            return NextRandom() & NextRandom() & NextRandom();
        }
        #endregion
        public abstract ulong GetPsuedoLegalMoves(ushort square, Color playerColor, ulong playerOccupancy, ulong opponentOccupancy);
        public abstract void Initialize();
    }
}