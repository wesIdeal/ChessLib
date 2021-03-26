using System;
using ChessLib.Core.MagicBitboard.Storage;

namespace ChessLib.Core.MagicBitboard
{
    internal class MagicGenerator
    {
        /// <summary>
        ///     Generates magic multiplier to retrieve moves for a given piece on a square
        /// </summary>
        /// <param name="moveObstructionBoards">The object containing occupancy and resultant moves</param>
        /// <returns>The magic key which was found</returns>
        public Core.MagicBitboard.Storage.MagicBitboard GenerateMagicKey(MoveObstructionBoard[] moveObstructionBoards)
        {
            var countOfSetBits = 12;
            var maxMoves = 1 << countOfSetBits;
            var emptyArray = new ulong[maxMoves];
            var attackArray = new ulong[maxMoves];
            Array.Copy(emptyArray, attackArray, maxMoves);

            var key = (ulong)0;
            var foundCollision = true;
            while (foundCollision)
            {
                foundCollision = false;
                key = GetRandomKey();
                attackArray = new ulong[maxMoves];
                foreach (var pattern in moveObstructionBoards)
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

            return new Core.MagicBitboard.Storage.MagicBitboard(key, attackArray);
        }

        #region Random Number Helpers

        private readonly Random _random = new Random(DateTime.Now.Millisecond);

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
    }
}