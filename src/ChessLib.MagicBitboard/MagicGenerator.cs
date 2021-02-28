using System;
using ChessLib.MagicBitboard.Bitwise;
using ChessLib.MagicBitboard.Storage;

namespace ChessLib.MagicBitboard
{
    internal class MagicGenerator
    {
        private MovingPieceService _movingPieceService = new MovingPieceService();

        /// <summary>
        ///     Generates magic multiplier to retrieve moves for a given piece on a square
        /// </summary>
        /// <param name="blockerAndMoveBoards">The object containing occupancy and resultant moves</param>
        /// <param name="countOfSetBits">The number of set bits in the move mask. Used to determine the array size.</param>
        /// <param name="attackArray">The newly ordered array of attacks, based on the calculated magic number</param>
        /// <returns>The magic key which was found</returns>
        public MagicBitboard GenerateMagicKey(MoveObstructionBoard[] blockerAndMoveBoards)
        {
            var countOfSetBits = 12; // MovingPieceService.GetSetBits(blockerAndMoveBoards.First().MoveBoard).Length;
            var maxMoves = 1 << countOfSetBits;
            var emptyArray = new ulong[maxMoves];
            var attackArray = new ulong[maxMoves];
            Array.Copy(emptyArray, attackArray, maxMoves);

            var key = (ulong) 0;
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

            return new MagicBitboard(key, attackArray);
        }

        #region Random Number Helpers

        private readonly Random _random = new Random();

        public ulong NextRandom()
        {
            var leftPart = (ulong) _random.Next() << 32;
            var rightPart = (ulong) _random.Next();
            return leftPart | rightPart;
        }

        protected ulong GetRandomKey()
        {
            return NextRandom() & NextRandom() & NextRandom();
        }

        #endregion
    }
}